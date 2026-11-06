---
tipo: info
aggiornato: 2026-11-06
verificato: 2026-11-05
fonti:
  - raw/compiled/events-impl/2026-11-06-task.md
  - raw/compiled/merge-rpg/2026-10-28-obiettivo-rpg.md
  - raw/compiled/merge-rpg/2026-10-28-fight.md
  - raw/compiled/merge-rpg/2026-10-28-popup-e-config.md
  - raw/compiled/merge-rpg/2026-10-28-riassunto-sessione.md
  - raw/compiled/santini-and-amari-first-implementation/2026-11-05-obiettivo.md
  - raw/compiled/santini-and-amari-first-implementation/2026-11-05-sessione.md
---

# RPG scripts

Scripts in `Assets/Scripts/Shared/Rpg/`, used by the scene `Assets/Scenes/TribunaRpg.unity` ([scenes](./scenes.md#tribunarpg)), and except `RpgStatsView` also by the test scene Bar-Michele ([Bar-Michele scripts](./bar-michele.md), where [BarRpgView](./bar-michele.md#barrpgview) is a copy of `RpgStatsView` that resolves card by card and makes the 7, 8 and 9 events): the first try of the RPG part, the rules of [Parte RPG](../business/parte-rpg.md#cosa-sappiamo) and the first santini and amari of [Meccaniche della run](../business/meccaniche-della-run.md#cosa-sappiamo) being tried (decisions in analysis, nothing here is fixed). At the end of each path the cards are resolved in order: values 1–9 raise a stat of the player (changed by the santini owned), a king starts a fight; between two paths the player can drink an amaro. Rules in plain C# classes (`CardEffects`, `Fight`, stats, santini, amari), view in three MonoBehaviours. Texts are ASCII only, as [GameLogFormatter](./stats.md#gamelogformatter).

| Class | Path | Kind |
|---|---|---|
| [RpgStatsView](#rpgstatsview) | `Assets/Scripts/Shared/Rpg/RpgStatsView.cs` | MonoBehaviour |
| [FightPopup](#fightpopup) | `Assets/Scripts/Shared/Rpg/FightPopup.cs` | MonoBehaviour |
| [InventoryView](#inventoryview) | `Assets/Scripts/Shared/Rpg/InventoryView.cs` | MonoBehaviour |
| [CardEffects](#cardeffects) | `Assets/Scripts/Shared/Rpg/CardEffects.cs` | static class |
| [PlayerInventory, Santino, SanFrancesco, SanMatteo](#playerinventory-santino-sanfrancesco-sanmatteo) | `Assets/Scripts/Shared/Rpg/PlayerInventory.cs`, `Assets/Scripts/Shared/Rpg/Santino.cs`, `Assets/Scripts/Shared/Rpg/SanFrancesco.cs`, `Assets/Scripts/Shared/Rpg/SanMatteo.cs` | serializable classes |
| [Amaro, HealingAmaro, DamageAmaro](#amaro-healingamaro-damageamaro) | `Assets/Scripts/Shared/Rpg/Amaro.cs`, `Assets/Scripts/Shared/Rpg/HealingAmaro.cs`, `Assets/Scripts/Shared/Rpg/DamageAmaro.cs` | serializable classes |
| [Fight, FightClock, FightResult, FightOutcome](#fight-fightclock-fightresult-fightoutcome) | `Assets/Scripts/Shared/Rpg/Fight.cs`, `Assets/Scripts/Shared/Rpg/FightClock.cs`, `Assets/Scripts/Shared/Rpg/FightResult.cs`, `Assets/Scripts/Shared/Rpg/FightOutcome.cs` | static class, plain C#, enum |
| [IFighter, PlayerStats, KingFighter](#ifighter-playerstats-kingfighter) | `Assets/Scripts/Shared/Rpg/IFighter.cs`, `Assets/Scripts/Shared/Rpg/PlayerStats.cs`, `Assets/Scripts/Shared/Rpg/KingFighter.cs` | interface, plain C# |
| [PlayerConfig, KingConfig, RpgRules](#playerconfig-kingconfig-rpgrules) | `Assets/Scripts/Shared/Rpg/PlayerConfig.cs`, `Assets/Scripts/Shared/Rpg/KingConfig.cs`, `Assets/Scripts/Shared/Rpg/RpgRules.cs` | serializable classes, constants |

## RpgStatsView

`Assets/Scripts/Shared/Rpg/RpgStatsView.cs`. The RPG part of the scene, on `UI/RpgStats` of `TribunaRpg`. Listens to [TableDeckManager](./table.md#tabledeckmanager): `GameStarted` resets the stats from `player` (so Reset starts again from the Inspector values), `PathPlayed` resolves the path, `EncounterEnded` stops the amari. Listens to [InventoryView](#inventoryview) `DrinkRequested`.

- **Resolution** (`ResolvePath`, `async UniTaskVoid` with `destroyCancellationToken`, body in `ResolveCards`): the cards of the path in order, without the bench move (`PathEnd.FilledFromBench` only fills a hole face down). A card that is not a king goes to [CardEffects](#cardeffects) with the santini of `inventory`. A king starts a [Fight](#fight-fightclock-fightresult-fightoutcome) with the stats reached so far, then the [FightPopup](#fightpopup) is awaited while `TableDeckManager.TablePaused` is on; one popup per king of the path.
- **Defeat** (`FightOutcome.KingWon`): calls `TableDeckManager.EndEncounter()` (the table ignores clicks until Reset; being inside `PathPlayed`, it also raises `EncounterEnded`) and the cards after the king are not applied.
- **Health**: the stats live for the whole encounter, so health carries over from one fight to the next; only Reset restores the Inspector values (rule being tried, [Parte RPG](../business/parte-rpg.md#domande-aperte)).
- **Amari** (`DrinkAmaro(index)`): allowed only between two paths (`CanDrink`: no path being resolved, popup included, and encounter not over). Removes the amaro from `inventory.amari`, applies it, logs it, refreshes stats and panel.
- **Inventory in Play mode**: `Update` compares the santini and amari of `inventory` (by reference) and `CanDrink` with what the [InventoryView](#inventoryview) shows and redraws it when they differ, so entries added, removed or retyped in the Inspector during Play show at once. Changing only a field of an entry (e.g. a percentage) does not redraw, but is used at the next card or drink. Reset does **not** reset `inventory`.
- **Output**: every card, turn, amaro and the stats at the end of the path in the Console; on screen `statsText` (damage, health/max, armour, Lire) and `eventsText` (last path: number of cards, or the outcome of its fights; or the last amaro drunk).
- **Serialized fields**: `tableDeckManager`, `statsText`, `eventsText`, `fightPopup`, `inventoryView` (all `[Required]`), `player` ([PlayerConfig](#playerconfig-kingconfig-rpgrules)), `king` ([KingConfig](#playerconfig-kingconfig-rpgrules)), `inventory` ([PlayerInventory](#playerinventory-santino-sanfrancesco-sanmatteo); in the scene: San Francesco, San Matteo, one Cerottino Martini, one Amaro Affilato).
- Uses `PathReceipt.CardLabel` ([PathReceipt](./hud.md#pathreceipt)) for the card names.

## FightPopup

`Assets/Scripts/Shared/Rpg/FightPopup.cs`. On `UI/FightPopup` of `TribunaRpg` (last child, full screen, inactive while hidden). `Show(title, body, cancellationToken)` (`UniTask`) sets the texts, activates the popup and completes when `closeButton` ("Continua") is clicked, then hides it. The full-screen background blocks the UI raycasts; the table cards are blocked by `TableDeckManager.TablePaused`, set by [RpgStatsView](#rpgstatsview), because `OnMouseDown` gets the click before the UI ([known issues](./known-issues.md#block-d--input)).

- **Serialized fields**: `title`, `body`, `closeButton` (an instance of `Assets/Prefab/OptionButton.prefab`).
- **Used by**: [RpgStatsView](#rpgstatsview), [BarRpgView](./bar-michele.md#barrpgview) (fight summaries, the four sevens, the shop message).

## InventoryView

`Assets/Scripts/Shared/Rpg/InventoryView.cs`. On `UI/Inventario` of `TribunaRpg`. Pure view: `Show(santini, amari, canDrink)` writes the names of the santini in `santiniText` and rebuilds one button per amaro ("Bevi <name>") under `amariContainer`, instantiated from `amaroButtonPrefab`, not interactable when `canDrink` is off. A click raises `DrinkRequested(index)` (index in the amari list); null entries are skipped.

- **Serialized fields**: `santiniText`, `amariContainer` (the `Amari` child, vertical layout), `amaroButtonPrefab` (`Assets/Prefab/OptionButton.prefab`), all `[Required]`.
- **Used by**: [RpgStatsView](#rpgstatsview) (`inventoryView`), [BarRpgView](./bar-michele.md#barrpgview) (`inventoryView`, Bar-Michele).

## CardEffects

`Assets/Scripts/Shared/Rpg/CardEffects.cs`. Rule being tried for a card of value `n` (1–9), by suit: **Spade** +n damage, **Denari** +n × `RpgRules.LirePerDenaro` (1000) Lire, **Coppe** +n health (not above the maximum), **Bastoni** +n armour. `IsKing(card)`: value `DeckRules.KingValue` ([DeckRules](./deck.md#deckrules)). `Apply(card, stats, config, santini)` changes the [PlayerStats](#ifighter-playerstats-kingfighter) and returns the log line; `config.healingFromCards` and `config.armorFromCards` off turn the Coppe and Bastoni effects off.

- **Santini**: the effect is applied `1 + Σ ExtraApplications(card)` times, and the Lire of each application are raised by `Σ LireBonusPercent(card)` %. The santini that triggered are named in the log line (e.g. `(x2 San Francesco)`). Null entries are ignored; duplicates add up.
- **Used by**: [RpgStatsView](#rpgstatsview), [BarRpgView](./bar-michele.md#barrpgview) (Bar-Michele).

## PlayerInventory, Santino, SanFrancesco, SanMatteo

`PlayerInventory` (`Assets/Scripts/Shared/Rpg/PlayerInventory.cs`): `[Serializable]` field of [RpgStatsView](#rpgstatsview) with two `[SerializeReference]` lists, `santini` and `amari`. Odin Inspector draws them with a type picker, so in Play mode entries are added, removed or changed from the Inspector (the manual test of the feature). No slot limit.

`Santino` (`Assets/Scripts/Shared/Rpg/Santino.cs`): abstract, `[Serializable]`. A passive for the whole run; each santino overrides only the hooks it needs: `Name`, `ExtraApplications(card)` (default 0), `LireBonusPercent(card)` (default 0). A new santino is a new subclass, possibly with a new hook read by [CardEffects](#cardeffects) or [Fight](#fight-fightclock-fightresult-fightoutcome).

| Santino | Class | Effect | Field |
|---|---|---|---|
| San Francesco | `SanFrancesco` (`Assets/Scripts/Shared/Rpg/SanFrancesco.cs`) | The horse (`RpgRules.HorseValue`, 9) of a path applies its effect twice, any suit | — |
| San Matteo | `SanMatteo` (`Assets/Scripts/Shared/Rpg/SanMatteo.cs`) | A Denari card gives `lireBonusPercent` % more Lire | `lireBonusPercent` (default `RpgRules.SanMatteoLireBonusPercent`, 50) |

- **Used by**: [RpgStatsView](#rpgstatsview), [CardEffects](#cardeffects), [InventoryView](#inventoryview) (names), [BarRpgView](./bar-michele.md#barrpgview) (Bar-Michele).

## Amaro, HealingAmaro, DamageAmaro

`Amaro` (`Assets/Scripts/Shared/Rpg/Amaro.cs`): abstract, `[Serializable]`. A consumable, drunk between two paths from the [InventoryView](#inventoryview) and removed from the inventory: `Name`, `Drink(stats)` applies it and returns the log line.

| Amaro | Class | Effect | Field |
|---|---|---|---|
| Cerottino Martini | `HealingAmaro` (`Assets/Scripts/Shared/Rpg/HealingAmaro.cs`) | Heals `healthPercent` % of the maximum health (at least 1), not above the maximum | `healthPercent` (default `RpgRules.HealingAmaroPercent`, 25) |
| Amaro Affilato | `DamageAmaro` (`Assets/Scripts/Shared/Rpg/DamageAmaro.cs`) | +`damageBonus` damage for the rest of the run (until Reset). Test amaro, placeholder name | `damageBonus` (default `RpgRules.DamageAmaroBonus`, 5) |

- **Used by**: [RpgStatsView](#rpgstatsview), [InventoryView](#inventoryview) (names), [BarRpgView](./bar-michele.md#barrpgview) (Bar-Michele).

## Fight, FightClock, FightResult, FightOutcome

`Assets/Scripts/Shared/Rpg/Fight.cs`. `Run(player, king)` plays the whole fight at once and returns a `FightResult` (outcome, one line per turn, damage done by each side); the player's health stays as the fight leaves it. `HitDamage(attacker, target)` = damage − armour of the target, never below 0; armour is **not** used up (user's choice).

- **Stalemate** (`FightOutcome.Stalemate`): when neither side can get past the other's armour the fight ends at once, without turns; the king stays and the path goes on. Agent's choice, accepted by the user on 2026-10-29 as the rule being tried.
- `FightClock` (`Assets/Scripts/Shared/Rpg/FightClock.cs`): order of the turns, not timed; today each fighter acts once per round, the player first. Meant for a future speed stat (more turns before the king), **not implemented**.
- `FightOutcome` (`Assets/Scripts/Shared/Rpg/FightOutcome.cs`): `PlayerWon`, `KingWon`, `Stalemate`.
- **Used by**: [RpgStatsView](#rpgstatsview), [BarRpgView](./bar-michele.md#barrpgview) (Bar-Michele).

## IFighter, PlayerStats, KingFighter

`Assets/Scripts/Shared/Rpg/IFighter.cs`: a side of a fight (name, health, max health, armour, damage, `TakeDamage` returning the health actually lost). `PlayerStats` (`Assets/Scripts/Shared/Rpg/PlayerStats.cs`): the player ("GIOCATORE"), also with `Money`; `Heal` is capped at `MaxHealth` and returns the amount healed. `KingFighter` (`Assets/Scripts/Shared/Rpg/KingFighter.cs`): a king, with fresh stats for every fight.

- **Used by**: [Fight](#fight-fightclock-fightresult-fightoutcome), [CardEffects](#cardeffects), [RpgStatsView](#rpgstatsview), the amari ([Amaro](#amaro-healingamaro-damageamaro)), [BarRpgView](./bar-michele.md#barrpgview) (Bar-Michele).

## PlayerConfig, KingConfig, RpgRules

`[Serializable]` Inspector fields of [RpgStatsView](#rpgstatsview) and [BarRpgView](./bar-michele.md#barrpgview). `PlayerConfig` (`Assets/Scripts/Shared/Rpg/PlayerConfig.cs`): `maxHealth`, `startingHealth` (lowered to the maximum), `damage`, `armor`, the toggles `healingFromCards` and `armorFromCards`; `CreateStats()`. `KingConfig` (`Assets/Scripts/Shared/Rpg/KingConfig.cs`): `health`, `armor`, `damage`, the same for every king; `CreateFighter(name)`.

`RpgRules` (`Assets/Scripts/Shared/Rpg/RpgRules.cs`): the defaults of the fields. Player: max health 100, damage 1, armour 1. King: health 10, armour 0, damage 1. `LirePerDenaro` 1000. Santini and amari: `HorseValue` 9 (also the horse of [BarEvents](./bar-michele.md#barevents)), `SanMatteoLireBonusPercent` 50, `HealingAmaroPercent` 25, `DamageAmaroBonus` 5.

## Open questions

- Not yet tried by hand in Play mode (only in Edit Mode through `unity-mcp`): panel and popup readability, defeat and Reset, the Coppe and Bastoni toggles. Santini and amari were tried by the user in Play mode on 2026-11-05.
- The `Inventario` panel is as tall as `RpgStats` and ends just below the last row of cards; no problem in the user's test (its background blocks the clicks on the table, [known issues](./known-issues.md#block-d--input)).
- Moment of the fight in the path and health between encounters: open in [Parte RPG](../business/parte-rpg.md#domande-aperte). Slots for santini and amari, moment to drink an amaro: open in [Meccaniche della run](../business/meccaniche-della-run.md#domande-aperte).
