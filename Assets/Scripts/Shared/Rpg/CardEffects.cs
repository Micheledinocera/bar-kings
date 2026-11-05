using System.Collections.Generic;
using System.Linq;

// Effect of one card of a path on the player's stats: values 1-9 raise a stat by the card value, by suit. A king
// starts a fight instead (Fight). The player's santini can repeat the effect or raise the Lire. Rule being tried
// (wiki business/parte-rpg.md, decision in analysis). ASCII only, as GameLogFormatter.
public static class CardEffects
{
    public static bool IsKing(CardDataInGame card) => card.cardData.valore == DeckRules.KingValue;

    // Applies a card that is not a king to the stats and returns a line describing what happened. The config can
    // turn off the healing of Coppe and the armour of Bastoni (to try the fights without them).
    public static string Apply(
        CardDataInGame card,
        PlayerStats stats,
        PlayerConfig config,
        IReadOnlyList<Santino> santini
    )
    {
        List<Santino> owned = santini.Where(santino => santino != null).ToList();
        List<Santino> repeaters = owned.Where(s => s.ExtraApplications(card) > 0).ToList();
        int applications = 1 + repeaters.Sum(s => s.ExtraApplications(card));
        List<Santino> lireBoosters = owned.Where(s => s.LireBonusPercent(card) > 0).ToList();
        int lireBonusPercent = lireBoosters.Sum(s => s.LireBonusPercent(card));

        string label = PathReceipt.CardLabel(card);
        List<string> effects = new();
        for (int i = 0; i < applications; i++)
        {
            effects.Add(ApplyOnce(card, stats, config, lireBonusPercent));
        }

        string line = $"{label}: {string.Join(", ", effects)}";
        if (repeaters.Count > 0)
        {
            line += $" (x{applications} {Names(repeaters)})";
        }
        if (lireBoosters.Count > 0)
        {
            line += $" (+{lireBonusPercent}% Lire {Names(lireBoosters)})";
        }
        return line;
    }

    static string ApplyOnce(
        CardDataInGame card,
        PlayerStats stats,
        PlayerConfig config,
        int lireBonusPercent
    )
    {
        int n = card.cardData.valore;
        switch (card.cardData.seme)
        {
            case Seme.Spade:
                stats.AddDamage(n);
                return $"+{n} danno";
            case Seme.Denari:
                int lire = n * RpgRules.LirePerDenaro;
                lire += lire * lireBonusPercent / 100;
                stats.AddMoney(lire);
                return $"+{lire} Lire";
            case Seme.Coppe when !config.healingFromCards:
                return "cura disattivata";
            case Seme.Coppe:
                int healed = stats.Heal(n);
                return healed == n
                    ? $"+{n} vita"
                    : $"cura {n}, +{healed} vita (massimo {stats.MaxHealth})";
            case Seme.Bastoni when !config.armorFromCards:
                return "armatura disattivata";
            case Seme.Bastoni:
                stats.AddArmor(n);
                return $"+{n} armatura";
            default:
                return "nessun effetto";
        }
    }

    static string Names(IEnumerable<Santino> santini) =>
        string.Join(", ", santini.Select(santino => santino.Name));
}
