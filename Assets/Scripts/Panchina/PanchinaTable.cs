using System.Collections.Generic;

// Logical state of the Panchina table: which card is in which slot and whether it is face up.
// The scene mirrors it (DeckManagerPanchina); rules read this, never the Transform hierarchy.
public class PanchinaTable
{
    readonly CardDataInGame[] cards;
    readonly bool[] faceUp;

    // dealtCards[i] is the card dealt to slot i, face down.
    public PanchinaTable(IReadOnlyList<CardDataInGame> dealtCards)
    {
        cards = new CardDataInGame[dealtCards.Count];
        faceUp = new bool[dealtCards.Count];
        for (int i = 0; i < dealtCards.Count; i++)
        {
            cards[i] = dealtCards[i];
        }
    }

    public int SlotCount => cards.Length;

    public CardDataInGame GetCard(int slot) => cards[slot];

    public bool IsFaceUp(int slot) => faceUp[slot];

    // Duplicates are distinct CardDataInGame instances, so the reference identifies one card on the table.
    public int SlotOf(CardDataInGame card)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (ReferenceEquals(cards[i], card))
            {
                return i;
            }
        }
        return -1;
    }

    // Plays a path from a face-down card and returns the moves in order. The card in hand goes face up to the
    // slot of its index, and the card it displaces becomes the card in hand. The path closes when the card in
    // hand belongs to the starting slot (it fills the hole). It stops when the target slot is already face up:
    // then the card in hand fills the starting slot (POC rule; see wiki business/modifica-del-mazzo.md).
    public List<PathMove> PlayPath(int startSlot)
    {
        List<PathMove> moves = new();
        CardDataInGame inHand = cards[startSlot];
        cards[startSlot] = null;

        while (true)
        {
            int target = inHand.index;
            if (target == startSlot || faceUp[target])
            {
                cards[startSlot] = inHand;
                faceUp[startSlot] = true;
                moves.Add(new PathMove(inHand, startSlot, target == startSlot));
                return moves;
            }

            CardDataInGame displaced = cards[target];
            cards[target] = inHand;
            faceUp[target] = true;
            moves.Add(new PathMove(inHand, target, false));
            inHand = displaced;
        }
    }
}

public readonly struct PathMove
{
    public readonly CardDataInGame Card;
    public readonly int ToSlot;

    // True on the last move when the path closed (the card went back to its own slot), false when it stopped.
    public readonly bool ClosesPath;

    public PathMove(CardDataInGame card, int toSlot, bool closesPath)
    {
        Card = card;
        ToSlot = toSlot;
        ClosesPath = closesPath;
    }
}
