// Two face-down cards swapped by the path shortening (PathShortening) before a path: Closer, the card that belongs to
// the starting slot, moved from CloserFrom to Target; Replaced, the card that was in Target, moved to CloserFrom. The
// scene swaps their GameObjects before animating the path.
public readonly struct CardSwap
{
    public readonly CardDataInGame Closer;
    public readonly int CloserFrom;
    public readonly CardDataInGame Replaced;
    public readonly int Target;

    public CardSwap(CardDataInGame closer, int closerFrom, CardDataInGame replaced, int target)
    {
        Closer = closer;
        CloserFrom = closerFrom;
        Replaced = replaced;
        Target = target;
    }
}
