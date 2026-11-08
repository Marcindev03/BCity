using BCity.Simulation;
using BCity.Simulation.Traffic;
using Godot;

namespace BCity.Game;

/// <summary>
/// M0 smoke scene: ring-road traffic rendered through a single MultiMesh.
/// Command-line user args (after "--"):
///   --frames=N          quit after N rendered frames and print stats
///   --screenshot=PATH   save a PNG of the last frame before quitting
/// </summary>
public partial class Main : Node3D
{
    private const int FloatsPerInstance = 16; // 3x4 transform + RGBA color
    private const int MaxTicksPerFrame = 8;

    private RingRoadSimulation _sim = null!;
    private MultiMesh _multiMesh = null!;
    private float[] _buffer = [];
    private double _accumulator;
    private int _framesToRun = -1;
    private int _frame;
    private double _frameTimeSum;
    private string? _screenshotPath;

    public override void _Ready()
    {
        ParseArgs();

        _sim = new RingRoadSimulation(rings: 20, vehiclesPerRing: 250, innerRadius: 400f, ringSpacing: 6f, seed: 7);
        _buffer = new float[_sim.VehicleCount * FloatsPerInstance];

        BuildEnvironment();
        BuildRoads();
        BuildVehicles();
        AddChild(new CameraRig());
    }

    public override void _Process(double delta)
    {
        _accumulator += delta;
        int ticks = 0;
        while (_accumulator >= SimulationConstants.TickSeconds && ticks < MaxTicksPerFrame)
        {
            _sim.Step();
            _accumulator -= SimulationConstants.TickSeconds;
            ticks++;
        }

        if (ticks == MaxTicksPerFrame)
        {
            _accumulator = 0; // drop time instead of spiralling when the machine is too slow
        }

        UpdateVehicleBuffer((float)(_accumulator / SimulationConstants.TickSeconds));
        TrackAutomation(delta);
    }

    private void UpdateVehicleBuffer(float alpha)
    {
        ReadOnlySpan<float> pos = _sim.Positions;
        ReadOnlySpan<float> prev = _sim.PreviousPositions;
        ReadOnlySpan<float> speed = _sim.Speeds;
        ReadOnlySpan<float> lengths = _sim.RingLengths;
        ReadOnlySpan<float> radii = _sim.RingRadii;
        float maxSpeed = _sim.Driver.DesiredSpeed;
        float[] b = _buffer;

        for (int v = 0; v < pos.Length; v++)
        {
            int ring = _sim.RingOf(v);
            float length = lengths[ring];
            float current = pos[v];
            if (current < prev[v])
            {
                current += length; // wrapped this tick
            }

            float s = prev[v] + (current - prev[v]) * alpha;
            float angle = s / length * Mathf.Tau;
            float c = MathF.Cos(angle);
            float sn = MathF.Sin(angle);
            float r = radii[ring];
            float t = Math.Clamp(speed[v] / maxSpeed, 0f, 1f);

            int o = v * FloatsPerInstance;
            b[o + 0] = -c; b[o + 1] = 0f; b[o + 2] = sn; b[o + 3] = r * c;
            b[o + 4] = 0f; b[o + 5] = 1f; b[o + 6] = 0f; b[o + 7] = 0.8f;
            b[o + 8] = -sn; b[o + 9] = 0f; b[o + 10] = -c; b[o + 11] = r * sn;
            b[o + 12] = 1f - t; b[o + 13] = 0.25f + 0.65f * t; b[o + 14] = 0.2f; b[o + 15] = 1f;
        }

        RenderingServer.MultimeshSetBuffer(_multiMesh.GetRid(), b);
    }

    private void BuildVehicles()
    {
        var material = new StandardMaterial3D { VertexColorUseAsAlbedo = true, Roughness = 0.8f };
        var mesh = new BoxMesh { Size = new Vector3(1.8f, 1.4f, 4.2f), Material = material };
        _multiMesh = new MultiMesh
        {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            UseColors = true,
            Mesh = mesh,
            InstanceCount = _sim.VehicleCount,
        };
        AddChild(new MultiMeshInstance3D { Multimesh = _multiMesh, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off });
    }

    private void BuildRoads()
    {
        var asphalt = new StandardMaterial3D { AlbedoColor = new Color(0.22f, 0.23f, 0.25f), Roughness = 1f };
        foreach (float radius in _sim.RingRadii)
        {
            var road = new MeshInstance3D
            {
                Mesh = new TorusMesh { InnerRadius = radius - 2f, OuterRadius = radius + 2f, Rings = 128, RingSegments = 4, Material = asphalt },
                Scale = new Vector3(1f, 0.02f, 1f),
                CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            };
            AddChild(road);
        }
    }

    private void BuildEnvironment()
    {
        var env = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Color,
            BackgroundColor = new Color(0.62f, 0.78f, 0.9f),
            AmbientLightSource = Godot.Environment.AmbientSource.Color,
            AmbientLightColor = new Color(0.75f, 0.78f, 0.85f),
            AmbientLightEnergy = 0.6f,
        };
        AddChild(new WorldEnvironment { Environment = env });
        AddChild(new DirectionalLight3D { RotationDegrees = new Vector3(-55f, 35f, 0f), ShadowEnabled = true });

        var grass = new StandardMaterial3D { AlbedoColor = new Color(0.45f, 0.62f, 0.38f), Roughness = 1f };
        AddChild(new MeshInstance3D { Mesh = new PlaneMesh { Size = new Vector2(2500f, 2500f), Material = grass } });
    }

    private void ParseArgs()
    {
        foreach (string arg in OS.GetCmdlineUserArgs())
        {
            if (arg.StartsWith("--frames=", StringComparison.Ordinal))
            {
                _framesToRun = int.Parse(arg["--frames=".Length..], System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (arg.StartsWith("--screenshot=", StringComparison.Ordinal))
            {
                _screenshotPath = arg["--screenshot=".Length..];
            }
        }
    }

    private void TrackAutomation(double delta)
    {
        if (_framesToRun < 0)
        {
            return;
        }

        _frame++;
        if (_frame > 30)
        {
            _frameTimeSum += delta; // skip startup frames
        }

        if (_frame < _framesToRun)
        {
            return;
        }

        if (_screenshotPath is not null && DisplayServer.GetName() != "headless")
        {
            Error err = GetViewport().GetTexture().GetImage().SavePng(_screenshotPath);
            GD.Print($"BCITY_SCREENSHOT path={_screenshotPath} result={err}");
        }

        int measured = Math.Max(1, _frame - 30);
        double avgMs = _frameTimeSum / measured * 1000.0;
        GD.Print($"BCITY_STATS frames={_frame} vehicles={_sim.VehicleCount} tick={_sim.Tick} avgFrameMs={avgMs:F2} fps={1000.0 / avgMs:F1} renderer={RenderingServer.GetCurrentRenderingDriverName()} gpu={RenderingServer.GetVideoAdapterName()}");
        GetTree().Quit();
    }
}
