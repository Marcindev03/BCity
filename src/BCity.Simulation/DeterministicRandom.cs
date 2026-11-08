namespace BCity.Simulation;

/// <summary>Deterministic xorshift32 RNG. Never use System.Random in simulation code.</summary>
public struct DeterministicRandom
{
    private uint _state;

    public DeterministicRandom(uint seed) => _state = seed == 0 ? 0x9E3779B9u : seed;

    public uint NextUInt()
    {
        uint x = _state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        _state = x;
        return x;
    }

    /// <summary>Uniform float in [0, 1).</summary>
    public float NextFloat() => (NextUInt() >> 8) * (1f / 16777216f);
}
