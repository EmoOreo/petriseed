using Godot;
using PetriSeed.Environment;
using PetriSeed.Simulation;

namespace PetriSeed.Organisms;

public sealed class PassiveMicrobeSimulation
{
    private readonly List<PassiveMicrobe> _microbes = [];
    private readonly PassiveMicrobeConfig _config;

    public PassiveMicrobeSimulation(PassiveMicrobeConfig config)
    {
        _config = config;
    }

    public IReadOnlyList<PassiveMicrobe> Microbes => _microbes;

    public int AliveCount { get; private set; }

    public int DeathCount { get; private set; }

    public float AverageEnergy { get; private set; }

    public void SpawnInitial(NutrientField field, RandomNumberService random)
    {
        _microbes.Clear();
        DeathCount = 0;

        for (var index = 0; index < _config.InitialMicrobeCount; index++)
        {
            var position = GetSpawnPosition(field, random);
            var velocity = Vector2.FromAngle(random.Range(0.0f, Mathf.Tau)) * random.Range(0.1f, 0.6f);
            var energy = random.Range(_config.StartingEnergy * 0.8f, _config.StartingEnergy * 1.2f);

            _microbes.Add(new PassiveMicrobe(index + 1, position, velocity, energy));
        }

        UpdateTelemetry();
    }

    public void Tick(NutrientField field, RandomNumberService random)
    {
        var deathsBeforeTick = DeathCount;

        foreach (var microbe in _microbes)
        {
            var wasAlive = microbe.IsAlive;
            microbe.Tick(field, _config, random);

            if (wasAlive && !microbe.IsAlive)
            {
                DeathCount++;
            }
        }

        if (DeathCount != deathsBeforeTick)
        {
            _microbes.RemoveAll(static microbe => !microbe.IsAlive);
        }

        UpdateTelemetry();
    }

    private Vector2 GetSpawnPosition(NutrientField field, RandomNumberService random)
    {
        var spawnRadius = field.DishRadiusWorld * _config.SpawnRadiusFraction;

        for (var attempt = 0; attempt < 128; attempt++)
        {
            var angle = random.Range(0.0f, Mathf.Tau);
            var radius = Mathf.Sqrt(random.NextFloat()) * spawnRadius;
            var position = field.DishCenterWorld + (Vector2.FromAngle(angle) * radius);

            if (field.TryGetCellAtWorld(position, out _, out _))
            {
                return position;
            }
        }

        return field.DishCenterWorld;
    }

    private void UpdateTelemetry()
    {
        AliveCount = 0;
        var totalEnergy = 0.0f;

        foreach (var microbe in _microbes)
        {
            if (!microbe.IsAlive)
            {
                continue;
            }

            AliveCount++;
            totalEnergy += microbe.Energy;
        }

        AverageEnergy = AliveCount == 0 ? 0.0f : totalEnergy / AliveCount;
    }
}
