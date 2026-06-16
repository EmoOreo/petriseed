namespace PetriSeed.Environment;

public sealed class NutrientFieldConfig
{
    public int Width { get; init; } = 64;

    public int Height { get; init; } = 36;

    public float CellSizePixels { get; init; } = 10.0f;

    public float StartingNutrientAmount { get; init; } = 0.12f;

    public float MaximumNutrientAmount { get; init; } = 1.0f;

    public float BaseRegenerationRate { get; init; } = 0.0025f;

    public float BaseDecayRate { get; init; } = 0.0012f;

    public float DiffusionRate { get; init; } = 0.18f;

    public int InitialPatchCount { get; init; } = 5;

    public int InitialPatchRadius { get; init; } = 4;

    public float InitialPatchAmount { get; init; } = 0.75f;

    public int TelemetryIntervalTicks { get; init; } = 20;
}
