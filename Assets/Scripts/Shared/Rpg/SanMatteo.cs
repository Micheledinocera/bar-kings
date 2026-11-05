using System;
using UnityEngine;

// San Matteo, tax collector and patron of bankers: a Denari card gives lireBonusPercent more Lire.
[Serializable]
public class SanMatteo : Santino
{
    [SerializeField, Min(0)]
    private int lireBonusPercent = RpgRules.SanMatteoLireBonusPercent;

    public override string Name => "San Matteo";

    public override int LireBonusPercent(CardDataInGame card) =>
        card.cardData.seme == Seme.Denari ? lireBonusPercent : 0;
}
