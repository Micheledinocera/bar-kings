using System;
using System.Collections.Generic;
using System.Linq;

// Builds the deck of one game from the base cards: removes and duplicates non-king cards at random, then deals the
// table. Plain C#: the same seed gives the same deal, so a game can be replayed and an autoplay can run it.
public static class DeckBuilder
{
    // - removedCount distinct non-king cards are taken out;
    // - duplicateCount copies of the remaining non-king cards are added, spread out: every card gets a copy before
    //   any gets a second one;
    // - the table gets the 4 kings and up to 36 non-king cards, on random slots; with fewer cards the free slots
    //   are holes. The non-king cards beyond the table are the excess.
    public static DeckDeal Build(
        IReadOnlyList<CardDataInGame> baseCards,
        int removedCount,
        int duplicateCount,
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
            .ToList();
        if (removedCount < 0 || removedCount > others.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(removedCount),
                $"{removedCount} cards to remove out of {others.Count}"
            );
        }
        Shuffle(others, random);
        List<CardDataInGame> removed = others.Take(removedCount).ToList();
        List<CardDataInGame> kept = others.Skip(removedCount).ToList();
        if (duplicateCount > 0 && kept.Count == 0)
        {
            throw new ArgumentException("No card left to duplicate", nameof(duplicateCount));
        }

        List<CardDataInGame> duplicates = new(duplicateCount);
        for (int i = 0; i < duplicateCount; i++)
        {
            CardDataInGame original = kept[i % kept.Count];
            duplicates.Add(new CardDataInGame(original.cardData, original.index));
        }

        List<CardDataInGame> pool = kept.Concat(duplicates).ToList();
        Shuffle(pool, random);
        int tableCapacity = DeckRules.TableSlots - DeckRules.KingCount;
        List<CardDataInGame> excess = pool.Skip(tableCapacity).ToList();

        List<CardDataInGame> tableSlots = pool.Take(tableCapacity).Concat(kings).ToList();
        while (tableSlots.Count < DeckRules.TableSlots)
        {
            tableSlots.Add(null);
        }
        Shuffle(tableSlots, random);

        return new DeckDeal(tableSlots, excess, removed, duplicates);
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
