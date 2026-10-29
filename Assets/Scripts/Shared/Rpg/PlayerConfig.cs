using System;
using UnityEngine;

// Starting stats of the player, set in the Inspector. Starting health above maxHealth is lowered to it.
[Serializable]
public class PlayerConfig
{
    [Min(1)]
    public int maxHealth = RpgRules.StartingMaxHealth;

    [Min(1)]
    public int startingHealth = RpgRules.StartingMaxHealth;

    [Min(0)]
    public int damage = RpgRules.StartingDamage;

    [Min(0)]
    public int armor = RpgRules.StartingArmor;

    // Off: the Coppe cards of a path give no healing.
    public bool healingFromCards = true;

    // Off: the Bastoni cards of a path give no armour.
    public bool armorFromCards = true;

    public PlayerStats CreateStats() => new(startingHealth, maxHealth, damage, armor);
}
