using System.Collections.Generic;

// One path of a simulated game, as GameSimulator played it.
public class SimulatedPath
{
    public int Number { get; }
    public int StartSlot { get; }

    // Cards moved by the path, in order; a bench card filling the starting slot is not one of them.
    public IReadOnlyList<CardDataInGame> Cards { get; }
    public PathEnd End { get; }

    // A bench card went face down into the starting slot after the path.
    public bool FilledFromBench { get; }

    // Kings face up on the table after the path.
    public int FaceUpKings { get; }

    public SimulatedPath(
        int number,
        int startSlot,
        IReadOnlyList<CardDataInGame> cards,
        PathEnd end,
        bool filledFromBench,
        int faceUpKings
    )
    {
        Number = number;
        StartSlot = startSlot;
        Cards = cards;
        End = end;
        FilledFromBench = filledFromBench;
        FaceUpKings = faceUpKings;
    }

    public int Length => Cards.Count;
}
