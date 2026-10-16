// Numbers of the Neapolitan deck and of the table, shared by the deck builder and the table scenes.
public static class DeckRules
{
    public const int TableSlots = 40; // one slot per card of the base deck
    public const int KingValue = 10;
    public const int KingCount = 4;
    public const int BaseNonKingCards = TableSlots - KingCount;
}
