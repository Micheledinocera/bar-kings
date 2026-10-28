using System;
using System.Collections.Generic;

// A fight between the player and a king, turn by turn on a FightClock until one side has no health left. Each
// hit does the attacker's damage minus the target's armour (never below 0); armour is not used up. Rule being
// tried (wiki business/parte-rpg.md, decision in analysis). ASCII only, as GameLogFormatter.
public static class Fight
{
    public static int HitDamage(IFighter attacker, IFighter target) =>
        Math.Max(0, attacker.Damage - target.Armor);

    // Plays the whole fight at once. The player's health stays as the fight leaves it.
    public static FightResult Run(PlayerStats player, KingFighter king)
    {
        List<string> turns = new();
        // Stats do not change during a fight: if no hit can do damage, it would never end.
        if (HitDamage(player, king) == 0 && HitDamage(king, player) == 0)
        {
            turns.Add("nessuno passa l'armatura dell'altro: pareggio");
            return new FightResult(FightOutcome.Stalemate, turns, 0, 0);
        }

        FightClock clock = new(new IFighter[] { player, king });
        int damageToKing = 0;
        int damageToPlayer = 0;
        while (true)
        {
            IFighter attacker = clock.NextTurn();
            IFighter target = attacker == player ? king : player;
            int lost = target.TakeDamage(HitDamage(attacker, target));
            if (attacker == player)
            {
                damageToKing += lost;
            }
            else
            {
                damageToPlayer += lost;
            }
            turns.Add(
                $"turno {clock.Turn}: {attacker.Name} colpisce {target.Name}: "
                    + $"{attacker.Damage} - {target.Armor} armatura = {lost} danni, "
                    + $"vita {target.Name} {target.Health}/{target.MaxHealth}"
            );
            if (target.Health == 0)
            {
                FightOutcome outcome =
                    attacker == player ? FightOutcome.PlayerWon : FightOutcome.KingWon;
                return new FightResult(outcome, turns, damageToKing, damageToPlayer);
            }
        }
    }
}
