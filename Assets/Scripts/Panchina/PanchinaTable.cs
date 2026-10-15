using System.Collections.Generic;

// Logical state of the Panchina table: the cards in each slot (a stack: duplicates end up one on top of the
// other) and whether the slot is face up. An empty slot is a hole. The scene mirrors this state
// (DeckManagerPanchina); rules read it, never the Transform hierarchy.
public class PanchinaTable
{
    readonly List<CardDataInGame>[] stacks;
    readonly bool[] faceUp;

    // dealtCards[i] is the card dealt to slot i, face down.
    public PanchinaTable(IReadOnlyList<CardDataInGame> dealtCards)
    {
        stacks = new List<CardDataInGame>[dealtCards.Count];
        faceUp = new bool[dealtCards.Count];
        for (int i = 0; i < dealtCards.Count; i++)
        {
            stacks[i] = new List<CardDataInGame> { dealtCards[i] };
        }
    }

    public int SlotCount => stacks.Length;

    // Top card of the slot, null for a hole.
    public CardDataInGame GetCard(int slot) => stacks[slot].Count > 0 ? stacks[slot][^1] : null;

    public int StackSize(int slot) => stacks[slot].Count;

    public bool IsHole(int slot) => stacks[slot].Count == 0;

    public bool IsFaceUp(int slot) => faceUp[slot];

    // Slot of a card, wherever it is in its stack. Duplicates are distinct CardDataInGame instances, so the
    // reference identifies one card on the table.
    public int SlotOf(CardDataInGame card)
    {
        for (int i = 0; i < stacks.Length; i++)
        {
            foreach (CardDataInGame stacked in stacks[i])
            {
                if (ReferenceEquals(stacked, card))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    // Plays a path from a face-down card and returns the moves in order. The card in hand goes face up to the
    // slot of its index, and the card it displaces becomes the card in hand.
    // - Closes when the card in hand belongs to the starting slot: it fills it.
    // - Stops when the slot of the card in hand is a hole: the card goes face up into it.
    // - Stops when the slot of the card in hand is already face up (a duplicate is there): the card goes on top
    //   of it.
    // When the path stops, the starting slot stays empty (a new hole). POC rules, see wiki
    // business/modifica-del-mazzo.md.
    public List<PathMove> PlayPath(int startSlot)
    {
        List<PathMove> moves = new();
        CardDataInGame inHand = stacks[startSlot][^1];
        stacks[startSlot].Clear();

        while (true)
        {
            int target = inHand.index;
            if (target == startSlot)
            {
                stacks[startSlot].Add(inHand);
                faceUp[startSlot] = true;
                moves.Add(new PathMove(inHand, startSlot, 0, PathEnd.Closed));
                return moves;
            }
            if (IsHole(target))
            {
                stacks[target].Add(inHand);
                faceUp[target] = true;
                moves.Add(new PathMove(inHand, target, 0, PathEnd.FilledHole));
                return moves;
            }
            if (faceUp[target])
            {
                stacks[target].Add(inHand);
                moves.Add(
                    new PathMove(
                        inHand,
                        target,
                        stacks[target].Count - 1,
                        PathEnd.StackedOnDuplicate
                    )
                );
                return moves;
            }

            CardDataInGame displaced = stacks[target][^1];
            stacks[target].Clear();
            stacks[target].Add(inHand);
            faceUp[target] = true;
            moves.Add(new PathMove(inHand, target, 0, PathEnd.None));
            inHand = displaced;
        }
    }
}

public enum PathEnd
{
    None, // the path goes on
    Closed, // the card went back to the starting slot
    FilledHole, // the card went into a hole; the starting slot is a hole
    StackedOnDuplicate, // the card went on top of a duplicate; the starting slot is a hole
}

public readonly struct PathMove
{
    public readonly CardDataInGame Card;
    public readonly int ToSlot;

    // Position in the target stack: 0 = the only card, 1 = on top of one duplicate, ...
    public readonly int StackIndex;
    public readonly PathEnd End;

    public PathMove(CardDataInGame card, int toSlot, int stackIndex, PathEnd end)
    {
        Card = card;
        ToSlot = toSlot;
        StackIndex = stackIndex;
        End = end;
    }
}
