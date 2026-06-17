using Godot;
using PetriSeed.Environment;
using PetriSeed.Organisms;

namespace PetriSeed.Simulation;

public partial class SimulationManager : Node2D
{
    private const double DefaultTicksPerSecond = 20.0d;
    private const ulong DefaultSeed = 1_337UL;

    private double _configuredTicksPerSecond = DefaultTicksPerSecond;
    private ulong _configuredSeed = DefaultSeed;
    private FixedTickLoop _tickLoop = new(DefaultTicksPerSecond);
    private RandomNumberService _random = new(DefaultSeed);
    private NutrientFieldConfig _nutrientFieldConfig = new();
    private NutrientField? _nutrientField;
    private NutrientVisualization? _nutrientVisualization;
    private Label? _nutrientStatsLabel;
    private PassiveMicrobeConfig _passiveMicrobeConfig = new();
    private PassiveMicrobeSimulation? _passiveMicrobeSimulation;
    private PassiveMicrobeVisualization? _passiveMicrobeVisualization;

    [Export]
    public bool IsPaused { get; set; }

    [Export(PropertyHint.Range, "0.25,8.0,0.25")]
    public double SpeedMultiplier { get; set; } = 1.0d;

    [Export(PropertyHint.Range, "1,240,1")]
    public double TicksPerSecond
    {
        get => _configuredTicksPerSecond;
        set
        {
            _configuredTicksPerSecond = value > 0.0d ? value : DefaultTicksPerSecond;
            _tickLoop.SetTicksPerSecond(_configuredTicksPerSecond);
        }
    }

    [Export]
    public ulong Seed
    {
        get => _configuredSeed;
        set
        {
            _configuredSeed = value == 0UL ? DefaultSeed : value;
            _random.Reseed(_configuredSeed);
        }
    }

    [Export(PropertyHint.Range, "1,100000,1")]
    public int PopulationCap { get; set; } = 2_000;

    public override void _Ready()
    {
        _tickLoop = new FixedTickLoop(_configuredTicksPerSecond);
        _random = new RandomNumberService(_configuredSeed);
        _nutrientFieldConfig = new NutrientFieldConfig();
        _nutrientField = new NutrientField(_nutrientFieldConfig, _random);
        _nutrientVisualization = GetNodeOrNull<NutrientVisualization>("NutrientVisualization");
        _nutrientVisualization?.Initialize(_nutrientField, _nutrientFieldConfig);
        _nutrientStatsLabel = GetNodeOrNull<Label>("DebugOverlay/NutrientStatsLabel");
        _passiveMicrobeConfig = new PassiveMicrobeConfig();
        _passiveMicrobeSimulation = new PassiveMicrobeSimulation(_passiveMicrobeConfig);
        _passiveMicrobeSimulation.SpawnInitial(_nutrientField, _random);
        _passiveMicrobeVisualization = GetNodeOrNull<PassiveMicrobeVisualization>("PassiveMicrobeVisualization");
        _passiveMicrobeVisualization?.Initialize(_passiveMicrobeSimulation, _passiveMicrobeConfig);

        GD.Print($"PetriSeed Phase 1B ready. Seed={_random.Seed}, TickRate={_tickLoop.TicksPerSecond:0.##}/s");
        PrintNutrientTelemetry();
    }

    public override void _Process(double delta)
    {
        if (IsPaused)
        {
            return;
        }

        var ticksReady = _tickLoop.Accumulate(delta, SpeedMultiplier);
        for (var tickIndex = 0; tickIndex < ticksReady; tickIndex++)
        {
            RunSimulationTick();
        }
    }

    private void RunSimulationTick()
    {
        _nutrientField?.Tick();
        if (_nutrientField is not null)
        {
            _passiveMicrobeSimulation?.Tick(_nutrientField, _random);
        }

        _nutrientVisualization?.OnSimulationTick();
        _passiveMicrobeVisualization?.OnSimulationTick();

        if (_tickLoop.TickCount % (ulong)_nutrientFieldConfig.TelemetryIntervalTicks == 0UL)
        {
            PrintNutrientTelemetry();
        }
    }

    private void PrintNutrientTelemetry()
    {
        if (_nutrientField is null)
        {
            return;
        }

        var aliveCount = _passiveMicrobeSimulation?.AliveCount ?? 0;
        var averageEnergy = _passiveMicrobeSimulation?.AverageEnergy ?? 0.0f;
        var deathCount = _passiveMicrobeSimulation?.DeathCount ?? 0;
        var logMessage =
            $"Tick {_tickLoop.TickCount}: total nutrients={_nutrientField.TotalNutrients:0.000}, " +
            $"average={_nutrientField.AverageNutrients:0.000}, " +
            $"diffusion delta={_nutrientField.DiffusionDeltaLastTick:0.000}, " +
            $"microbes alive={aliveCount}, average energy={averageEnergy:0.000}, deaths={deathCount}";

        GD.Print(logMessage);

        if (_nutrientStatsLabel is not null)
        {
            _nutrientStatsLabel.Text =
                $"Tick Count: {_tickLoop.TickCount}\n" +
                $"Total Nutrients: {_nutrientField.TotalNutrients:0.000}\n" +
                $"Average Nutrients: {_nutrientField.AverageNutrients:0.000}\n" +
                $"Diffusion Delta: {_nutrientField.DiffusionDeltaLastTick:0.000}\n" +
                $"Microbes Alive: {aliveCount}\n" +
                $"Average Energy: {averageEnergy:0.000}\n" +
                $"Starvation Deaths: {deathCount}";
        }
    }
}
