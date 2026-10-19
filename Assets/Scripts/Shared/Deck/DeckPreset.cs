using Sirenix.OdinInspector;
using UnityEngine;

// A deck to try: the 36 non-king cards of the base deck with some removed and some duplicates added, plus the 4
// kings, always there. Which cards are removed or duplicated is drawn at every game (DeckBuilder), or with
// CardPick.First they are the first ones in table order. Assets in Assets/Instances/DeckPresets, created by the menus
// Tools/Genera Preset Mazzo and Tools/Preset Builder.
[CreateAssetMenu(fileName = "DeckPreset", menuName = "Carte/Deck Preset")]
public class DeckPreset : ScriptableObject
{
    [SerializeField, MinValue(0), MaxValue(DeckRules.BaseNonKingCards)]
    private int removedCards;

    [SerializeField, MinValue(0), MaxValue(DeckRules.MaxDuplicates)]
    private int duplicates;

    [SerializeField, EnumToggleButtons]
    private CardPick pick;

    public int RemovedCards => removedCards;

    public int Duplicates => duplicates;

    public CardPick Pick => pick;

    // Non-king cards of the deck; the kings are added to them.
    [ShowInInspector, ReadOnly]
    public int NonKingCards => DeckRules.BaseNonKingCards - removedCards + duplicates;

#if UNITY_EDITOR
    public void Set(int removed, int duplicateCount, CardPick cardPick = CardPick.Random)
    {
        removedCards = removed;
        duplicates = duplicateCount;
        pick = cardPick;
    }
#endif
}
