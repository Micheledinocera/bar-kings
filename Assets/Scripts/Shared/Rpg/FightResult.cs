using System.Collections.Generic;

// What happened in a fight: the outcome, one line per turn and the health each side took from the other.
public class FightResult
{
    public FightOutcome Outcome { get; }
    public IReadOnlyList<string> Turns { get; }
    public int DamageToKing { get; }
    public int DamageToPlayer { get; }

    public FightResult(
        FightOutcome outcome,
        IReadOnlyList<string> turns,
        int damageToKing,
        int damageToPlayer
    )
    {
        Outcome = outcome;
        Turns = turns;
        DamageToKing = damageToKing;
        DamageToPlayer = damageToPlayer;
    }
}
