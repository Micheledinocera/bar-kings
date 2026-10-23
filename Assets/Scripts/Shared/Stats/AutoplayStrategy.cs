// How the autoplay chooses the face-down slot each path starts from: trial strategies for the statistics, not game
// rules.
public enum AutoplayStrategy
{
    Random, // any face-down slot, at random
    Targeted, // first the face-down slots of the removed cards (a player who knows the deck), at random; then any
}
