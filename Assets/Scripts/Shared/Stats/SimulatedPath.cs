using System.Collections.Generic;

// One path of a simulated game, as GameSimulator played it.
public class SimulatedPath
{
    public int Number { get; }

    // Cards moved by the path, in order; a bench card filling the starting slot is not one of them.
    public IReadOnlyList<CardDataInGame> Cards { get; }
    public PathEnd End { get; }

    // A bench card went face down into the starting slot after the path.
    public bool FilledFromBench { get; }

    // Kings face up on the table after the path.
    public int FaceUpKings { get; }

    // Holes on the table after the path.
    public int Holes { get; }

    public SimulatedPath(
        int number,
        IReadOnlyList<CardDataInGame> cards,
        PathEnd end,
        bool filledFromBench,
        int faceUpKings,
        int holes
    )
    {
        Number = number;
        Cards = cards;
        End = end;
        FilledFromBench = filledFromBench;
        FaceUpKings = faceUpKings;
        Holes = holes;
    }

    public int Length => Cards.Count;
}
