using System.Collections.Generic;
using System.Linq;

// The tombola card of the Bar scene: the base 4x10 grid with its holes, and the shapes of holes already complete
// (TableShapes). Shapes being studied for the tribuna, wiki business/modifica-del-mazzo.md (decision in analysis).
public class TombolaCard
{
    public IReadOnlyList<TombolaCell> Cells { get; }

    public IReadOnlyList<TableShape> CompleteShapes { get; }

    public TombolaCard(TableState table)
    {
        // Slots beyond the base grid (index>40) are not on the card.
        int[] holeSince = new int[DeckRules.TableSlots];
        for (int slot = 0; slot < holeSince.Length; slot++)
        {
            holeSince[slot] = slot < table.SlotCount && table.IsHole(slot) ? 0 : -1;
        }
        CompleteShapes = TableShapes
            .Complete(holeSince)
            .Select(complete => complete.shape)
            .ToList();

        HashSet<int> shapeSlots = CompleteShapes.SelectMany(shape => shape.Slots).ToHashSet();
        Cells = holeSince
            .Select(
                (since, slot) =>
                    since < 0 ? TombolaCell.Empty
                    : shapeSlots.Contains(slot) ? TombolaCell.ShapeHole
                    : TombolaCell.Hole
            )
            .ToList();
    }

    // "quadrato 2x2: 1 | colonna: 2", or a line saying that no shape is complete. ASCII only, as GameLogFormatter.
    public string Summary()
    {
        if (CompleteShapes.Count == 0)
        {
            return "nessuna forma, per ora";
        }
        return string.Join(
            " | ",
            CompleteShapes
                .GroupBy(shape => shape.Kind)
                .OrderBy(group => group.Key)
                .Select(group => $"{KindLabel(group.Key)}: {group.Count()}")
        );
    }

    static string KindLabel(ShapeKind kind) =>
        kind switch
        {
            ShapeKind.Column => "colonna",
            ShapeKind.Diagonal => "diagonale",
            ShapeKind.Square2 => "quadrato 2x2",
            ShapeKind.Square3 => "quadrato 3x3",
            ShapeKind.Square4 => "quadrato 4x4",
            _ => kind.ToString(),
        };
}
