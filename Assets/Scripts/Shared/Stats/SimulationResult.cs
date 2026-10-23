using System.Collections.Generic;

// The games of one preset with one autoplay strategy (TableSimulationRunner).
public class SimulationResult
{
    public DeckPreset Preset { get; }
    public AutoplayStrategy Strategy { get; }
    public IReadOnlyList<SimulatedGame> Games { get; }

    public SimulationResult(
        DeckPreset preset,
        AutoplayStrategy strategy,
        IReadOnlyList<SimulatedGame> games
    )
    {
        Preset = preset;
        Strategy = strategy;
        Games = games;
    }
}
