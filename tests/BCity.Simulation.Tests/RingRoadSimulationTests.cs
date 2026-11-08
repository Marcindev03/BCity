using BCity.Simulation.Traffic;
using Xunit;

namespace BCity.Simulation.Tests.Traffic;

public class RingRoadSimulationTests
{
    private static RingRoadSimulation Create(uint seed = 42) =>
        new(rings: 4, vehiclesPerRing: 120, innerRadius: 150f, ringSpacing: 6f, seed: seed);

    [Fact]
    public void SameSeedProducesIdenticalState()
    {
        var a = Create();
        var b = Create();
        for (int i = 0; i < 2_000; i++)
        {
            a.Step();
            b.Step();
        }

        Assert.Equal(a.StateHash(), b.StateHash());
    }

    [Fact]
    public void DifferentSeedProducesDifferentState()
    {
        var a = Create(1);
        var b = Create(2);
        for (int i = 0; i < 2_000; i++)
        {
            a.Step();
            b.Step();
        }

        Assert.NotEqual(a.StateHash(), b.StateHash());
    }

    [Fact]
    public void VehiclesNeverCollideAndRespectSpeedLimits()
    {
        var sim = Create();
        for (int t = 0; t < 6_000; t++)
        {
            sim.Step();
            for (int v = 0; v < sim.VehicleCount; v++)
            {
                Assert.True(sim.GapToLeader(v) >= 0f, $"Collision: vehicle {v} at tick {sim.Tick}");
                Assert.InRange(sim.Speeds[v], 0f, sim.Driver.DesiredSpeed * 1.05f);
            }
        }
    }

    [Fact]
    public void TrafficFlowsOnAverage()
    {
        var sim = Create();
        for (int t = 0; t < 2_400; t++)
        {
            sim.Step();
        }

        float sum = 0f;
        foreach (float s in sim.Speeds)
        {
            sum += s;
        }

        Assert.True(sum / sim.VehicleCount > 1f, "Traffic should not be in total gridlock.");
    }

    [Fact]
    public void StepDoesNotAllocate()
    {
        var sim = Create();
        sim.Step();
        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int t = 0; t < 1_000; t++)
        {
            sim.Step();
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }
}
