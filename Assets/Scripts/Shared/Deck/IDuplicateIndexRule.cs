// Index>40: the index, and so the table slot, given to each card added to the deck. Rules are meant to vary the
// table configuration when the deck is built; today only NextFreeIndexRule exists.
public interface IDuplicateIndexRule
{
    // duplicateNumber counts the added cards from 0, in the order they are added.
    int IndexOf(CardDataInGame original, int duplicateNumber);
}
