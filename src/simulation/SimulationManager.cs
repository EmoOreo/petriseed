using Godot;

namespace PetriSeed.Simulation;

public partial class SimulationManager : Node2D
{
    private const double DefaultTicksPerSecond = 20.0d;
    private const ulong DefaultSeed = 1_337UL;

    private double _configuredTicksPerSecond = DefaultTicksPerSecond;
    private ulong _configuredSeed = DefaultSeed;
    private FixedTickLoop _tickLoop = new(DefaultTicksPerSecond);
    private RandomNumberService _random = new(DefaultSeed);

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

        GD.Print($"PetriSeed Phase 0 ready. Seed={_random.Seed}, TickRate={_tickLoop.TicksPerSecond:0.##}/s");
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
        if (_tickLoop.TickCount % 20UL == 0UL)
        {
            GD.Print($"Simulation tick {_tickLoop.TickCount}; rng-sample={_random.NextFloat():0.000}");
        }
    }
}
