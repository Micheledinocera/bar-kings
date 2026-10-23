using System;
using Sirenix.OdinInspector;
using UnityEngine;

// A non-king card a deck preset duplicates on purpose, with the number of copies added to the original.
[Serializable]
public class ChosenCopies
{
    [SerializeField, Required]
    private CardData card;

    [SerializeField, MinValue(1), MaxValue(DeckRules.MaxDuplicates)]
    private int extraCopies = 1;

    public CardData Card => card;

    public int ExtraCopies => extraCopies;

    public ChosenCopies(CardData card, int extraCopies)
    {
        this.card = card;
        this.extraCopies = extraCopies;
    }
}
