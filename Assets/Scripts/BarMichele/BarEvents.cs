using System.Collections.Generic;
using System.Linq;

// TEMP Bar-Michele (feature events-impl): the special card events being tried in the Bar-Michele scene (raw
// docs/raw/events-impl/2026-11-06-idee-eventi.md, decision in analysis). Cards up to MaxParametricValue raise a
// stat (CardEffects); the others are events:
// - horse (9): gives escapes, no stat. The santini that repeat a card's effect (San Francesco) give one more each;
// - king: the player chooses to fight or escape. An escape is free while escapes are left, otherwise it costs a
//   part of the Lire (a placeholder malus, to be decided);
// - fante (8): the player chooses to enter the shop (nothing yet) or to skip it for a share of the Lire a game
//   can give;
// - seven: raises its stat like the cards below it, and the four sevens of an encounter give a bonus.
// ASCII only, as GameLogFormatter.
public static class BarEvents
{
    public const int MaxParametricValue = 7; // cards up to the 7 raise a stat
    public const int FanteValue = 8;
    public const int EscapePenaltyLirePercent = 25; // of the Lire owned, when escaping with no escape left
    public const int SkipShopShareDivisor = 4; // skipping the shop gives 1/4 of the Lire a game can give

    public static bool IsHorse(CardDataInGame card) => card.cardData.valore == RpgRules.HorseValue;

    public static bool IsFante(CardDataInGame card) => card.cardData.valore == FanteValue;

    // Sevens: each raises its stat like the other parametric cards, and turning all of them in one encounter
    // gives a bonus on every stat, twice what the four sevens give on their own.
    public const int SevenValue = 7;
    public const int SevensForBonus = 4; // one per suit
    public const int SevensBonus = 2 * SevenValue;

    public static bool IsSeven(CardDataInGame card) => card.cardData.valore == SevenValue;

    // Every slot of the table is face up or a hole: no card is left to turn.
    public static bool AllCardsTurned(TableState table) =>
        Enumerable
            .Range(0, table.SlotCount)
            .All(slot => table.IsHole(slot) || table.IsFaceUp(slot));

    // Applies the bonus of the four sevens and returns a line describing it.
    public static string ApplySevensBonus(PlayerStats stats)
    {
        int lire = SevensBonus * RpgRules.LirePerDenaro;
        stats.AddDamage(SevensBonus);
        stats.AddArmor(SevensBonus);
        int healed = stats.Heal(SevensBonus);
        stats.AddMoney(lire);
        return $"+{SevensBonus} danno, +{SevensBonus} armatura, +{healed} vita, +{lire} Lire";
    }

    // Escapes given by a horse: one, plus one for each extra application of the santini owned.
    public static int EscapesFrom(CardDataInGame card, IReadOnlyList<Santino> santini) =>
        1 + santini.Where(santino => santino != null).Sum(s => s.ExtraApplications(card));

    // Lire lost escaping from a king with no escape left.
    public static int EscapePenalty(int lireOwned) => lireOwned * EscapePenaltyLirePercent / 100;

    // Lire a game can give: the Denari cards that raise a stat (1 to MaxParametricValue), without santini.
    public static int LirePerGame =>
        Enumerable.Range(1, MaxParametricValue).Sum() * RpgRules.LirePerDenaro;

    // Lire for skipping the shop of a fante.
    public static int SkipShopLire => LirePerGame / SkipShopShareDivisor;
}
