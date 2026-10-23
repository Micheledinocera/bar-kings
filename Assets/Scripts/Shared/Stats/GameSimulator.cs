using System;
using System.Collections.Generic;
using System.Linq;

// Autoplay of the table scenes: deals a deck as the scene does and plays paths until the encounter ends, right after
// the path that turns the 4th king face up (rule being tried for the tribuna, see wiki business/modifica-del-mazzo.md;
// the scenes go on until no face-down card is left). The starting slot of each path is chosen by the
// AutoplayStrategy: a trial strategy for the statistics, not a game rule.
// The game always ends: every path turns at least one card face up, and the kings are always on the table.
public static class GameSimulator
{
    public static SimulatedGame Play(
        IReadOnlyList<CardDataInGame> baseCards,
        DeckPreset preset,
        ExcessCards excess,
        int gameSeed,
        Random choices,
        AutoplayStrategy strategy = AutoplayStrategy.Random
    )
    {
        DeckDeal deal = DeckBuilder.Build(excess, baseCards, preset, new Random(gameSeed));
        TableState table = new(deal.TableSlots, deal.BenchCards(excess));
        HashSet<int> removedSlots = new(
            deal.Removed.Select(card => card.index).Where(slot => slot < table.SlotCount)
        );

        // Path after which each slot became a hole for the last time (0: since the deal), -1 when not a hole.
        int[] holeSince = new int[table.SlotCount];
        UpdateHoles(table, holeSince, 0);

        List<SimulatedPath> paths = new();
        int allKingsPath = 0;
        List<int> faceDown = FaceDownSlots(table);
        while (faceDown.Count > 0 && allKingsPath == 0)
        {
            int startSlot = ChooseStart(faceDown, removedSlots, strategy, choices);
            List<PathMove> moves = table.PlayPath(startSlot);
            List<PathMove> pathMoves = moves
                .Where(move => move.End != PathEnd.FilledFromBench)
                .ToList();
            int faceUpKings = table.FaceUpKings;
            UpdateHoles(table, holeSince, paths.Count + 1);
            paths.Add(
                new SimulatedPath(
                    paths.Count + 1,
                    pathMoves.Select(move => move.Card).ToList(),
                    pathMoves[^1].End,
                    pathMoves.Count < moves.Count,
                    faceUpKings,
                    holeSince.Count(since => since >= 0)
                )
            );
            if (allKingsPath == 0 && faceUpKings == DeckRules.KingCount)
            {
                allKingsPath = paths.Count;
            }
            faceDown = FaceDownSlots(table);
        }
        return new SimulatedGame(gameSeed, deal, paths, allKingsPath, holeSince, faceDown.Count);
    }

    static int ChooseStart(
        List<int> faceDown,
        HashSet<int> removedSlots,
        AutoplayStrategy strategy,
        Random choices
    )
    {
        if (strategy == AutoplayStrategy.Targeted)
        {
            List<int> targets = faceDown.Where(removedSlots.Contains).ToList();
            if (targets.Count > 0)
            {
                return targets[choices.Next(targets.Count)];
            }
        }
        return faceDown[choices.Next(faceDown.Count)];
    }

    static void UpdateHoles(TableState table, int[] holeSince, int path)
    {
        for (int slot = 0; slot < holeSince.Length; slot++)
        {
            if (!table.IsHole(slot))
            {
                holeSince[slot] = -1;
            }
            else if (holeSince[slot] < 0 || path == 0)
            {
                holeSince[slot] = path;
            }
        }
    }

    static List<int> FaceDownSlots(TableState table) =>
        Enumerable
            .Range(0, table.SlotCount)
            .Where(slot => !table.IsHole(slot) && !table.IsFaceUp(slot))
            .ToList();
}
