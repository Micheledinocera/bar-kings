using System.Collections.Generic;
using System.Linq;
using System.Text;

// Text of the game log of the table scenes (Console and on-screen log): the deck of the game, then every path with
// its length and its cards. Plain C#, so an autoplay can reuse it. ASCII only: TextMeshPro adds the characters
// missing from the font atlas to the dynamic fallback font asset, which would then change at every Play.
public static class GameLogFormatter
{
    public static string FormatDeal(string presetName, int seed, DeckDeal deal, bool excessToBench)
    {
        StringBuilder text = new();
        text.AppendLine($"New game | preset {presetName} | seed {seed}");
        text.AppendLine(
            $"Deck: {deal.DeckSize} cards ({DeckRules.KingCount} kings) | table {deal.TableCards} | holes {deal.Holes} | "
                + (excessToBench ? "bench" : "discarded")
                + $" {deal.Excess.Count}"
        );
        text.AppendLine($"Removed ({deal.Removed.Count}): {CardList(deal.Removed)}");
        text.Append($"Duplicates ({deal.Duplicates.Count}): {CardList(deal.Duplicates)}");
        return text.ToString();
    }

    // Length = cards moved by the path; a bench card filling the starting slot is listed apart. faceUpKings: kings
    // face up on the table after the path.
    public static string FormatPath(
        int pathNumber,
        int startSlot,
        IReadOnlyList<PathMove> moves,
        int faceUpKings
    )
    {
        List<PathMove> pathMoves = moves
            .Where(move => move.End != PathEnd.FilledFromBench)
            .ToList();
        StringBuilder text = new();
        text.AppendLine(
            $"Path {pathNumber} | slot {startSlot} | {pathMoves.Count} cards | {pathMoves[^1].End} | "
                + $"kings {faceUpKings}/{DeckRules.KingCount}"
        );
        text.Append(
            string.Join(" -> ", pathMoves.Select(move => $"{CardName(move.Card)} [{move.ToSlot}]"))
        );
        foreach (PathMove move in moves.Where(move => move.End == PathEnd.FilledFromBench))
        {
            text.Append($"\nBench: {CardName(move.Card)} [{move.ToSlot}] face down");
        }
        return text.ToString();
    }

    public static string FormatAllKings(int pathNumber) =>
        $"All {DeckRules.KingCount} kings face up at path {pathNumber}";

    public static string CardName(CardDataInGame card) =>
        $"{card.cardData.valore} {card.cardData.seme}";

    static string CardList(IEnumerable<CardDataInGame> cards)
    {
        string list = string.Join(", ", cards.Select(CardName));
        return list.Length > 0 ? list : "-";
    }
}
