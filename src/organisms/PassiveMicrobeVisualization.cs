using Godot;

namespace PetriSeed.Organisms;

public partial class PassiveMicrobeVisualization : Node2D
{
    private PassiveMicrobeSimulation? _simulation;
    private PassiveMicrobeConfig? _config;

    public void Initialize(PassiveMicrobeSimulation simulation, PassiveMicrobeConfig config)
    {
        _simulation = simulation;
        _config = config;
        ZIndex = 30;
        QueueRedraw();
    }

    public void OnSimulationTick()
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_simulation is null || _config is null)
        {
            return;
        }

        foreach (var microbe in _simulation.Microbes)
        {
            if (!microbe.IsAlive)
            {
                continue;
            }

            var energyRatio = Mathf.Clamp(microbe.Energy / _config.MaximumEnergy, 0.0f, 1.0f);
            var radius = Mathf.Lerp(_config.RadiusPixels * 0.7f, _config.RadiusPixels * 1.6f, energyRatio);
            var bodyColor = new Color(0.35f, 0.95f, 1.0f, 0.9f).Lerp(new Color(0.95f, 1.0f, 0.55f, 1.0f), energyRatio);

            DrawCircle(microbe.Position, radius + 2.0f, new Color(0.0f, 0.0f, 0.0f, 0.55f));
            DrawCircle(microbe.Position, radius, bodyColor);
            DrawArc(microbe.Position, radius + 1.0f, 0.0f, Mathf.Tau, 24, new Color(1.0f, 1.0f, 1.0f, 0.65f), 1.0f);
        }
    }
}
