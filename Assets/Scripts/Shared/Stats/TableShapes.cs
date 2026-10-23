using System.Collections.Generic;
using System.Linq;

// Every shape of holes on the base table grid, drawn on the grid (they depend on the order of the suit rows): rows
// are the suits in Seme order, columns the values, so slot = row * SlotsPerRow + value - 1. Shapes being studied for
// the tribuna, see wiki business/modifica-del-mazzo.md (decision in analysis).
public static class TableShapes
{
    static readonly int[] SquareSizes = { 2, 3, 4 };

    // Built once, never changed.
    public static readonly IReadOnlyList<TableShape> All = Build();

    public static int Slot(int row, int column) => row * DeckRules.SlotsPerRow + column;

    // Value of the card that belongs to a base slot (1 = ace ... 10 = king).
    public static int ValueOf(int slot) => slot % DeckRules.SlotsPerRow + 1;

    // Shapes whose slots are all holes, each with the path after which it was complete for good: the last of the
    // paths that made its slots holes. holeSince[slot]: that path for a hole (0 = a hole since the deal), -1 for a
    // slot that is not a hole.
    public static List<(TableShape shape, int path)> Complete(IReadOnlyList<int> holeSince) =>
        All.Where(shape => shape.Slots.All(slot => holeSince[slot] >= 0))
            .Select(shape => (shape, shape.Slots.Max(slot => holeSince[slot])))
            .ToList();

    static List<TableShape> Build()
    {
        List<TableShape> shapes = new();
        for (int column = 0; column < DeckRules.SlotsPerRow; column++)
        {
            shapes.Add(
                new TableShape(
                    ShapeKind.Column,
                    $"column {column + 1}",
                    Enumerable
                        .Range(0, DeckRules.BaseRows)
                        .Select(row => Slot(row, column))
                        .ToList()
                )
            );
        }
        for (int column = 0; column + DeckRules.BaseRows <= DeckRules.SlotsPerRow; column++)
        {
            int start = column;
            shapes.Add(
                new TableShape(
                    ShapeKind.Diagonal,
                    $"diagonal {start + 1} down",
                    Enumerable
                        .Range(0, DeckRules.BaseRows)
                        .Select(step => Slot(step, start + step))
                        .ToList()
                )
            );
            shapes.Add(
                new TableShape(
                    ShapeKind.Diagonal,
                    $"diagonal {start + 1} up",
                    Enumerable
                        .Range(0, DeckRules.BaseRows)
                        .Select(step => Slot(DeckRules.BaseRows - 1 - step, start + step))
                        .ToList()
                )
            );
        }
        foreach (int size in SquareSizes)
        {
            ShapeKind kind = ShapeKind.Square2 + (size - SquareSizes[0]);
            for (int row = 0; row + size <= DeckRules.BaseRows; row++)
            {
                for (int column = 0; column + size <= DeckRules.SlotsPerRow; column++)
                {
                    List<int> slots = new(size * size);
                    for (int r = row; r < row + size; r++)
                    {
                        for (int c = column; c < column + size; c++)
                        {
                            slots.Add(Slot(r, c));
                        }
                    }
                    shapes.Add(
                        new TableShape(
                            kind,
                            $"{kind.ToString().ToLowerInvariant()} {(Seme)row} {column + 1}",
                            slots
                        )
                    );
                }
            }
        }
        return shapes;
    }
}
