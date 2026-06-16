using Godot;

namespace PetriSeed.Environment;

public partial class NutrientVisualization : Node2D
{
    private NutrientField? _field;
    private NutrientFieldConfig? _config;

    [Export]
    public bool UseHeatmap { get; set; } = true;

    public void Initialize(NutrientField field, NutrientFieldConfig config)
    {
        _field = field;
        _config = config;
        QueueRedraw();
    }

    public void OnSimulationTick()
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_field is null || _config is null)
        {
            return;
        }

        var cellSize = new Vector2(_config.CellSizePixels, _config.CellSizePixels);
        var origin = new Vector2(
            -(_field.Width * _config.CellSizePixels) * 0.5f,
            -(_field.Height * _config.CellSizePixels) * 0.5f);

        for (var y = 0; y < _field.Height; y++)
        {
            for (var x = 0; x < _field.Width; x++)
            {
                var amount = _field.GetNutrientAmount(x, y);
                var density = Mathf.Clamp(amount / _field.MaximumNutrientAmount, 0.0f, 1.0f);
                var position = origin + new Vector2(x * _config.CellSizePixels, y * _config.CellSizePixels);

                DrawRect(new Rect2(position, cellSize), GetColorForDensity(density));
            }
        }
    }

    private Color GetColorForDensity(float density)
    {
        if (!UseHeatmap)
        {
            return new Color(density, density, density, 1.0f);
        }

        var low = new Color(0.04f, 0.06f, 0.05f, 1.0f);
        var mid = new Color(0.18f, 0.55f, 0.28f, 1.0f);
        var high = new Color(0.95f, 0.9f, 0.35f, 1.0f);

        return density < 0.5f
            ? low.Lerp(mid, density * 2.0f)
            : mid.Lerp(high, (density - 0.5f) * 2.0f);
    }
}
