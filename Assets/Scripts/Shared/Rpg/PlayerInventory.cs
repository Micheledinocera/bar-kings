using System;
using System.Collections.Generic;
using UnityEngine;

// The santini and amari the player owns. Serialized with [SerializeReference] so that Odin shows a type picker:
// entries can be added, removed or changed in the Inspector during Play mode, and the scene picks them up at once
// (RpgStatsView). Not reset by a new game (Reset): it is the test inventory. Empty entries (null) are ignored.
[Serializable]
public class PlayerInventory
{
    [SerializeReference]
    public List<Santino> santini = new();

    [SerializeReference]
    public List<Amaro> amari = new();
}
