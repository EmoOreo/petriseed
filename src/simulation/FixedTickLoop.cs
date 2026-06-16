namespace PetriSeed.Simulation;

public sealed class FixedTickLoop
{
    public FixedTickLoop(double ticksPerSecond)
    {
        TicksPerSecond = ticksPerSecond;
    }

    public double TicksPerSecond { get; private set; }

    public double TickDurationSeconds => 1.0d / TicksPerSecond;

    public double AccumulatedSeconds { get; private set; }

    public ulong TickCount { get; private set; }

    public void SetTicksPerSecond(double ticksPerSecond)
    {
        TicksPerSecond = ticksPerSecond > 0.0d ? ticksPerSecond : 1.0d;
    }

    public int Accumulate(double deltaSeconds, double speedMultiplier)
    {
        if (deltaSeconds <= 0.0d || speedMultiplier <= 0.0d)
        {
            return 0;
        }

        AccumulatedSeconds += deltaSeconds * speedMultiplier;

        var ticksReady = 0;
        while (AccumulatedSeconds >= TickDurationSeconds)
        {
            AccumulatedSeconds -= TickDurationSeconds;
            TickCount++;
            ticksReady++;
        }

        return ticksReady;
    }

    public void Reset()
    {
        AccumulatedSeconds = 0.0d;
        TickCount = 0;
    }
}
