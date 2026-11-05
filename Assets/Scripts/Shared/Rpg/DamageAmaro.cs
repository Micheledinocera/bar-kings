using System;
using UnityEngine;

// Test amaro: raises the damage of the player by damageBonus for the rest of the run.
[Serializable]
public class DamageAmaro : Amaro
{
    [SerializeField, Min(1)]
    private int damageBonus = RpgRules.DamageAmaroBonus;

    public override string Name => "Amaro Affilato";

    public override string Drink(PlayerStats stats)
    {
        stats.AddDamage(damageBonus);
        return $"{Name}: +{damageBonus} danno per il resto della run (danno {stats.Damage})";
    }
}
