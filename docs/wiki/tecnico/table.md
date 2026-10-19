---
tipo: info
aggiornato: 2026-10-19
verificato: 2026-10-19
consolidato: 2026-10-19
fonti:
  - raw/compiled/preset-builder/2026-10-19-obiettivo.md
  - raw/compiled/demo-1/2026-10-16-opzioni.md
  - raw/compiled/demo-1/2026-10-16-obiettivo.md
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/index40/2026-10-16-obiettivo.md
  - raw/compiled/stats/2026-10-16-sessione.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/tribuna/2026-10-15-implementazione.md
  - raw/compiled/tribuna/2026-10-15-obiettivo.md
  - raw/compiled/panchina-reprise/2026-10-15-implementazione.md
  - raw/compiled/panchina-reprise/2026-10-15-obiettivo.md
  - raw/compiled/panchina-continue/2026-10-15-implementazione.md
  - raw/compiled/unity-mcp/2026-10-15-test-e-controllo.md
  - raw/compiled/panchina-start/2026-10-11-obiettivo.md
  - raw/compiled/panchina-start/2026-10-11-fix-path.md
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Table scripts

Scripts in `Assets/Scripts/Shared/Table/`, used by the scenes `Assets/Scenes/Panchina.unity`, `Assets/Scenes/Tribuna.unity` and `Assets/Scenes/Index40.unity`: the POCs of the **panchina**, **tribuna** and **index>40** ideas of [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). One code for the three scenes: they differ in `TableDeckManager.excessCards` ([ExcessCards](./deck.md#excesscards): `Bench`, `Discard`, `Index`), the preset, and in Index40 the 80 slots and the camera. Options of the scenes (preset of the next game, animation speed) in [OptionsPanel](#optionspanel). The shared [CardData](./shared.md#carddata), [CardDatabase](./shared.md#carddatabase), [CardDataInGame](./shared.md#carddataingame), [CardDisplay3D](./shared.md#carddisplay3d) and [GameManager](./shared.md#gamemanager) are reused. Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [TableState](#tablestate) | `Assets/Scripts/Shared/Table/TableState.cs` | plain C# model, with `PathMove` and `PathEnd` |
| [TableDeckManager](#tabledeckmanager) | `Assets/Scripts/Shared/Table/TableDeckManager.cs` | MonoBehaviour |
| [AnimationSpeed, AnimationTiming](#animationspeed-animationtiming) | `Assets/Scripts/Shared/Table/AnimationSpeed.cs`, `Assets/Scripts/Shared/Table/AnimationTiming.cs` | enum, serializable class |
| [OptionsPanel](#optionspanel) | `Assets/Scripts/Shared/Table/OptionsPanel.cs` | MonoBehaviour |
| [TableCardController](#tablecardcontroller) | `Assets/Scripts/Shared/Table/TableCardController.cs` | MonoBehaviour |
| [TableCardAnimations](#tablecardanimations) | `Assets/Scripts/Shared/Table/TableCardAnimations.cs` | static class (DOTween) |
| [CardSlot](#cardslot) | `Assets/Scripts/Shared/Table/CardSlot.cs` | MonoBehaviour |
| [CardSlotContainer](#cardslotcontainer) | `Assets/Scripts/Shared/Table/CardSlotContainer.cs` | MonoBehaviour |

## TableState

`Assets/Scripts/Shared/Table/TableState.cs`. Plain C# **logical state of the table** (no MonoBehaviour, no DOTween): for each slot (40, up to 80 in index>40) a stack of [CardDataInGame](./shared.md#carddataingame) (duplicates end up one on top of the other; an empty slot is a **hole**) and whether it is face up, plus the **bench** (panchina): a queue of the cards beyond the table, empty in the tribuna and in index>40. The scene mirrors it; rules read it, never the hierarchy.

- Built by [TableDeckManager](#tabledeckmanager) after dealing, from the dealt cards (slot `i` holds the card dealt there, face down; `null` leaves it a hole, for decks under 40 cards) and the bench cards (drawn in order).
- `GetCard(slot)` (top card, null for a hole), `StackSize`, `IsHole`, `IsFaceUp`, `SlotOf(card)` (by reference: duplicates are distinct instances; bench cards are not found), `BenchCount`, `FaceUpKings` (kings in face-up slots; a king is never covered, kings have no duplicates).
- `PlayPath(startSlot)` applies a path to the state and returns its `PathMove`s (card, target slot, position in the stack, `PathEnd`). Rule being tried (POC, decision [in analisi](../business/modifica-del-mazzo.md)):
  - the card in hand goes face up to the slot of its `index`; the card there becomes the card in hand;
  - **closed** (`PathEnd.Closed`): the card in hand belongs to the starting slot and fills it;
  - **stacked on a duplicate** (`PathEnd.StackedOnDuplicate`): the slot of the card in hand is already face up, so a card with the same index is there; the card goes on top of it, and the starting slot stays a **hole**;
  - **filled a hole** (`PathEnd.FilledHole`): the slot of the card in hand is a hole; the card goes face up into it, and the starting slot becomes a new hole.
  - Checks in this order: starting slot, hole, face-up slot, normal move.
  - **filled from the bench** (`PathEnd.FilledFromBench`): when the path stopped, the starting slot (a hole) gets the next bench card **face down**, as an extra last move; with the bench empty (always, in the tribuna) the hole stays. While the bench has cards every hole is filled at once, so "filled a hole" happens only with the bench empty.
- **Used by**: [TableDeckManager](#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator) (autoplay).

## TableDeckManager

`Assets/Scripts/Shared/Table/TableDeckManager.cs`. Game setup and view of the Panchina and Tribuna scenes. In `Start`:

1. sets the singleton `instance`, blocks clicks, draws the game seed (`seed`, or a random one when 0);
2. builds the deck with `DeckBuilder.Build(excessCards, …)` ([DeckBuilder](./deck.md#deckbuilder)) from `deckDatabase` ([CardDatabase](./shared.md#carddatabase)) and `deckPreset` ([DeckPreset](./deck.md#deckpreset)). `Bench` and `Discard`: 40 table slots with the 4 kings, holes in the last slots when the deck is under 40 cards; the excess goes to the **bench** (`Bench`) or is thrown away, never instantiated (`Discard`). `Index`: one slot per card index (40 + duplicates), holes at the indices of the removed cards, no excess. Duplicates and missing cards on the table are **intended test cases**;
3. clears the [game log](./stats.md#gamelogview) and logs the deal ([GameLogFormatter](./stats.md#gamelogformatter): preset, seed, removed cards, duplicates) in the Console and on screen;
4. shows the slots of the deal and hides the others (`ShowSlots`: Index40 has 80, a smaller deck uses fewer); instantiates `cardPrefab` as one pile at `deckPosition`, table cards on top and bench below, with a map card → `CardDisplay3D`; deals every table card to `SlotPosition(slot)`, each a child of its slot (holes get none); the bench cards stay face down in the pile;
5. builds the [TableState](#tablestate) from the table slots and bench cards, unblocks clicks.

`Start` runs these steps as an `async UniTaskVoid` with `.Forget()`; dealing (`MoveCards`, `UniTask`) is cancelled by `destroyCancellationToken` (`TweenCancelBehaviour.KillAndCancelAwait`: the await stops, no destroyed card is touched), and a cancellation is not logged as an error.

- `Excess` ([ExcessCards](./deck.md#excesscards)) and `DeckDatabase`: read by [TableSimulationRunner](./stats.md#tablesimulationrunner).
- `PlayPath(clickedCard)` (`UniTask`): ignores face-up cards and the bench pile; asks the table for the moves, logs the path (length, cards, kings face up; once all 4 kings are face up, a line with the path number), then animates the moves one by one with [TableCardAnimations](#tablecardanimations) (`PathMoveAnimation`; a card on a duplicate is raised by `StackOffset` per card below it; a bench card uses `MoveAnimation`, face down, from the pile to the hole) and re-parents each moved card to its slot, so the hierarchy follows the table. Cancelled by the manager's `destroyCancellationToken`.
- `ResetTable()`: called by the `ResetButton` of each scene ([scenes](./scenes.md#panchina-tribuna-and-index40)); destroys every card and runs the setup again (new seed when `seed` is 0), without leaving Play mode. Ignored while clicks are blocked (dealing or a path running), so no card is destroyed mid-animation.
- `SlotPosition(slot)` (static): grid position of a slot, rows of `DeckRules.SlotsPerRow` (10): 4 rows, up to 8 in index>40.
- **Serialized fields**: `cardPrefab` (`Assets/Prefab/3DCardTable.prefab`), `deckPosition` (its child 0 must be the slot container, see [scenes](./scenes.md#panchina-tribuna-and-index40)), `deckDatabase` (`Assets/Instances/CardDatabase.asset`), `deckPreset` (preset of the **next** game: `Preset32` in Panchina, `Preset71` in Tribuna, `Preset45` in Index40; the game in progress keeps its own, `CurrentPreset`, until Reset), `seed` (0 = random; set it to the seed of a log or CSV row to replay that deal), `excessCards` (`Bench` in Panchina, `Discard` in Tribuna, `Index` in Index40), `gameLog` (optional, the `GameLog` panel), `animationSpeed` ([AnimationSpeed](#animationspeed-animationtiming), `Fast` in the three scenes) and `slowTiming` / `normalTiming` / `fastTiming` (durations of each level; defaults 0.2/0.5, 0.1/0.2, 0.03/0.05 seconds per dealt card / path move; the next animation uses the current level). In the Panchina scene YAML the component still has the old class identifier `DeckManager3DTest`.
- `GetSlotByIndex(index)`: `deckPosition.GetChild(0).GetChild(index)`, the slot Transform by child order; used only to parent cards, not to read the state.
- **Static state**: only `instance`; the card lists are instance fields. A script recompile during Play nulls `instance`: clicks then throw until Play is restarted.
- **Constants**: deck and table numbers in [DeckRules](./deck.md#deckrules), `StackOffset`; the grid layout in `SlotPosition` is still literal numbers.
- `SetNextPreset(preset)`, `NextPreset`, `CurrentPreset`, `SetSpeed(speed)`, `Speed`: used by [OptionsPanel](#optionspanel).
- **Used by**: [TableCardController](#tablecardcontroller) (`instance.PlayPath`), the `ResetButton` of the three scenes (`ResetTable`, persistent `OnClick`), [OptionsPanel](#optionspanel) (`tableDeckManager`), [TableSimulationRunner](./stats.md#tablesimulationrunner) (`tableDeckManager`).

## AnimationSpeed, AnimationTiming

`Assets/Scripts/Shared/Table/AnimationSpeed.cs`: the three speed levels of the card animations, `Slow`, `Normal`, `Fast` ("Piano", "Normale", "Veloce" in the options panel). `Assets/Scripts/Shared/Table/AnimationTiming.cs`: `dealMoveDuration` and `pathMoveDuration` of one level, serialized three times on [TableDeckManager](#tabledeckmanager).

- **Used by**: [TableDeckManager](#tabledeckmanager), [OptionsPanel](#optionspanel).

## OptionsPanel

`Assets/Scripts/Shared/Table/OptionsPanel.cs`. Options of the table scenes. On the root of `Assets/Prefab/Options.prefab`, which stays active; `Toggle()` (persistent `OnClick` of the prefab's `OptionsButton`) shows or hides its `panel`. The first time it opens it builds, from `optionButtonPrefab` (`Assets/Prefab/OptionButton.prefab`), a button per preset of `presetLibrary` ([DeckPresetLibrary](./deck.md#deckpresetlibrary): the standard and custom presets, label with the cards; in a scrollable list) and per [AnimationSpeed](#animationspeed-animationtiming) level:

- a preset sets the preset of the **next** game (`TableDeckManager.SetNextPreset`, used from the next Reset); the game in progress continues;
- a speed level applies at once (`TableDeckManager.SetSpeed`);
- the chosen options are shown as not interactable; `presetStatus` shows the preset in play and the next one.
- **Serialized fields**: `tableDeckManager` (scene override), `panel`, `optionButtonPrefab`, `presetButtons` (the content of the preset `ScrollRect`, see [scenes](./scenes.md#prefabs)), `speedButtons`, `presetStatus`, `presetLibrary` (`Assets/Instances/DeckPresetLibrary.asset`).
- The choice lasts while the scene is open: the next load of the scene starts from its own preset and speed.

## TableCardController

`Assets/Scripts/Shared/Table/TableCardController.cs`. On `OnMouseDown`, unless the pointer is over the UI (`EventSystem.IsPointerOverGameObject`: `OnMouseDown` ignores the UI, so without it a click on the options panel played the card below), and when clicks are not blocked ([GameManager](./shared.md#gamemanager)), blocks them, runs `TableDeckManager.instance.PlayPath(card)` and unblocks them in a `finally`. `OnMouseDown` calls an `async UniTaskVoid` method with `.Forget()`.

- **Prefab**: `Assets/Prefab/3DCardTable.prefab` (with [CardDisplay3D](./shared.md#carddisplay3d)).

## TableCardAnimations

`Assets/Scripts/Shared/Table/TableCardAnimations.cs`. Static DOTween helpers for the table scenes:

- `MoveAnimation(card, target, duration)`: lift and move; used to deal and to bring a bench card into a hole.
- `PathMoveAnimation(card, displaced, target, liftCard, duration)`: one move of a path: the clicked card is lifted (first move only), the card flips face up, the displaced card (if any) is lifted, then the card moves to the target. `SetLink` on the card.
- **Used by**: [TableDeckManager](#tabledeckmanager).

## CardSlot

`Assets/Scripts/Shared/Table/CardSlot.cs`. `int index` on each of the 40 slots of Panchina and Tribuna (`Assets/Prefab/CardSlot.prefab` instances; the component is on the instances, not on the prefab). **Never read by the code**: slots are found by child order. The scene values are not in child order, by choice (test data); Tribuna's slots are a copy of Panchina's. Index40's 80 slot instances have no `CardSlot` component.

## CardSlotContainer

`Assets/Scripts/Shared/Table/CardSlotContainer.cs`. `List<GameObject> CardSlotList` on the slot container of Panchina and Tribuna (not in Index40). **Never read by the code.**
