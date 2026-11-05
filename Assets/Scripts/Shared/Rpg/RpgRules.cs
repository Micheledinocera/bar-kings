// Default numbers of the RPG part being tried (wiki business/parte-rpg.md, decision in analysis). The stats of
// the player and of the kings start from these and are set in the Inspector (PlayerConfig, KingConfig).
public static class RpgRules
{
    public const int StartingMaxHealth = 100;
    public const int StartingDamage = 1;
    public const int StartingArmor = 1;

    public const int KingHealth = 10;
    public const int KingArmor = 0;
    public const int KingDamage = 1;

    public const int LirePerDenaro = 1000; // a Denari card of value n gives n * 1000 Lire

    // Santini and amari (wiki business/meccaniche-della-run.md, decision in analysis): defaults of their fields.
    public const int HorseValue = 9; // the cavallo, triggers San Francesco
    public const int SanMatteoLireBonusPercent = 50;
    public const int HealingAmaroPercent = 25;
    public const int DamageAmaroBonus = 5;
}
