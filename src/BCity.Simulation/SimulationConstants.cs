namespace BCity.Simulation;

public static class SimulationConstants
{
    /// <summary>Fixed simulation tick rate (Hz). Rendering interpolates between ticks.</summary>
    public const int TicksPerSecond = 20;

    public const float TickSeconds = 1f / TicksPerSecond;
}
