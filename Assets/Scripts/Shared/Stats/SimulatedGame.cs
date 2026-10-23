using System.Collections.Generic;
using System.Linq;

// A game played to the end by GameSimulator: its deck, its paths in order and the holes left at the end.
public class SimulatedGame
{
    // Seed of the deck: set as TableDeckManager seed, the scene deals the same table.
    public int Seed { get; }
    public DeckDeal Deal { get; }
    public IReadOnlyList<SimulatedPath> Paths { get; }

    // Path after which all the kings are face up; 0 if it never happens.
    public int AllKingsPath { get; }

    // For each slot at the end: the path after which it became a hole for good (0: since the deal), -1 when it is
    // not a hole. Only the holes at the end count.
    public IReadOnlyList<int> HoleSince { get; }

    // Cards still face down at the end.
    public int FaceDownAtEnd { get; }

    // Shapes of holes at the end, each with the path after which it was complete (TableShapes.Complete).
    public IReadOnlyList<(TableShape shape, int path)> Shapes { get; }

    public SimulatedGame(
        int seed,
        DeckDeal deal,
        IReadOnlyList<SimulatedPath> paths,
        int allKingsPath,
        IReadOnlyList<int> holeSince,
        int faceDownAtEnd
    )
    {
        Seed = seed;
        Deal = deal;
        Paths = paths;
        AllKingsPath = allKingsPath;
        HoleSince = holeSince;
        FaceDownAtEnd = faceDownAtEnd;
        Shapes = TableShapes.Complete(holeSince);
    }

    public IEnumerable<int> HoleSlots =>
        Enumerable.Range(0, HoleSince.Count).Where(slot => HoleSince[slot] >= 0);
}
