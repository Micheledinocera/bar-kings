// Effect of one card of a path on the player's stats: values 1-9 raise a stat by the card value, by suit. A king
// starts a fight instead (Fight). Rule being tried (wiki business/parte-rpg.md, decision in analysis). ASCII
// only, as GameLogFormatter.
public static class CardEffects
{
    public static bool IsKing(CardDataInGame card) => card.cardData.valore == DeckRules.KingValue;

    // Applies a card that is not a king to the stats and returns a line describing what happened. The config can
    // turn off the healing of Coppe and the armour of Bastoni (to try the fights without them).
    public static string Apply(CardDataInGame card, PlayerStats stats, PlayerConfig config)
    {
        int n = card.cardData.valore;
        string label = PathReceipt.CardLabel(card);
        switch (card.cardData.seme)
        {
            case Seme.Spade:
                stats.AddDamage(n);
                return $"{label}: +{n} danno";
            case Seme.Denari:
                int lire = n * RpgRules.LirePerDenaro;
                stats.AddMoney(lire);
                return $"{label}: +{lire} Lire";
            case Seme.Coppe when !config.healingFromCards:
                return $"{label}: cura disattivata";
            case Seme.Coppe:
                int healed = stats.Heal(n);
                return healed == n
                    ? $"{label}: +{n} vita"
                    : $"{label}: cura {n}, +{healed} vita (massimo {stats.MaxHealth})";
            case Seme.Bastoni when !config.armorFromCards:
                return $"{label}: armatura disattivata";
            case Seme.Bastoni:
                stats.AddArmor(n);
                return $"{label}: +{n} armatura";
            default:
                return $"{label}: nessun effetto";
        }
    }
}
