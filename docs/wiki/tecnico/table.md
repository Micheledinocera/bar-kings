---
tipo: info
aggiornato: 2026-10-15
verificato: 2026-10-15
consolidato: 2026-10-15
fonti:
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

Scripts in `Assets/Scripts/Shared/Table/`, used by the scenes `Assets/Scenes/Panchina.unity` and `Assets/Scenes/ScenaTribuna.unity`: the POCs of the **panchina** and **tribuna** ideas of [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). One code for both scenes: the only difference is `TableDeckManager.excessCards` (`Bench` for the panchina, `Discard` for the tribuna). `TableDeckManager`, `TableCardController` and `TableCardAnimations` started as **copies** of the first-POC [shared](./shared.md) classes (`DeckManager3D`, `CardController`, `CardAnimations`, still used by `SampleScene` and `ScenaIndex40`); the shared [CardData](./shared.md#carddata), [CardDatabase](./shared.md#carddatabase), [CardDataInGame](./shared.md#carddataingame), [CardDisplay3D](./shared.md#carddisplay3d) and [GameManager](./shared.md#gamemanager) are reused. Issues: [known-issues](./known-issues.md).

| Class | Path | Copy of |
|---|---|---|
| [TableState](#tablestate) | `Assets/Scripts/Shared/Table/TableState.cs` | — (plain C# model, with `PathMove` and `PathEnd`) |
| [TableDeckManager](#tabledeckmanager) | `Assets/Scripts/Shared/Table/TableDeckManager.cs` | [DeckManager3D](./shared.md#deckmanager3d) |
| [TableCardController](#tablecardcontroller) | `Assets/Scripts/Shared/Table/TableCardController.cs` | [CardController](./shared.md#cardcontroller) |
| [TableCardAnimations](#tablecardanimations) | `Assets/Scripts/Shared/Table/TableCardAnimations.cs` | [CardAnimations](./shared.md#cardanimations) |
| [CardSlot](#cardslot) | `Assets/Scripts/Shared/Table/CardSlot.cs` | — |
| [CardSlotContainer](#cardslotcontainer) | `Assets/Scripts/Shared/Table/CardSlotContainer.cs` | — |

## TableState

`Assets/Scripts/Shared/Table/TableState.cs`. Plain C# **logical state of the table** (no MonoBehaviour, no DOTween): for each of the 40 slots a stack of [CardDataInGame](./shared.md#carddataingame) (duplicates end up one on top of the other; an empty slot is a **hole**) and whether it is face up, plus the **bench** (panchina): a queue of the cards beyond the table, empty in the tribuna. The scene mirrors it; rules read it, never the hierarchy.

- Built by [TableDeckManager](#tabledeckmanager) after dealing, from the dealt cards (slot `i` holds the card dealt there, face down) and the bench cards (drawn in order).
- `GetCard(slot)` (top card, null for a hole), `StackSize`, `IsHole`, `IsFaceUp`, `SlotOf(card)` (by reference: duplicates are distinct instances; bench cards are not found), `BenchCount`.
- `PlayPath(startSlot)` applies a path to the state and returns its `PathMove`s (card, target slot, position in the stack, `PathEnd`). Rule being tried (POC, decision [in analisi](../business/modifica-del-mazzo.md)):
  - the card in hand goes face up to the slot of its `index`; the card there becomes the card in hand;
  - **closed** (`PathEnd.Closed`): the card in hand belongs to the starting slot and fills it;
  - **stacked on a duplicate** (`PathEnd.StackedOnDuplicate`): the slot of the card in hand is already face up, so a card with the same index is there; the card goes on top of it, and the starting slot stays a **hole**;
  - **filled a hole** (`PathEnd.FilledHole`): the slot of the card in hand is a hole; the card goes face up into it, and the starting slot becomes a new hole.
  - Checks in this order: starting slot, hole, face-up slot, normal move.
  - **filled from the bench** (`PathEnd.FilledFromBench`): when the path stopped, the starting slot (a hole) gets the next bench card **face down**, as an extra last move; with the bench empty (always, in the tribuna) the hole stays. While the bench has cards every hole is filled at once, so "filled a hole" happens only with the bench empty.
- **Used by**: [TableDeckManager](#tabledeckmanager).

## TableDeckManager

`Assets/Scripts/Shared/Table/TableDeckManager.cs`. Game setup and view of the Panchina and Tribuna scenes. In `Start`:

1. sets the singleton `instance`, blocks clicks, builds `cardDatabase` / `cardDatabaseInGame` from `deckDatabase` ([CardDatabase](./shared.md#carddatabase));
2. clears its static lists, checks that the database has exactly `KingCount` (4) kings (`valore == KingValue`, 10; otherwise throws), then builds the deck: the 36 non-king cards plus `duplicateCount` **duplicates** of the first non-king cards (default 9: ace to nine of Denari, always the same, never kings; each a **new** `CardDataInGame` instance), shuffled; the first 36 go to the table with the 4 kings, shuffled again (`DECK_LIMIT`, 40 cards). The cards in excess follow `excessCards`: `Bench` → the **bench** (`benchCards`); `Discard` → thrown away, never instantiated. Duplicates and missing cards on the table are **intended test cases**;
3. instantiates `cardPrefab` as one pile at `deckPosition`, table cards on top and bench below; deals the 40 table cards to `SlotPosition(i)`, each a child of slot `i`; the bench cards stay face down in the pile;
4. builds the [TableState](#tablestate) from the table and bench cards and a map card → `CardDisplay3D`, unblocks clicks.

`Start` runs these steps as an `async UniTaskVoid` with `.Forget()`; dealing (`MoveCards`, `UniTask`) is cancelled by `destroyCancellationToken`, and a cancellation is not logged as an error.

- `ExcessCards` (nested enum): `Bench` (panchina), `Discard` (tribuna).
- `PlayPath(clickedCard)` (`UniTask`): ignores face-up cards and the bench pile; asks the table for the moves and animates them one by one with [TableCardAnimations](#tablecardanimations) (`PathMoveAnimation`; a card on a duplicate is raised by `StackOffset` per card below it; a bench card uses `MoveAnimation`, face down, from the pile to the hole), then re-parents each moved card to its slot, so the hierarchy follows the table. Cancelled by the manager's `destroyCancellationToken`.
- `ResetTable()`: called by the `ResetButton` of each scene ([scenes](./scenes.md#panchina-and-scenatribuna)); destroys every card and runs the setup again, without leaving Play mode. Ignored while clicks are blocked (dealing or a path running), so no card is destroyed mid-animation.
- `SlotPosition(slot)` (static): grid position of a slot, 4 rows of 10.
- **Serialized fields**: `cardPrefab` (`Assets/Prefab/3DCardTable.prefab`), `deckPosition` (its child 0 must be the slot container, see [scenes](./scenes.md#panchina-and-scenatribuna)), `deckDatabase` (`Assets/Instances/CardDatabase.asset`; if empty, `Start` throws), `excessCards` (`Bench` in Panchina, `Discard` in ScenaTribuna), `duplicateCount` (0–36, default 9), `dealMoveDuration` (seconds per card when dealing, default 0.1), `pathMoveDuration` (seconds per flip or move in a path, default 0.5). In the Panchina scene YAML the component still has the old class identifier `DeckManager3DTest`.
- `GetSlotByIndex(index)`: `deckPosition.GetChild(0).GetChild(index)`, the slot Transform by child order; used only to parent cards, not to read the state.
- **Static state**: `instance`, `cardDatabase`, `cardDatabaseInGame`, `shuffledCardDatabaseInGame` (the 40 table cards), `deckCards` (all card GameObjects, table first). All but `instance` are rebuilt or cleared at every setup; none is reset when entering Play mode without domain reload. A script recompile during Play nulls `instance`: clicks then throw until Play is restarted.
- **Constants**: `DECK_LIMIT = 40` (table cards), `KingValue = 10`, `KingCount = 4`, `StackOffset`; the grid layout in `SlotPosition` is still literal numbers.
- **Used by**: [TableCardController](#tablecardcontroller) (`instance.PlayPath`), the `ResetButton` of both scenes (`ResetTable`, persistent `OnClick`).

## TableCardController

`Assets/Scripts/Shared/Table/TableCardController.cs`. On `OnMouseDown`, when clicks are not blocked ([GameManager](./shared.md#gamemanager)), blocks them, runs `TableDeckManager.instance.PlayPath(card)` and unblocks them in a `finally`. `OnMouseDown` calls an `async UniTaskVoid` method with `.Forget()`.

- **Prefab**: `Assets/Prefab/3DCardTable.prefab` (with [CardDisplay3D](./shared.md#carddisplay3d)).

## TableCardAnimations

`Assets/Scripts/Shared/Table/TableCardAnimations.cs`. Static DOTween helpers for the table scenes:

- `MoveAnimation(card, target, duration)`: lift and move; used to deal and to bring a bench card into a hole.
- `PathMoveAnimation(card, displaced, target, liftCard, duration)`: one move of a path, same look as the first POC: the clicked card is lifted (first move only), the card flips face up, the displaced card (if any) is lifted, then the card moves to the target. `SetLink` on the card.
- **Used by**: [TableDeckManager](#tabledeckmanager).

## CardSlot

`Assets/Scripts/Shared/Table/CardSlot.cs`. `int index` on each of the 40 slots (`Assets/Prefab/CardSlot.prefab` instances in both scenes; the component is on the instances, not on the prefab). **Never read by the code**: slots are found by child order. The scene values are not in child order, by choice (test data); ScenaTribuna's slots are a copy of Panchina's.

## CardSlotContainer

`Assets/Scripts/Shared/Table/CardSlotContainer.cs`. `List<GameObject> CardSlotList` on the slot container of both scenes. **Never read by the code.**
