using System;

// A king in a fight: fresh stats from KingConfig for every fight.
public class KingFighter : IFighter
{
    public string Name { get; }
    public int Health { get; private set; }
    public int MaxHealth { get; }
    public int Armor { get; }
    public int Damage { get; }

    public KingFighter(string name, int health, int armor, int damage)
    {
        Name = name;
        Health = MaxHealth = health;
        Armor = armor;
        Damage = damage;
    }

    public int TakeDamage(int amount)
    {
        int lost = Math.Clamp(amount, 0, Health);
        Health -= lost;
        return lost;
    }
}
