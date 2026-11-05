using System;

// A santino: a passive power-up the player owns for the whole run, triggered by events of the path. Each santino
// is a subclass that overrides only the hooks it uses; the player's santini are a [SerializeReference] list of
// PlayerInventory, editable in the Inspector during Play mode. Rule being tried (wiki
// business/meccaniche-della-run.md, decision in analysis). ASCII only, as GameLogFormatter.
[Serializable]
public abstract class Santino
{
    public abstract string Name { get; }

    // How many more times the effect of a card (not a king) is applied.
    public virtual int ExtraApplications(CardDataInGame card) => 0;

    // Percentage added to the Lire given by a card.
    public virtual int LireBonusPercent(CardDataInGame card) => 0;
}
