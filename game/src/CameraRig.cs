using Godot;

namespace BCity.Game;

/// <summary>RTS-style camera: WASD/arrows pan, Q/E rotate, mouse wheel zoom.</summary>
public partial class CameraRig : Node3D
{
    private const float PanSpeed = 400f;
    private const float RotateSpeed = 1.5f;
    private const float MinDistance = 40f;
    private const float MaxDistance = 1500f;

    private Camera3D _camera = null!;
    private float _distance = 650f;
    private float _pitch = Mathf.DegToRad(-50f);

    public override void _Ready()
    {
        _camera = new Camera3D { Far = 5000f, Current = true };
        AddChild(_camera);
        ApplyZoom();
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        var input = Vector3.Zero;
        if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up)) input.Z -= 1f;
        if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down)) input.Z += 1f;
        if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left)) input.X -= 1f;
        if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right)) input.X += 1f;
        if (Input.IsKeyPressed(Key.Q)) RotateY(RotateSpeed * dt);
        if (Input.IsKeyPressed(Key.E)) RotateY(-RotateSpeed * dt);

        if (input != Vector3.Zero)
        {
            float speed = PanSpeed * (_distance / 650f);
            Translate(input.Normalized() * speed * dt);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true } mouse)
        {
            if (mouse.ButtonIndex == MouseButton.WheelUp) _distance *= 0.9f;
            if (mouse.ButtonIndex == MouseButton.WheelDown) _distance *= 1.1f;
            _distance = Mathf.Clamp(_distance, MinDistance, MaxDistance);
            ApplyZoom();
        }
    }

    private void ApplyZoom()
    {
        _camera.Position = new Vector3(0f, -Mathf.Sin(_pitch) * _distance, Mathf.Cos(_pitch) * _distance);
        _camera.Rotation = new Vector3(_pitch, 0f, 0f);
    }
}
