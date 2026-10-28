using System.Collections.Generic;

// Order of the turns of a fight, not timed. Today every fighter acts once per round, in the given order (the
// player first, then the king); a speed stat could later give a fighter more turns before the others.
public class FightClock
{
    readonly IReadOnlyList<IFighter> fighters;
    int next;

    public FightClock(IReadOnlyList<IFighter> fighters) => this.fighters = fighters;

    // Number of the last turn given, 1 for the first.
    public int Turn { get; private set; }

    // The fighter who acts in the next turn.
    public IFighter NextTurn()
    {
        Turn++;
        IFighter fighter = fighters[next];
        next = (next + 1) % fighters.Count;
        return fighter;
    }
}
