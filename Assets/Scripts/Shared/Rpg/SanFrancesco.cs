using System;

// San Francesco d'Assisi, who talked to animals: the horse (9) of a path applies its effect twice.
[Serializable]
public class SanFrancesco : Santino
{
    public override string Name => "San Francesco";

    public override int ExtraApplications(CardDataInGame card) =>
        card.cardData.valore == RpgRules.HorseValue ? 1 : 0;
}
