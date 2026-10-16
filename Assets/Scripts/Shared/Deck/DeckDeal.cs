using System.Collections.Generic;
using System.Linq;

// The deck of one game, as built by DeckBuilder: the card dealt to each table slot and the cards left over.
public class DeckDeal
{
    // TableSlots[i] is the card dealt face down to slot i; null leaves the slot a hole. 40 slots, more in index>40.
    public IReadOnlyList<CardDataInGame> TableSlots { get; }

    // Non-king cards beyond the table, in draw order: the bench of the Panchina, discarded in the Tribuna; empty in
    // index>40, where every card has a slot.
    public IReadOnlyList<CardDataInGame> Excess { get; }

    // Base cards taken out of the deck.
    public IReadOnlyList<CardDataInGame> Removed { get; }

    // Copies added to the deck, each a distinct instance.
    public IReadOnlyList<CardDataInGame> Duplicates { get; }

    public DeckDeal(
        IReadOnlyList<CardDataInGame> tableSlots,
        IReadOnlyList<CardDataInGame> excess,
        IReadOnlyList<CardDataInGame> removed,
        IReadOnlyList<CardDataInGame> duplicates
    )
    {
        TableSlots = tableSlots;
        Excess = excess;
        Removed = removed;
        Duplicates = duplicates;
    }

    // Cards that wait in the bench: the excess in the panchina, none otherwise.
    public IReadOnlyList<CardDataInGame> BenchCards(ExcessCards excess) =>
        excess == ExcessCards.Bench ? Excess : System.Array.Empty<CardDataInGame>();

    public int TableCards => TableSlots.Count(card => card != null);

    public int Holes => TableSlots.Count - TableCards;

    // Table cards whose slot is already claimed by another copy of the same card.
    public int TableDuplicates =>
        TableCards
        - TableSlots.Where(card => card != null).Select(card => card.index).Distinct().Count();

    // Every card of the deck, kings included.
    public int DeckSize => TableCards + Excess.Count;
}
