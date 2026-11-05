using System;

// An amaro: a consumable the player drinks between two paths of an encounter (InventoryView), used up once
// drunk. Each amaro is a subclass; the player's amari are a [SerializeReference] list of PlayerInventory, editable
// in the Inspector during Play mode. Rule being tried (wiki business/meccaniche-della-run.md, decision in
// analysis). ASCII only, as GameLogFormatter.
[Serializable]
public abstract class Amaro
{
    public abstract string Name { get; }

    // Applies the amaro to the stats and returns a line describing what happened.
    public abstract string Drink(PlayerStats stats);
}
