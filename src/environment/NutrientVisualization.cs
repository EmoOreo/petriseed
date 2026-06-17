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
        ZIndex = 10;
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

        DrawDishBackdrop(_field, _config);
        DrawNutrients(_field, _config);
        DrawDishRim(_field, _config);
    }

    private void DrawDishBackdrop(NutrientField field, NutrientFieldConfig config)
    {
        var radius = GetDishRadius(field, config);

        DrawCircle(Vector2.Zero, radius + 12.0f, new Color(0.78f, 0.95f, 1.0f, 0.18f));
        DrawCircle(Vector2.Zero, radius + 4.0f, new Color(0.02f, 0.035f, 0.05f, 1.0f));
    }

    private void DrawNutrients(NutrientField field, NutrientFieldConfig config)
    {
        var cellSize = new Vector2(config.CellSizePixels, config.CellSizePixels);
        var origin = new Vector2(
            -(field.Width * config.CellSizePixels) * 0.5f,
            -(field.Height * config.CellSizePixels) * 0.5f);

        for (var y = 0; y < field.Height; y++)
        {
            for (var x = 0; x < field.Width; x++)
            {
                if (!field.IsInsideDish(x, y))
                {
                    continue;
                }

                var amount = field.GetNutrientAmount(x, y);
                var density = Mathf.Clamp(amount / field.MaximumNutrientAmount, 0.0f, 1.0f);
                var position = origin + new Vector2(x * config.CellSizePixels, y * config.CellSizePixels);

                DrawRect(new Rect2(position, cellSize), GetColorForDensity(density));
            }
        }
    }

    private void DrawDishRim(NutrientField field, NutrientFieldConfig config)
    {
        var radius = GetDishRadius(field, config);

        DrawArc(Vector2.Zero, radius + 4.0f, 0.0f, Mathf.Tau, 160, new Color(0.75f, 0.95f, 1.0f, 0.8f), 5.0f);
        DrawArc(Vector2.Zero, radius - 3.0f, 0.0f, Mathf.Tau, 160, new Color(0.18f, 0.4f, 0.8f, 0.45f), 2.0f);
    }

    private float GetDishRadius(NutrientField field, NutrientFieldConfig config)
    {
        return Mathf.Min(field.Width, field.Height) * config.CellSizePixels * 0.5f;
    }

    private Color GetColorForDensity(float density)
    {
        if (!UseHeatmap)
        {
            return new Color(density, density, density, 1.0f);
        }

        var low = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        var mid = new Color(0.0f, 0.25f, 1.0f, 1.0f);
        var high = new Color(1.0f, 0.95f, 0.2f, 1.0f);
        var peak = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        if (density < 0.5f)
        {
            return low.Lerp(mid, density * 2.0f);
        }

        if (density < 0.85f)
        {
            return mid.Lerp(high, (density - 0.5f) / 0.35f);
        }

        return high.Lerp(peak, (density - 0.85f) / 0.15f);
    }
}
