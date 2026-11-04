using System;

// Hidden shortening of the paths (rule being tried, wiki business/modifica-del-mazzo.md, decision in analysis): from
// the startAfterCards-th card moved by a path, before each next card the chance that it is the card closing the path
// grows by chanceIncrement. When the roll succeeds, TableState swaps two face-down cards so that the next card closes.
// The random source continues the one of the deal, so a game seed replays the same swaps for the same clicks.
public class PathShortening
{
    readonly int startAfterCards;
    readonly double chanceIncrement;
    readonly Random random;

    public PathShortening(int startAfterCards, double chanceIncrement, Random random)
    {
        this.startAfterCards = startAfterCards;
        this.chanceIncrement = chanceIncrement;
        this.random = random;
    }

    // cardsMoved: cards moved by the path so far, the last one included.
    public bool ShouldClose(int cardsMoved)
    {
        if (cardsMoved < startAfterCards)
        {
            return false;
        }
        double chance = (cardsMoved - startAfterCards + 1) * chanceIncrement;
        return random.NextDouble() < chance;
    }
}
