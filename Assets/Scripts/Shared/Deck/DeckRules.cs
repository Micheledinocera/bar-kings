// Numbers of the Neapolitan deck and of the table, shared by the deck builder and the table scenes.
public static class DeckRules
{
    public const int TableSlots = 40; // one slot per card of the base deck
    public const int KingValue = 10;
    public const int KingCount = 4;
    public const int BaseNonKingCards = TableSlots - KingCount;

    public const int SlotsPerRow = 10;
    public const int BaseRows = TableSlots / SlotsPerRow; // one row per suit (Seme order), one column per value

    // Index>40: rows added after the base table, one slot per added card.
    public const int MaxExtraRows = 4;
    public const int MaxTableSlots = TableSlots + MaxExtraRows * SlotsPerRow;

    // Duplicates of a deck preset: at most one per extra slot, so that index>40 can deal every preset.
    public const int MaxDuplicates = MaxTableSlots - TableSlots;
}
