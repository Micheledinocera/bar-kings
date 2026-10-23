using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

// A deck to try: the 36 non-king cards of the base deck with some removed and some duplicates added, plus the 4
// kings, always there. Cards can be chosen (chosenRemoved, chosenDuplicates, the same at every game); the others
// removed or duplicated are drawn at every game (DeckBuilder), or with CardPick.First they are the first ones in
// table order. Assets in Assets/Instances/DeckPresets, created by the menus Tools/Genera Preset Mazzo,
// Tools/Genera Preset Forme and Tools/Preset Builder.
[CreateAssetMenu(fileName = "DeckPreset", menuName = "Carte/Deck Preset")]
public class DeckPreset : ScriptableObject
{
    // Removed beyond the chosen ones, picked as pick says.
    [SerializeField, MinValue(0), MaxValue(DeckRules.BaseNonKingCards)]
    private int removedCards;

    // Spread duplicates beyond the chosen ones: every kept card gets a copy before any gets a second one.
    [SerializeField, MinValue(0), MaxValue(DeckRules.MaxDuplicates)]
    private int duplicates;

    [SerializeField, EnumToggleButtons]
    private CardPick pick;

    // Non-king cards always removed.
    [SerializeField]
    private List<CardData> chosenRemoved = new();

    // Non-king cards always duplicated, each with its copies.
    [SerializeField]
    private List<ChosenCopies> chosenDuplicates = new();

    public int RemovedCards => removedCards;

    public int Duplicates => duplicates;

    public CardPick Pick => pick;

    public IReadOnlyList<CardData> ChosenRemoved => chosenRemoved;

    public IReadOnlyList<ChosenCopies> ChosenDuplicates => chosenDuplicates;

    // Name shown in the log and the options; no preset is the normal deck (40 cards, nothing removed or added).
    public static string NameOf(DeckPreset preset) => preset != null ? preset.name : "Normale";

    // Non-king cards of the deck; the kings are added to them.
    [ShowInInspector, ReadOnly]
    public int NonKingCards =>
        DeckRules.BaseNonKingCards
        - removedCards
        - chosenRemoved.Count
        + duplicates
        + chosenDuplicates.Sum(chosen => chosen.ExtraCopies);

#if UNITY_EDITOR
    public void Set(int removed, int duplicateCount, CardPick cardPick = CardPick.Random)
    {
        removedCards = removed;
        duplicates = duplicateCount;
        pick = cardPick;
    }

    public void SetChosen(IEnumerable<CardData> removed, IEnumerable<ChosenCopies> duplicated)
    {
        chosenRemoved = removed.ToList();
        chosenDuplicates = duplicated.ToList();
    }
#endif
}
