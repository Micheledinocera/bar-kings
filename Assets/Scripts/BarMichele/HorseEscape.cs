using System.Collections.Generic;
using System.Linq;

// TEMP Bar-Michele (feature events-impl): the horse (9) event being tried in the Bar-Michele scene (raw
// docs/raw/events-impl/2026-11-06-idee-eventi.md, decision in analysis). A horse of a path gives no stat, only
// escapes: each escape lets the player skip one fight with a king, by choice, until the end of the encounter.
// The santini that repeat a card's effect (San Francesco) give one more escape each. ASCII only, as
// GameLogFormatter.
public static class HorseEscape
{
    public static bool IsHorse(CardDataInGame card) => card.cardData.valore == RpgRules.HorseValue;

    // Escapes given by a horse: one, plus one for each extra application of the santini owned.
    public static int EscapesFrom(CardDataInGame card, IReadOnlyList<Santino> santini) =>
        1 + santini.Where(santino => santino != null).Sum(s => s.ExtraApplications(card));
}
