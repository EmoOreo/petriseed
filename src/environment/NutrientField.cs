using Godot;
using PetriSeed.Simulation;

namespace PetriSeed.Environment;

public sealed class NutrientField
{
    private readonly NutrientFieldConfig _config;
    private readonly NutrientCell[] _cells;
    private readonly float[] _nextAmounts;

    public NutrientField(NutrientFieldConfig config, RandomNumberService random)
    {
        _config = config;
        _cells = new NutrientCell[_config.Width * _config.Height];
        _nextAmounts = new float[_cells.Length];

        for (var index = 0; index < _cells.Length; index++)
        {
            _cells[index] = new NutrientCell(
                _config.StartingNutrientAmount,
                _config.BaseRegenerationRate,
                _config.BaseDecayRate);
        }

        AddSeededNutrientPatches(random);
    }

    public int Width => _config.Width;

    public int Height => _config.Height;

    public float MaximumNutrientAmount => _config.MaximumNutrientAmount;

    public float TotalNutrients { get; private set; }

    public float AverageNutrients { get; private set; }

    public float DiffusionDeltaLastTick { get; private set; }

    public NutrientCell GetCell(int x, int y)
    {
        return _cells[ToIndex(x, y)];
    }

    public float GetNutrientAmount(int x, int y)
    {
        if (!Contains(x, y))
        {
            return 0.0f;
        }

        return GetCell(x, y).nutrientAmount;
    }

    public void Tick()
    {
        ApplyRegenerationAndDecay();
        ApplyDiffusion();
        UpdateTelemetry();
    }

    public void AddNutrients(int centerX, int centerY, int radius, float amount)
    {
        var radiusSquared = radius * radius;

        for (var y = centerY - radius; y <= centerY + radius; y++)
        {
            for (var x = centerX - radius; x <= centerX + radius; x++)
            {
                if (!Contains(x, y))
                {
                    continue;
                }

                var dx = x - centerX;
                var dy = y - centerY;
                if ((dx * dx) + (dy * dy) > radiusSquared)
                {
                    continue;
                }

                var cell = GetCell(x, y);
                var distance = Mathf.Sqrt((dx * dx) + (dy * dy));
                var falloff = 1.0f - Mathf.Clamp(distance / Mathf.Max(radius, 1), 0.0f, 1.0f);
                var weightedAmount = amount * Mathf.Lerp(0.35f, 1.0f, falloff);

                cell.nutrientAmount = Mathf.Clamp(
                    cell.nutrientAmount + weightedAmount,
                    0.0f,
                    _config.MaximumNutrientAmount);
            }
        }

        UpdateTelemetry();
    }

    private void AddSeededNutrientPatches(RandomNumberService random)
    {
        for (var patchIndex = 0; patchIndex < _config.InitialPatchCount; patchIndex++)
        {
            var centerX = random.Range(_config.InitialPatchRadius, _config.Width - _config.InitialPatchRadius);
            var centerY = random.Range(_config.InitialPatchRadius, _config.Height - _config.InitialPatchRadius);
            AddNutrients(centerX, centerY, _config.InitialPatchRadius, _config.InitialPatchAmount);
        }
    }

    private void ApplyRegenerationAndDecay()
    {
        for (var index = 0; index < _cells.Length; index++)
        {
            var cell = _cells[index];
            var decayed = cell.nutrientAmount * (1.0f - cell.decayRate);
            var regenerated = decayed + cell.regenerationRate;

            _nextAmounts[index] = Mathf.Clamp(regenerated, 0.0f, _config.MaximumNutrientAmount);
        }
    }

    private void ApplyDiffusion()
    {
        DiffusionDeltaLastTick = 0.0f;
        var diffusionRate = Mathf.Clamp(_config.DiffusionRate, 0.0f, 0.25f);

        for (var y = 0; y < _config.Height; y++)
        {
            for (var x = 0; x < _config.Width; x++)
            {
                var index = ToIndex(x, y);

                if (x + 1 < _config.Width)
                {
                    DiffusePair(index, ToIndex(x + 1, y), diffusionRate);
                }

                if (y + 1 < _config.Height)
                {
                    DiffusePair(index, ToIndex(x, y + 1), diffusionRate);
                }
            }
        }

        for (var index = 0; index < _cells.Length; index++)
        {
            _cells[index].nutrientAmount = Mathf.Clamp(_nextAmounts[index], 0.0f, _config.MaximumNutrientAmount);
        }
    }

    private void DiffusePair(int firstIndex, int secondIndex, float diffusionRate)
    {
        var difference = _nextAmounts[firstIndex] - _nextAmounts[secondIndex];
        var flow = difference * diffusionRate;

        _nextAmounts[firstIndex] -= flow;
        _nextAmounts[secondIndex] += flow;
        DiffusionDeltaLastTick += Mathf.Abs(flow);
    }

    private void UpdateTelemetry()
    {
        var total = 0.0f;

        for (var index = 0; index < _cells.Length; index++)
        {
            total += _cells[index].nutrientAmount;
        }

        TotalNutrients = total;
        AverageNutrients = _cells.Length == 0 ? 0.0f : total / _cells.Length;
    }

    private bool Contains(int x, int y)
    {
        return x >= 0 && y >= 0 && x < _config.Width && y < _config.Height;
    }

    private int ToIndex(int x, int y)
    {
        return x + (y * _config.Width);
    }
}
