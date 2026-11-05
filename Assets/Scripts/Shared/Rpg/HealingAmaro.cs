using System;
using UnityEngine;

// Cerottino Martini: heals healthPercent of the maximum health (at least 1), not above the maximum.
[Serializable]
public class HealingAmaro : Amaro
{
    [SerializeField, Range(1, 100)]
    private int healthPercent = RpgRules.HealingAmaroPercent;

    public override string Name => "Cerottino Martini";

    public override string Drink(PlayerStats stats)
    {
        int amount = Math.Max(1, stats.MaxHealth * healthPercent / 100);
        int healed = stats.Heal(amount);
        return $"{Name}: cura {healthPercent}% ({amount}), +{healed} vita "
            + $"(vita {stats.Health}/{stats.MaxHealth})";
    }
}
