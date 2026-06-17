namespace PetriSeed.Environment;

public sealed class NutrientFieldConfig
{
    public int Width { get; init; } = 72;

    public int Height { get; init; } = 72;

    public float CellSizePixels { get; init; } = 8.0f;

    public float StartingNutrientAmount { get; init; } = 0.0f;

    public float MaximumNutrientAmount { get; init; } = 1.0f;

    public float BaseRegenerationRate { get; init; } = 0.0008f;

    public float BaseDecayRate { get; init; } = 0.0005f;

    public float DiffusionRate { get; init; } = 0.22f;

    public int InitialPatchCount { get; init; } = 7;

    public int InitialPatchRadius { get; init; } = 6;

    public float InitialPatchAmount { get; init; } = 1.0f;

    public int TelemetryIntervalTicks { get; init; } = 20;
}
