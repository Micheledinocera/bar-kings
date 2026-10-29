---
tipo: info
aggiornato: 2026-10-29
verificato: 2026-10-29
fonti:
  - raw/compiled/merge-rpg/2026-10-28-obiettivo-rpg.md
  - raw/compiled/merge-rpg/2026-10-28-fight.md
  - raw/compiled/merge-rpg/2026-10-28-popup-e-config.md
  - raw/compiled/merge-rpg/2026-10-28-riassunto-sessione.md
---

# RPG scripts

Scripts in `Assets/Scripts/Shared/Rpg/`, used only by the scene `Assets/Scenes/TribunaRpg.unity` ([scenes](./scenes.md#tribunarpg)): the first try of the RPG part, the rules of [Parte RPG](../business/parte-rpg.md#cosa-sappiamo) being tried (decision in analysis, nothing here is fixed). At the end of each path the cards are resolved in order: values 1–9 raise a stat of the player, a king starts a fight. Rules in plain C# classes (`CardEffects`, `Fight`, stats), view in two MonoBehaviours. Texts are ASCII only, as [GameLogFormatter](./stats.md#gamelogformatter).

| Class | Path | Kind |
|---|---|---|
| [RpgStatsView](#rpgstatsview) | `Assets/Scripts/Shared/Rpg/RpgStatsView.cs` | MonoBehaviour |
| [FightPopup](#fightpopup) | `Assets/Scripts/Shared/Rpg/FightPopup.cs` | MonoBehaviour |
| [CardEffects](#cardeffects) | `Assets/Scripts/Shared/Rpg/CardEffects.cs` | static class |
| [Fight, FightClock, FightResult, FightOutcome](#fight-fightclock-fightresult-fightoutcome) | `Assets/Scripts/Shared/Rpg/Fight.cs`, `Assets/Scripts/Shared/Rpg/FightClock.cs`, `Assets/Scripts/Shared/Rpg/FightResult.cs`, `Assets/Scripts/Shared/Rpg/FightOutcome.cs` | static class, plain C#, enum |
| [IFighter, PlayerStats, KingFighter](#ifighter-playerstats-kingfighter) | `Assets/Scripts/Shared/Rpg/IFighter.cs`, `Assets/Scripts/Shared/Rpg/PlayerStats.cs`, `Assets/Scripts/Shared/Rpg/KingFighter.cs` | interface, plain C# |
| [PlayerConfig, KingConfig, RpgRules](#playerconfig-kingconfig-rpgrules) | `Assets/Scripts/Shared/Rpg/PlayerConfig.cs`, `Assets/Scripts/Shared/Rpg/KingConfig.cs`, `Assets/Scripts/Shared/Rpg/RpgRules.cs` | serializable classes, constants |

## RpgStatsView

`Assets/Scripts/Shared/Rpg/RpgStatsView.cs`. The RPG part of the scene, on `UI/RpgStats` of `TribunaRpg`. Listens only to [TableDeckManager](./table.md#tabledeckmanager): `GameStarted` resets the stats from `player` (so Reset starts again from the Inspector values), `PathPlayed` resolves the path.

- **Resolution** (`ResolvePath`, `async UniTaskVoid` with `destroyCancellationToken`): the cards of the path in order, without the bench move (`PathEnd.FilledFromBench` only fills a hole face down). A card that is not a king goes to [CardEffects](#cardeffects). A king starts a [Fight](#fight-fightclock-fightresult-fightoutcome) with the stats reached so far, then the [FightPopup](#fightpopup) is awaited while `TableDeckManager.TablePaused` is on; one popup per king of the path.
- **Defeat** (`FightOutcome.KingWon`): calls `TableDeckManager.EndEncounter()` (the table ignores clicks until Reset; being inside `PathPlayed`, it also raises `EncounterEnded`) and the cards after the king are not applied.
- **Output**: every card, turn and the stats at the end of the path in the Console; on screen `statsText` (damage, health/max, armour, Lire) and `eventsText` (last path: number of cards, or the outcome of its fights).
- **Serialized fields**: `tableDeckManager`, `statsText`, `eventsText`, `fightPopup` (all `[Required]`), `player` ([PlayerConfig](#playerconfig-kingconfig-rpgrules)), `king` ([KingConfig](#playerconfig-kingconfig-rpgrules)).
- Uses `PathReceipt.CardLabel` ([PathReceipt](./hud.md#pathreceipt)) for the card names.

## FightPopup

`Assets/Scripts/Shared/Rpg/FightPopup.cs`. On `UI/FightPopup` of `TribunaRpg` (last child, full screen, inactive while hidden). `Show(title, body, cancellationToken)` (`UniTask`) sets the texts, activates the popup and completes when `closeButton` ("Continua") is clicked, then hides it. The full-screen background blocks the UI raycasts; the table cards are blocked by `TableDeckManager.TablePaused`, set by [RpgStatsView](#rpgstatsview), because `OnMouseDown` gets the click before the UI ([known issues](./known-issues.md#block-d--input)).

- **Serialized fields**: `title`, `body`, `closeButton` (an instance of `Assets/Prefab/OptionButton.prefab`).
- **Used by**: [RpgStatsView](#rpgstatsview).

## CardEffects

`Assets/Scripts/Shared/Rpg/CardEffects.cs`. Rule being tried for a card of value `n` (1–9), by suit: **Spade** +n damage, **Denari** +n × `RpgRules.LirePerDenaro` (1000) Lire, **Coppe** +n health (not above the maximum), **Bastoni** +n armour. `IsKing(card)`: value `DeckRules.KingValue` ([DeckRules](./deck.md#deckrules)). `Apply(card, stats, config)` changes the [PlayerStats](#ifighter-playerstats-kingfighter) and returns the log line; `config.healingFromCards` and `config.armorFromCards` off turn the Coppe and Bastoni effects off.

- **Used by**: [RpgStatsView](#rpgstatsview).

## Fight, FightClock, FightResult, FightOutcome

`Assets/Scripts/Shared/Rpg/Fight.cs`. `Run(player, king)` plays the whole fight at once and returns a `FightResult` (outcome, one line per turn, damage done by each side); the player's health stays as the fight leaves it. `HitDamage(attacker, target)` = damage − armour of the target, never below 0; armour is **not** used up (user's choice).

- **Stalemate** (`FightOutcome.Stalemate`): when neither side can get past the other's armour the fight ends at once, without turns; the king stays and the path goes on. Agent's choice, accepted by the user on 2026-10-29 as the rule being tried.
- `FightClock` (`Assets/Scripts/Shared/Rpg/FightClock.cs`): order of the turns, not timed; today each fighter acts once per round, the player first. Meant for a future speed stat (more turns before the king), **not implemented**.
- `FightOutcome` (`Assets/Scripts/Shared/Rpg/FightOutcome.cs`): `PlayerWon`, `KingWon`, `Stalemate`.
- **Used by**: [RpgStatsView](#rpgstatsview).

## IFighter, PlayerStats, KingFighter

`Assets/Scripts/Shared/Rpg/IFighter.cs`: a side of a fight (name, health, max health, armour, damage, `TakeDamage` returning the health actually lost). `PlayerStats` (`Assets/Scripts/Shared/Rpg/PlayerStats.cs`): the player ("GIOCATORE"), also with `Money`; `Heal` is capped at `MaxHealth` and returns the amount healed. `KingFighter` (`Assets/Scripts/Shared/Rpg/KingFighter.cs`): a king, with fresh stats for every fight.

- **Used by**: [Fight](#fight-fightclock-fightresult-fightoutcome), [CardEffects](#cardeffects), [RpgStatsView](#rpgstatsview).

## PlayerConfig, KingConfig, RpgRules

`[Serializable]` Inspector fields of [RpgStatsView](#rpgstatsview). `PlayerConfig` (`Assets/Scripts/Shared/Rpg/PlayerConfig.cs`): `maxHealth`, `startingHealth` (lowered to the maximum), `damage`, `armor`, the toggles `healingFromCards` and `armorFromCards`; `CreateStats()`. `KingConfig` (`Assets/Scripts/Shared/Rpg/KingConfig.cs`): `health`, `armor`, `damage`, the same for every king; `CreateFighter(name)`.

`RpgRules` (`Assets/Scripts/Shared/Rpg/RpgRules.cs`): the defaults of the fields. Player: max health 100, damage 1, armour 1. King: health 10, armour 0, damage 1. `LirePerDenaro` 1000.

## Open questions

- Not yet tried by hand in Play mode (only in Edit Mode through `unity-mcp`): panel and popup readability, defeat and Reset, the Coppe and Bastoni toggles.
- Moment of the fight in the path and health between encounters: open in [Parte RPG](../business/parte-rpg.md#domande-aperte).
