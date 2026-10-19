using System;
using System.Collections.Generic;
using System.Linq;

// Builds the deck of one game from the base cards: removes and duplicates non-king cards, at random or the first
// ones in table order (CardPick), then deals the table. Plain C#: the same seed gives the same deal, so a game can be
// replayed and an autoplay can run it.
// - removedCount distinct non-king cards are taken out;
// - duplicateCount copies of the remaining non-king cards are added, spread out: every card gets a copy before
//   any gets a second one.
public static class DeckBuilder
{
    // The deal of a table scene for its excess mode: Build for the panchina and the tribuna, BuildIndexed with the
    // next free index for index>40.
    public static DeckDeal Build(
        ExcessCards excess,
        IReadOnlyList<CardDataInGame> baseCards,
        DeckPreset preset,
        Random random
    ) =>
        excess == ExcessCards.Index
            ? BuildIndexed(
                baseCards,
                preset.RemovedCards,
                preset.Duplicates,
                preset.Pick,
                new NextFreeIndexRule(),
                random
            )
            : Build(baseCards, preset.RemovedCards, preset.Duplicates, preset.Pick, random);

    // Panchina and tribuna: the 40 slots get the 4 kings and up to 36 non-king cards, shuffled on the first slots;
    // with fewer cards the last slots are holes. The non-king cards beyond the table are the excess.
    public static DeckDeal Build(
        IReadOnlyList<CardDataInGame> baseCards,
        int removedCount,
        int duplicateCount,
        CardPick pick,
        Random random
    )
    {
        (List<CardDataInGame> kings, List<CardDataInGame> removed, List<CardDataInGame> kept) =
            SplitBaseCards(baseCards, removedCount, pick, random);
        List<CardDataInGame> duplicates = Duplicate(
            kept,
            duplicateCount,
            (original, _) => original.index
        );

        List<CardDataInGame> pool = kept.Concat(duplicates).ToList();
        Shuffle(pool, random);
        int tableCapacity = DeckRules.TableSlots - DeckRules.KingCount;
        List<CardDataInGame> excess = pool.Skip(tableCapacity).ToList();

        List<CardDataInGame> tableSlots = pool.Take(tableCapacity).Concat(kings).ToList();
        Shuffle(tableSlots, random);
        while (tableSlots.Count < DeckRules.TableSlots)
        {
            tableSlots.Add(null);
        }

        return new DeckDeal(tableSlots, excess, removed, duplicates);
    }

    // Index>40: every added card gets its own index from indexRule, so the table grows past the 40 base slots (at
    // most DeckRules.MaxTableSlots). The slots of the removed cards stay holes; every other card, kings included, is
    // shuffled on the remaining slots. Nothing is left over.
    public static DeckDeal BuildIndexed(
        IReadOnlyList<CardDataInGame> baseCards,
        int removedCount,
        int duplicateCount,
        CardPick pick,
        IDuplicateIndexRule indexRule,
        Random random
    )
    {
        (List<CardDataInGame> kings, List<CardDataInGame> removed, List<CardDataInGame> kept) =
            SplitBaseCards(baseCards, removedCount, pick, random);
        List<CardDataInGame> duplicates = Duplicate(kept, duplicateCount, indexRule.IndexOf);

        int slotCount = DeckRules.TableSlots;
        HashSet<int> extraIndexes = new();
        foreach (CardDataInGame duplicate in duplicates)
        {
            if (
                duplicate.index < DeckRules.TableSlots
                || duplicate.index >= DeckRules.MaxTableSlots
                || !extraIndexes.Add(duplicate.index)
            )
            {
                throw new InvalidOperationException(
                    $"Added card index {duplicate.index}: expected a free index from {DeckRules.TableSlots} to "
                        + $"{DeckRules.MaxTableSlots - 1}"
                );
            }
            slotCount = Math.Max(slotCount, duplicate.index + 1);
        }

        HashSet<int> holes = new(removed.Select(card => card.index));
        List<int> freeSlots = Enumerable
            .Range(0, slotCount)
            .Where(slot => !holes.Contains(slot))
            .ToList();
        List<CardDataInGame> cards = kept.Concat(kings).Concat(duplicates).ToList();
        if (cards.Count != freeSlots.Count)
        {
            throw new InvalidOperationException(
                $"{cards.Count} cards for {freeSlots.Count} free slots: the index rule leaves gaps"
            );
        }
        Shuffle(cards, random);
        CardDataInGame[] tableSlots = new CardDataInGame[slotCount];
        for (int i = 0; i < cards.Count; i++)
        {
            tableSlots[freeSlots[i]] = cards[i];
        }

        return new DeckDeal(tableSlots, Array.Empty<CardDataInGame>(), removed, duplicates);
    }

    // The 4 kings, the removed non-king cards and the kept ones: after a shuffle, or in table order with
    // CardPick.First, so the first cards are removed and the first kept ones are duplicated.
    static (
        List<CardDataInGame> kings,
        List<CardDataInGame> removed,
        List<CardDataInGame> kept
    ) SplitBaseCards(
        IReadOnlyList<CardDataInGame> baseCards,
        int removedCount,
        CardPick pick,
        Random random
    )
    {
        List<CardDataInGame> kings = baseCards
            .Where(card => card.cardData.valore == DeckRules.KingValue)
            .ToList();
        if (kings.Count != DeckRules.KingCount)
        {
            throw new InvalidOperationException(
                $"The base deck has {kings.Count} kings, expected {DeckRules.KingCount}"
            );
        }

        List<CardDataInGame> others = baseCards
            .Where(card => card.cardData.valore != DeckRules.KingValue)
            .OrderBy(card => card.index)
            .ToList();
        if (removedCount < 0 || removedCount > others.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(removedCount),
                $"{removedCount} cards to remove out of {others.Count}"
            );
        }
        if (pick == CardPick.Random)
        {
            Shuffle(others, random);
        }
        return (kings, others.Take(removedCount).ToList(), others.Skip(removedCount).ToList());
    }

    // Copies of the kept cards, spread out; each copy is a new instance with the index given by indexOf.
    static List<CardDataInGame> Duplicate(
        List<CardDataInGame> kept,
        int duplicateCount,
        Func<CardDataInGame, int, int> indexOf
    )
    {
        if (duplicateCount > 0 && kept.Count == 0)
        {
            throw new ArgumentException("No card left to duplicate", nameof(duplicateCount));
        }
        List<CardDataInGame> duplicates = new(duplicateCount);
        for (int i = 0; i < duplicateCount; i++)
        {
            CardDataInGame original = kept[i % kept.Count];
            duplicates.Add(new CardDataInGame(original.cardData, indexOf(original, i)));
        }
        return duplicates;
    }

    static void Shuffle<T>(IList<T> list, Random random)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int j = random.Next(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
