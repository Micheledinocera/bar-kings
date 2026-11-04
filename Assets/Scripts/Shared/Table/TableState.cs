using System.Collections.Generic;
using System.Linq;

// Logical state of the table (Panchina and Tribuna scenes): the cards in each slot (a stack: duplicates end up
// one on top of the other) and whether the slot is face up. An empty slot is a hole. In the Panchina the cards
// beyond the table wait in the bench, which fills the holes left by paths; in the Tribuna the bench is empty and
// holes stay. The scene mirrors this state (TableDeckManager); rules read it, never the Transform hierarchy.
public class TableState
{
    readonly List<CardDataInGame>[] stacks;
    readonly bool[] faceUp;
    readonly Queue<CardDataInGame> bench;
    readonly PathShortening shortening;

    // dealtCards[i] is the card dealt to slot i, face down; null leaves slot i a hole (a deck smaller than the
    // table). benchCards are drawn in order to fill holes. shortening: null plays the paths as dealt.
    public TableState(
        IReadOnlyList<CardDataInGame> dealtCards,
        IEnumerable<CardDataInGame> benchCards,
        PathShortening shortening = null
    )
    {
        this.shortening = shortening;
        stacks = new List<CardDataInGame>[dealtCards.Count];
        faceUp = new bool[dealtCards.Count];
        for (int i = 0; i < dealtCards.Count; i++)
        {
            stacks[i] = new List<CardDataInGame>();
            if (dealtCards[i] != null)
            {
                stacks[i].Add(dealtCards[i]);
            }
        }
        bench = new Queue<CardDataInGame>(benchCards);
    }

    public int SlotCount => stacks.Length;

    public int BenchCount => bench.Count;

    // Top card of the slot, null for a hole.
    public CardDataInGame GetCard(int slot) => stacks[slot].Count > 0 ? stacks[slot][^1] : null;

    public int StackSize(int slot) => stacks[slot].Count;

    public bool IsHole(int slot) => stacks[slot].Count == 0;

    public bool IsFaceUp(int slot) => faceUp[slot];

    // Kings in face-up slots. A king is never covered: kings have no duplicates, so nothing stacks on them.
    public int FaceUpKings
    {
        get
        {
            int kings = 0;
            for (int i = 0; i < stacks.Length; i++)
            {
                if (faceUp[i])
                {
                    kings += stacks[i].Count(card => card.cardData.valore == DeckRules.KingValue);
                }
            }
            return kings;
        }
    }

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
    // When the path stops, the starting slot is left empty: the next card of the panchina fills it face down
    // (PathEnd.FilledFromBench). With the panchina empty, the slot stays a hole. POC rules, see wiki
    // business/modifica-del-mazzo.md.
    // With a PathShortening, the path may close early: swap is the two face-down cards swapped so that the next
    // card closes, null when the path was played as dealt.
    public List<PathMove> PlayPath(int startSlot, out CardSwap? swap)
    {
        swap = null;
        List<PathMove> moves = PlayPathMoves(startSlot, ref swap);
        if (IsHole(startSlot) && bench.Count > 0)
        {
            CardDataInGame benchCard = bench.Dequeue();
            stacks[startSlot].Add(benchCard);
            faceUp[startSlot] = false;
            moves.Add(new PathMove(benchCard, startSlot, 0, PathEnd.FilledFromBench));
        }
        return moves;
    }

    public List<PathMove> PlayPath(int startSlot) => PlayPath(startSlot, out _);

    List<PathMove> PlayPathMoves(int startSlot, ref CardSwap? swap)
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

            if (
                shortening != null
                && stacks[target][^1].index != startSlot
                && shortening.ShouldClose(moves.Count + 1)
            )
            {
                swap = SwapInCloser(target, startSlot);
            }
            CardDataInGame displaced = stacks[target][^1];
            stacks[target].Clear();
            stacks[target].Add(inHand);
            faceUp[target] = true;
            moves.Add(new PathMove(inHand, target, 0, PathEnd.None));
            inHand = displaced;
        }
    }

    // Swaps the face-down card of target with a face-down card that belongs to the starting slot, so that the path
    // closes with the next card. Null when there is none (removed from the deck, or already face up): the path goes
    // on as dealt. Face-down slots hold one card: cards stack only on face-up slots.
    CardSwap? SwapInCloser(int target, int startSlot)
    {
        for (int slot = 0; slot < stacks.Length; slot++)
        {
            if (
                slot == target
                || faceUp[slot]
                || IsHole(slot)
                || stacks[slot][^1].index != startSlot
            )
            {
                continue;
            }
            CardDataInGame closer = stacks[slot][^1];
            CardDataInGame replaced = stacks[target][^1];
            stacks[slot][^1] = replaced;
            stacks[target][^1] = closer;
            return new CardSwap(closer, slot, replaced, target);
        }
        return null;
    }
}

public enum PathEnd
{
    None, // the path goes on
    Closed, // the card went back to the starting slot
    FilledHole, // the card went into a hole; the starting slot is a hole
    StackedOnDuplicate, // the card went on top of a duplicate; the starting slot is a hole
    FilledFromBench, // after the path, a panchina card went face down into the starting slot
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
