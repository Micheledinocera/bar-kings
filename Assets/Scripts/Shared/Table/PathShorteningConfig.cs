using Sirenix.OdinInspector;
using UnityEngine;

// Settings of the hidden path shortening (PathShortening), for the first encounters of a run, while the deck is
// still unaltered and the paths are long. Asset in Assets/Instances/PathShortening.
[CreateAssetMenu(fileName = "PathShorteningConfig", menuName = "Carte/Path Shortening Config")]
public class PathShorteningConfig : ScriptableObject
{
    // Encounters are numbered from 1. There is one encounter per game for now: the run of bars and encounters does
    // not exist yet.
    public const int FirstEncounter = 1;

    // Cards moved by a path before the shortening can start.
    [SerializeField, MinValue(1)]
    private int startAfterCards = 8;

    // Chance added at each card from startAfterCards on (0.1 = +10% per card).
    [SerializeField, Range(0f, 1f)]
    private float chanceIncrement = 0.1f;

    // Encounters of the run with the shortening on, from the first; 0 turns it off.
    [SerializeField, MinValue(0)]
    private int encounters = 1;

    // The shortening of an encounter, or null when it is off for that encounter.
    public PathShortening Create(int encounter, System.Random random) =>
        encounter <= encounters
            ? new PathShortening(startAfterCards, chanceIncrement, random)
            : null;

    public override string ToString() =>
        $"shortening from card {startAfterCards}, +{chanceIncrement:P0} per card, first {encounters} encounters";
}
