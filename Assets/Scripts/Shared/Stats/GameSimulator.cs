using System;
using System.Collections.Generic;
using System.Linq;

// Autoplay of the table scenes: deals a deck as the scene does and plays paths until no face-down card is left.
// Every path starts from a face-down slot chosen at random: a trial strategy for the statistics, not a game rule.
// The game always ends: every path turns at least one card face up and the bench is finite.
public static class GameSimulator
{
    public static SimulatedGame Play(
        IReadOnlyList<CardDataInGame> baseCards,
        DeckPreset preset,
        bool excessToBench,
        int gameSeed,
        Random choices
    )
    {
        DeckDeal deal = DeckBuilder.Build(
            baseCards,
            preset.RemovedCards,
            preset.Duplicates,
            new Random(gameSeed)
        );
        TableState table = new(
            deal.TableSlots,
            excessToBench ? deal.Excess : Array.Empty<CardDataInGame>()
        );

        List<SimulatedPath> paths = new();
        int allKingsPath = 0;
        List<int> faceDown = FaceDownSlots(table);
        while (faceDown.Count > 0)
        {
            int startSlot = faceDown[choices.Next(faceDown.Count)];
            List<PathMove> moves = table.PlayPath(startSlot);
            List<PathMove> pathMoves = moves
                .Where(move => move.End != PathEnd.FilledFromBench)
                .ToList();
            int faceUpKings = table.FaceUpKings;
            paths.Add(
                new SimulatedPath(
                    paths.Count + 1,
                    startSlot,
                    pathMoves.Select(move => move.Card).ToList(),
                    pathMoves[^1].End,
                    pathMoves.Count < moves.Count,
                    faceUpKings
                )
            );
            if (allKingsPath == 0 && faceUpKings == DeckRules.KingCount)
            {
                allKingsPath = paths.Count;
            }
            faceDown = FaceDownSlots(table);
        }
        return new SimulatedGame(gameSeed, deal, paths, allKingsPath);
    }

    static List<int> FaceDownSlots(TableState table) =>
        Enumerable
            .Range(0, table.SlotCount)
            .Where(slot => !table.IsHole(slot) && !table.IsFaceUp(slot))
            .ToList();
}
