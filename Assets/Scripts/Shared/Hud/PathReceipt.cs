using System.Collections.Generic;
using System.Linq;
using System.Text;

// Text of the receipt of the Bar scene, written while the path is played: the header, one line per card as it is
// turned, then how the path ended. Cards only: the events of the cards are not decided yet (wiki
// business/parte-rpg.md). ASCII only, as GameLogFormatter.
public static class PathReceipt
{
    const string Separator = "- - - - - - - - - - - - - - - -";

    public const string Empty = "nessun path ancora:\nscegli una carta coperta";

    public static string Header(int pathNumber) =>
        $"scontrino non fiscale - path n. {pathNumber}\n{Separator}";

    // number: 1 for the first card of the path.
    public static string Line(int number, CardDataInGame card) => $"{number, 2}  {CardLabel(card)}";

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
            text.Append($"\n* dalla panchina: {CardLabel(move.Card)} coperta");
        }
        return text.ToString();
    }

    // "7 SPADE", "ASSO DENARI", "RE COPPE".
    public static string CardLabel(CardDataInGame card) =>
        $"{ValueLabel(card.cardData.valore)} {card.cardData.seme.ToString().ToUpperInvariant()}";

    static string ValueLabel(int value) =>
        value switch
        {
            1 => "ASSO",
            8 => "FANTE",
            9 => "CAVALLO",
            DeckRules.KingValue => "RE",
            _ => value.ToString(),
        };

    static string EndLabel(PathEnd end) =>
        end switch
        {
            PathEnd.Closed => "CICLO CHIUSO",
            PathEnd.FilledHole => "FINITO IN UN BUCO",
            PathEnd.StackedOnDuplicate => "SU UN DOPPIONE",
            _ => end.ToString().ToUpperInvariant(),
        };
}
