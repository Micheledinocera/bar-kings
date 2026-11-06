using System.Collections.Generic;
using System.Linq;
using System.Text;

// TEMP Bar-Michele (feature events-impl): copy of PathReceipt for the receipt of the Bar-Michele scene
// (BarReceiptView). Differences from PathReceipt: a line is only the card, without its number in the path, and a
// closed path ends with "TOTALE". The empty receipt, the header and the card names are PathReceipt's. ASCII only,
// as GameLogFormatter.
public static class BarPathReceipt
{
    const string Separator = "- - - - - - - - - - - - - - - -";

    public static string Line(CardDataInGame card) => PathReceipt.CardLabel(card);

    public static string Footer(IReadOnlyList<PathMove> moves)
    {
        List<PathMove> pathMoves = moves
            .Where(move => move.End != PathEnd.FilledFromBench)
            .ToList();
        string cards = pathMoves.Count == 1 ? "1 CARTA" : $"{pathMoves.Count} CARTE";
        StringBuilder text = new();
        text.AppendLine(Separator);
        text.Append($"{EndLabel(pathMoves[^1].End)}<pos=70%>{cards}");
        foreach (PathMove move in moves.Where(move => move.End == PathEnd.FilledFromBench))
        {
            text.Append($"\n* dalla panchina: {PathReceipt.CardLabel(move.Card)} coperta");
        }
        return text.ToString();
    }

    static string EndLabel(PathEnd end) =>
        end switch
        {
            PathEnd.Closed => "TOTALE",
            PathEnd.FilledHole => "FINITO IN UN BUCO",
            PathEnd.StackedOnDuplicate => "SU UN DOPPIONE",
            _ => end.ToString().ToUpperInvariant(),
        };
}
