using System;

// Stats of the player in the RPG part: damage, health (capped at MaxHealth), armour and money (Lire).
public class PlayerStats : IFighter
{
    public string Name => "GIOCATORE";
    public int Damage { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; }
    public int Armor { get; private set; }
    public int Money { get; private set; }

    public PlayerStats(int startingHealth, int maxHealth, int damage, int armor)
    {
        MaxHealth = maxHealth;
        Health = Math.Clamp(startingHealth, 0, maxHealth);
        Damage = damage;
        Armor = armor;
    }

    public void AddDamage(int amount) => Damage += amount;

    public void AddArmor(int amount) => Armor += amount;

    public void AddMoney(int amount) => Money += amount;

    // Health never goes above MaxHealth: returns the amount actually healed.
    public int Heal(int amount)
    {
        int healed = Math.Clamp(amount, 0, MaxHealth - Health);
        Health += healed;
        return healed;
    }

    public int TakeDamage(int amount)
    {
        int lost = Math.Clamp(amount, 0, Health);
        Health -= lost;
        return lost;
    }
}
