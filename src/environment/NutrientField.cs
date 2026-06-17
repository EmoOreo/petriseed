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
            var x = index % _config.Width;
            var y = index / _config.Width;
            var isInsideDish = IsInsideDish(x, y);

            _cells[index] = new NutrientCell(
                isInsideDish ? _config.StartingNutrientAmount : 0.0f,
                isInsideDish ? _config.BaseRegenerationRate : 0.0f,
                isInsideDish ? _config.BaseDecayRate : 0.0f);
        }

        AddSeededNutrientPatches(random);
    }

    public int Width => _config.Width;

    public int Height => _config.Height;

    public float MaximumNutrientAmount => _config.MaximumNutrientAmount;

    public float TotalNutrients { get; private set; }

    public float AverageNutrients { get; private set; }

    public float DiffusionDeltaLastTick { get; private set; }

    public Vector2 DishCenterWorld => Vector2.Zero;

    public float DishRadiusWorld => (Mathf.Min(_config.Width, _config.Height) * _config.CellSizePixels * 0.5f) - _config.CellSizePixels;

    public bool IsInsideDish(int x, int y)
    {
        if (!Contains(x, y))
        {
            return false;
        }

        var centerX = (_config.Width - 1) * 0.5f;
        var centerY = (_config.Height - 1) * 0.5f;
        var radius = (Mathf.Min(_config.Width, _config.Height) * 0.5f) - 1.0f;
        var dx = x - centerX;
        var dy = y - centerY;

        return ((dx * dx) + (dy * dy)) <= radius * radius;
    }

    public NutrientCell GetCell(int x, int y)
    {
        return _cells[ToIndex(x, y)];
    }

    public float GetNutrientAmount(int x, int y)
    {
        if (!IsInsideDish(x, y))
        {
            return 0.0f;
        }

        return GetCell(x, y).nutrientAmount;
    }

    public Vector2 GetCellCenterWorld(int x, int y)
    {
        var origin = GetWorldOrigin();

        return origin + new Vector2(
            (x + 0.5f) * _config.CellSizePixels,
            (y + 0.5f) * _config.CellSizePixels);
    }

    public bool TryGetCellAtWorld(Vector2 worldPosition, out int cellX, out int cellY)
    {
        var localPosition = worldPosition - GetWorldOrigin();

        cellX = Mathf.FloorToInt(localPosition.X / _config.CellSizePixels);
        cellY = Mathf.FloorToInt(localPosition.Y / _config.CellSizePixels);

        return IsInsideDish(cellX, cellY);
    }

    public float ConsumeNutrientsAtWorld(Vector2 worldPosition, float requestedAmount)
    {
        if (requestedAmount <= 0.0f || !TryGetCellAtWorld(worldPosition, out var cellX, out var cellY))
        {
            return 0.0f;
        }

        var cell = GetCell(cellX, cellY);
        var consumedAmount = Mathf.Min(cell.nutrientAmount, requestedAmount);
        cell.nutrientAmount -= consumedAmount;

        UpdateTelemetry();

        return consumedAmount;
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
                if (!IsInsideDish(x, y))
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
        AddNutrients(_config.Width / 2, _config.Height / 2, _config.InitialPatchRadius + 1, _config.InitialPatchAmount);

        for (var patchIndex = 0; patchIndex < _config.InitialPatchCount; patchIndex++)
        {
            var centerX = _config.Width / 2;
            var centerY = _config.Height / 2;

            for (var attempt = 0; attempt < 64; attempt++)
            {
                var candidateX = random.Range(_config.InitialPatchRadius, _config.Width - _config.InitialPatchRadius);
                var candidateY = random.Range(_config.InitialPatchRadius, _config.Height - _config.InitialPatchRadius);

                if (IsInsideDish(candidateX, candidateY))
                {
                    centerX = candidateX;
                    centerY = candidateY;
                    break;
                }
            }

            AddNutrients(centerX, centerY, _config.InitialPatchRadius, _config.InitialPatchAmount);
        }
    }

    private void ApplyRegenerationAndDecay()
    {
        for (var index = 0; index < _cells.Length; index++)
        {
            var cell = _cells[index];
            if (!IsInsideDish(index % _config.Width, index / _config.Width))
            {
                _nextAmounts[index] = 0.0f;
                continue;
            }

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
        if (!IsInsideDish(firstIndex % _config.Width, firstIndex / _config.Width) ||
            !IsInsideDish(secondIndex % _config.Width, secondIndex / _config.Width))
        {
            return;
        }

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

    private Vector2 GetWorldOrigin()
    {
        return new Vector2(
            -(_config.Width * _config.CellSizePixels) * 0.5f,
            -(_config.Height * _config.CellSizePixels) * 0.5f);
    }

    private int ToIndex(int x, int y)
    {
        return x + (y * _config.Width);
    }
}
