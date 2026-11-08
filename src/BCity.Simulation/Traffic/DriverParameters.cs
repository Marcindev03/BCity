namespace BCity.Simulation.Traffic;

/// <summary>Car-following parameters (Intelligent Driver Model).</summary>
public readonly record struct DriverParameters(
    float DesiredSpeed,
    float MaxAcceleration,
    float ComfortableDeceleration,
    float MinimumGap,
    float TimeHeadway,
    float VehicleLength)
{
    public static DriverParameters CityCar { get; } = new(
        DesiredSpeed: 13.9f,          // 50 km/h
        MaxAcceleration: 1.5f,
        ComfortableDeceleration: 2.0f,
        MinimumGap: 2.0f,
        TimeHeadway: 1.4f,
        VehicleLength: 4.5f);
}
