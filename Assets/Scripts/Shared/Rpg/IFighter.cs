// A side of a fight (the player or a king): its stats and the damage it takes.
public interface IFighter
{
    string Name { get; }
    int Health { get; }
    int MaxHealth { get; }
    int Armor { get; }
    int Damage { get; }

    // Loses health, never below 0: returns the health actually lost.
    int TakeDamage(int amount);
}
