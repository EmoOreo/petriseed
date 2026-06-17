using Godot;
using PetriSeed.Environment;
using PetriSeed.Simulation;

namespace PetriSeed.Organisms;

public sealed class PassiveMicrobe
{
    public PassiveMicrobe(int id, Vector2 position, Vector2 velocity, float energy)
    {
        Id = id;
        Position = position;
        Velocity = velocity;
        Energy = energy;
    }

    public int Id { get; }

    public Vector2 Position { get; private set; }

    public Vector2 Velocity { get; private set; }

    public float Energy { get; private set; }

    public bool IsAlive { get; private set; } = true;

    public void Tick(NutrientField field, PassiveMicrobeConfig config, RandomNumberService random)
    {
        if (!IsAlive)
        {
            return;
        }

        var nutrientDirection = SenseNutrientDirection(field, config);
        var wanderDirection = GetWanderDirection(random);
        var desiredDirection = (nutrientDirection * config.NutrientSeekStrength) +
                               (wanderDirection * config.WanderStrength);

        if (desiredDirection.LengthSquared() > 0.0001f)
        {
            desiredDirection = desiredDirection.Normalized();
        }

        var desiredVelocity = desiredDirection * config.MaximumSpeedPixelsPerTick;
        Velocity = Velocity.Lerp(desiredVelocity, config.SteeringSmoothing);

        var nextPosition = Position + Velocity;
        if (!field.TryGetCellAtWorld(nextPosition, out _, out _))
        {
            nextPosition = KeepInsideDish(nextPosition, field);
            Velocity = Velocity.Bounce((field.DishCenterWorld - Position).Normalized()) * 0.35f;
        }

        Position = nextPosition;

        var consumedNutrients = field.ConsumeNutrientsAtWorld(Position, config.NutrientIntakePerTick);
        Energy = Mathf.Clamp(
            Energy + (consumedNutrients * config.NutrientEnergyEfficiency) - config.StarvationEnergyDrain,
            0.0f,
            config.MaximumEnergy);

        if (Energy <= 0.0f)
        {
            IsAlive = false;
            Velocity = Vector2.Zero;
        }
    }

    private Vector2 SenseNutrientDirection(NutrientField field, PassiveMicrobeConfig config)
    {
        if (!field.TryGetCellAtWorld(Position, out var currentX, out var currentY))
        {
            return (field.DishCenterWorld - Position).Normalized();
        }

        var currentAmount = field.GetNutrientAmount(currentX, currentY);
        var bestAmount = currentAmount;
        var bestCell = new Vector2I(currentX, currentY);

        for (var y = currentY - config.SenseRadiusCells; y <= currentY + config.SenseRadiusCells; y++)
        {
            for (var x = currentX - config.SenseRadiusCells; x <= currentX + config.SenseRadiusCells; x++)
            {
                var amount = field.GetNutrientAmount(x, y);
                if (amount <= bestAmount)
                {
                    continue;
                }

                bestAmount = amount;
                bestCell = new Vector2I(x, y);
            }
        }

        if (bestCell.X == currentX && bestCell.Y == currentY)
        {
            return Vector2.Zero;
        }

        return (field.GetCellCenterWorld(bestCell.X, bestCell.Y) - Position).Normalized();
    }

    private static Vector2 GetWanderDirection(RandomNumberService random)
    {
        var angle = random.Range(0.0f, Mathf.Tau);

        return Vector2.FromAngle(angle);
    }

    private Vector2 KeepInsideDish(Vector2 nextPosition, NutrientField field)
    {
        var offset = nextPosition - field.DishCenterWorld;
        if (offset.LengthSquared() <= 0.0001f)
        {
            return field.DishCenterWorld;
        }

        return field.DishCenterWorld + (offset.Normalized() * field.DishRadiusWorld);
    }
}
