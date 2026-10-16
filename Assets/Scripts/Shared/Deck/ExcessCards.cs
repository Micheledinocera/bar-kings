// What happens to the cards beyond the 40 base slots: the deck-modification idea tried by each table scene.
public enum ExcessCards
{
    Bench, // Panchina: they wait face down in the pile and fill the holes left by paths
    Discard, // Tribuna: they are thrown away, holes stay
    Index, // Index40: every added card gets the next free index and its own slot, in up to 4 extra rows
}
