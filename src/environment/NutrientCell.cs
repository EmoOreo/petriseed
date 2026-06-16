namespace PetriSeed.Environment;

public sealed class NutrientCell
{
    public NutrientCell(float nutrientAmount, float regenerationRate, float decayRate)
    {
        this.nutrientAmount = nutrientAmount;
        this.regenerationRate = regenerationRate;
        this.decayRate = decayRate;
    }

    public float nutrientAmount;

    public float regenerationRate;

    public float decayRate;
}
