namespace PetriSeed.Organisms;

public sealed class PassiveMicrobeConfig
{
    public int InitialMicrobeCount { get; init; } = 32;

    public float SpawnRadiusFraction { get; init; } = 0.82f;

    public float RadiusPixels { get; init; } = 4.0f;

    public float StartingEnergy { get; init; } = 0.42f;

    public float MaximumEnergy { get; init; } = 1.0f;

    public float StarvationEnergyDrain { get; init; } = 0.0035f;

    public float NutrientIntakePerTick { get; init; } = 0.018f;

    public float NutrientEnergyEfficiency { get; init; } = 0.75f;

    public float MaximumSpeedPixelsPerTick { get; init; } = 1.9f;

    public float WanderStrength { get; init; } = 0.34f;

    public float NutrientSeekStrength { get; init; } = 0.9f;

    public float SteeringSmoothing { get; init; } = 0.2f;

    public int SenseRadiusCells { get; init; } = 5;
}
