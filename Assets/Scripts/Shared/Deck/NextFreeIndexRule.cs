// Index>40: every added card gets the next index after the base table (40, 41, ...), whatever the card.
public class NextFreeIndexRule : IDuplicateIndexRule
{
    public int IndexOf(CardDataInGame original, int duplicateNumber) =>
        DeckRules.TableSlots + duplicateNumber;
}
