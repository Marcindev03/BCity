namespace BCity.Simulation.Traffic;

/// <summary>
/// Smoke-test scenario: concentric single-lane ring roads with IDM car-following.
/// Shows phantom traffic jams emerging from small random perturbations.
/// Data is stored as struct-of-arrays; <see cref="Step"/> performs zero allocations.
/// Vehicles never overtake, so the leader of vehicle i is i+1 within its ring (wrapping).
/// </summary>
public sealed class RingRoadSimulation
{
    private readonly float[] _position;
    private readonly float[] _previousPosition;
    private readonly float[] _speed;
    private readonly float[] _ringLength;
    private readonly float[] _ringRadius;
    private readonly DriverParameters _driver;
    private readonly float _perturbationChance;
    private DeterministicRandom _random;

    public RingRoadSimulation(
        int rings,
        int vehiclesPerRing,
        float innerRadius,
        float ringSpacing,
        uint seed,
        DriverParameters? driver = null,
        float perturbationChance = 0.002f)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rings, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(vehiclesPerRing, 2);

        Rings = rings;
        VehiclesPerRing = vehiclesPerRing;
        _driver = driver ?? DriverParameters.CityCar;
        _perturbationChance = perturbationChance;
        _random = new DeterministicRandom(seed);

        int count = rings * vehiclesPerRing;
        _position = new float[count];
        _previousPosition = new float[count];
        _speed = new float[count];
        _ringLength = new float[rings];
        _ringRadius = new float[rings];

        for (int r = 0; r < rings; r++)
        {
            float radius = innerRadius + r * ringSpacing;
            _ringRadius[r] = radius;
            _ringLength[r] = 2f * MathF.PI * radius;
            float spacing = _ringLength[r] / vehiclesPerRing;
            if (spacing < _driver.VehicleLength + _driver.MinimumGap)
            {
                throw new ArgumentException($"Ring {r} is too short for {vehiclesPerRing} vehicles.");
            }

            for (int i = 0; i < vehiclesPerRing; i++)
            {
                int v = r * vehiclesPerRing + i;
                _position[v] = i * spacing;
                _previousPosition[v] = _position[v];
            }
        }
    }

    public int Rings { get; }
    public int VehiclesPerRing { get; }
    public int VehicleCount => _position.Length;
    public long Tick { get; private set; }

    public ReadOnlySpan<float> Positions => _position;
    public ReadOnlySpan<float> PreviousPositions => _previousPosition;
    public ReadOnlySpan<float> Speeds => _speed;
    public ReadOnlySpan<float> RingLengths => _ringLength;
    public ReadOnlySpan<float> RingRadii => _ringRadius;
    public DriverParameters Driver => _driver;

    public int RingOf(int vehicle) => vehicle / VehiclesPerRing;

    /// <summary>Distance along the ring from the rear of the leader to the front of this vehicle.</summary>
    public float GapToLeader(int vehicle)
    {
        int ring = RingOf(vehicle);
        int first = ring * VehiclesPerRing;
        int leader = vehicle + 1 == first + VehiclesPerRing ? first : vehicle + 1;
        float d = _position[leader] - _position[vehicle];
        if (d <= 0f)
        {
            d += _ringLength[ring];
        }

        return d - _driver.VehicleLength;
    }

    public void Step()
    {
        const float dt = SimulationConstants.TickSeconds;
        DriverParameters p = _driver;
        float sqrtAb = MathF.Sqrt(p.MaxAcceleration * p.ComfortableDeceleration);
        int perRing = VehiclesPerRing;

        Array.Copy(_position, _previousPosition, _position.Length);

        for (int r = 0; r < Rings; r++)
        {
            int first = r * perRing;
            float length = _ringLength[r];

            // Snapshot of vehicle 0 so the last vehicle sees its leader's pre-tick state.
            float firstPos = _position[first];
            float firstSpeed = _speed[first];

            for (int i = 0; i < perRing; i++)
            {
                int v = first + i;
                bool leaderIsFirst = i == perRing - 1;
                float leaderPos = leaderIsFirst ? firstPos : _position[v + 1];
                float leaderSpeed = leaderIsFirst ? firstSpeed : _speed[v + 1];

                float gap = leaderPos - _position[v];
                if (gap <= 0f)
                {
                    gap += length;
                }

                gap = MathF.Max(gap - p.VehicleLength, 0.01f);
                float speed = _speed[v];
                float dv = speed - leaderSpeed;
                float desiredGap = p.MinimumGap + MathF.Max(0f, speed * p.TimeHeadway + speed * dv / (2f * sqrtAb));
                float ratio = speed / p.DesiredSpeed;
                float accel = p.MaxAcceleration * (1f - ratio * ratio * ratio * ratio - (desiredGap / gap) * (desiredGap / gap));

                if (_random.NextFloat() < _perturbationChance)
                {
                    accel -= p.ComfortableDeceleration * 2f; // driver distraction -> phantom jam seed
                }

                float newSpeed = MathF.Max(0f, speed + accel * dt);
                float advance = MathF.Min((speed + newSpeed) * 0.5f * dt, MathF.Max(0f, gap - 0.01f));

                float pos = _position[v] + advance;
                if (pos >= length)
                {
                    pos -= length;
                }

                _position[v] = pos;
                _speed[v] = advance <= 0f ? 0f : newSpeed;
            }
        }

        Tick++;
    }

    /// <summary>Order-dependent checksum of the full state, used by determinism tests.</summary>
    public ulong StateHash()
    {
        ulong hash = 1469598103934665603UL;
        for (int i = 0; i < _position.Length; i++)
        {
            hash = (hash ^ BitConverter.SingleToUInt32Bits(_position[i])) * 1099511628211UL;
            hash = (hash ^ BitConverter.SingleToUInt32Bits(_speed[i])) * 1099511628211UL;
        }

        return hash;
    }
}
