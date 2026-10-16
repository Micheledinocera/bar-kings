using System.Collections.Generic;

// A game played to the end by GameSimulator: its deck and its paths in order.
public class SimulatedGame
{
    // Seed of the deck: set as TableDeckManager seed, the scene deals the same table.
    public int Seed { get; }
    public DeckDeal Deal { get; }
    public IReadOnlyList<SimulatedPath> Paths { get; }

    // Path after which all the kings are face up; 0 if it never happens.
    public int AllKingsPath { get; }

    public SimulatedGame(
        int seed,
        DeckDeal deal,
        IReadOnlyList<SimulatedPath> paths,
        int allKingsPath
    )
    {
        Seed = seed;
        Deal = deal;
        Paths = paths;
        AllKingsPath = allKingsPath;
    }
}
