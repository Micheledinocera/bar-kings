using System.Collections.Generic;

// One shape at one place of the base table grid: the slots that must all be holes.
public class TableShape
{
    public ShapeKind Kind { get; }

    // Kind and place, e.g. "column 3", "diagonal 1 down", "square2 Coppe 4" (top-left cell).
    public string Name { get; }

    public IReadOnlyList<int> Slots { get; }

    public TableShape(ShapeKind kind, string name, IReadOnlyList<int> slots)
    {
        Kind = kind;
        Name = name;
        Slots = slots;
    }
}
