using System;
using UnityEngine;

// Stats of the kings, set in the Inspector. Today every king has the same ones.
[Serializable]
public class KingConfig
{
    [Min(1)]
    public int health = RpgRules.KingHealth;

    [Min(0)]
    public int armor = RpgRules.KingArmor;

    [Min(0)]
    public int damage = RpgRules.KingDamage;

    public KingFighter CreateFighter(string name) => new(name, health, armor, damage);
}
