using Godot;

namespace PetriSeed.Simulation;

public sealed class RandomNumberService
{
    private readonly RandomNumberGenerator _generator = new();

    public RandomNumberService(ulong seed)
    {
        Seed = seed;
        _generator.Seed = seed;
    }

    public ulong Seed { get; private set; }

    public void Reseed(ulong seed)
    {
        Seed = seed;
        _generator.Seed = seed;
    }

    public float NextFloat()
    {
        return _generator.Randf();
    }

    public float Range(float minimum, float maximum)
    {
        return _generator.RandfRange(minimum, maximum);
    }

    public int Range(int minimumInclusive, int maximumExclusive)
    {
        if (maximumExclusive <= minimumInclusive)
        {
            return minimumInclusive;
        }

        return _generator.RandiRange(minimumInclusive, maximumExclusive - 1);
    }

    public bool Chance(float probability)
    {
        return NextFloat() < Mathf.Clamp(probability, 0.0f, 1.0f);
    }
}
