---
tipo: info
aggiornato: 2026-10-11
verificato: 2026-10-11
fonti:
  - raw/compiled/panchina-start/2026-10-11-obiettivo.md
  - raw/compiled/panchina-start/2026-10-11-fix-path.md
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Panchina scripts

Scripts in `Assets/Scripts/Panchina/`, used by the scene `Assets/Scenes/Panchina.unity`: the POC of the **panchina** idea of [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). `DeckManagerPanchina`, `CardControllerPanchina` and `CardAnimationsPanchina` started as **copies** of [shared](./shared.md) classes with a `Panchina` suffix; the table state and the path rule are Panchina's own ([PanchinaTable](#panchinatable)); the shared [CardData](./shared.md#carddata), [CardDatabase](./shared.md#carddatabase), [CardDataInGame](./shared.md#carddataingame), [CardDisplay3D](./shared.md#carddisplay3d) and [GameManager](./shared.md#gamemanager) are reused. Issues: [known-issues](./known-issues.md).

| Class | Path | Copy of |
|---|---|---|
| [PanchinaTable](#panchinatable) | `Assets/Scripts/Panchina/PanchinaTable.cs` | — (plain C# model, with `PathMove` and `PathEnd`) |
| [DeckManagerPanchina](#deckmanagerpanchina) | `Assets/Scripts/Panchina/DeckManager.cs` | [DeckManager3D](./shared.md#deckmanager3d) |
| [CardControllerPanchina](#cardcontrollerpanchina) | `Assets/Scripts/Panchina/CardController.cs` | [CardController](./shared.md#cardcontroller) |
| [CardAnimationsPanchina](#cardanimationspanchina) | `Assets/Scripts/Panchina/CardAnimations.cs` | [CardAnimations](./shared.md#cardanimations) |
| [CardSlot](#cardslot) | `Assets/Scripts/Panchina/CardSlot.cs` | — |
| [CardSlotContainer](#cardslotcontainer) | `Assets/Scripts/Panchina/CardSlotContainer.cs` | — |

## PanchinaTable

`Assets/Scripts/Panchina/PanchinaTable.cs`. Plain C# **logical state of the table** (no MonoBehaviour, no DOTween): for each of the 40 slots a stack of [CardDataInGame](./shared.md#carddataingame) (duplicates end up one on top of the other; an empty slot is a **hole**) and whether it is face up. The scene mirrors it; rules read it, never the hierarchy.

- Built by [DeckManagerPanchina](#deckmanagerpanchina) after dealing: slot `i` holds the card dealt there, face down.
- `GetCard(slot)` (top card, null for a hole), `StackSize`, `IsHole`, `IsFaceUp`, `SlotOf(card)` (by reference: duplicates are distinct instances).
- `PlayPath(startSlot)` applies a path to the state and returns its `PathMove`s (card, target slot, position in the stack, `PathEnd`). Rule being tried (POC, decision [in analisi](../business/modifica-del-mazzo.md)):
  - the card in hand goes face up to the slot of its `index`; the card there becomes the card in hand;
  - **closed** (`PathEnd.Closed`): the card in hand belongs to the starting slot and fills it;
  - **stacked on a duplicate** (`PathEnd.StackedOnDuplicate`): the slot of the card in hand is already face up, so a card with the same index is there; the card goes on top of it, and the starting slot stays a **hole**. Filling holes from the panchina is not implemented.
- **Used by**: [DeckManagerPanchina](#deckmanagerpanchina).

## DeckManagerPanchina

`Assets/Scripts/Panchina/DeckManager.cs`. Game setup and view of the Panchina scene. In `Start`:

1. sets the singleton `instance`, blocks clicks, builds `cardDatabase` / `cardDatabaseInGame` from `deckDatabase` ([CardDatabase](./shared.md#carddatabase));
2. builds the deck: the 36 non-king cards, plus **duplicates** of the first 9 cards of the database (ace to nine of Denari, always the same; each a **new** `CardDataInGame` instance), shuffled and cut to 36, plus the 4 kings (`valore == 10`), shuffled again (40 cards). Duplicates and therefore missing cards are **intended test cases**: the panchina exists to handle them;
3. instantiates `cardPrefab` as a pile, deals each card to `SlotPosition(i)` and makes it a child of slot `i`;
4. builds the [PanchinaTable](#panchinatable) from the dealt cards and a map card → `CardDisplay3D`, unblocks clicks.

- `PlayPath(clickedCard)` (`UniTask`): ignores face-up cards; asks the table for the moves and animates them one by one with [CardAnimationsPanchina](#cardanimationspanchina) (a card on a duplicate is raised by `StackOffset` per card below it), then re-parents each moved card to its slot, so the hierarchy follows the table. Cancelled if the manager is destroyed.
- `SlotPosition(slot)` (static): grid position of a slot, 4 rows of 10.
- **Serialized fields**: `cardPrefab` (`Assets/Prefab/3DCardPanchina.prefab`), `deckPosition` (its child 0 must be the slot container, see [scenes](./scenes.md#panchina)), `deckDatabase` (`Assets/Instances/CardDatabase.asset`; if empty, `Start` throws), `dealMoveDuration` (seconds per card when dealing, default 0.1), `pathMoveDuration` (seconds per flip or move in a path, default 0.5). In the scene YAML the component still has the old class identifier `DeckManager3DTest`.
- `GetSlotByIndex(index)`: `deckPosition.GetChild(0).GetChild(index)`, the slot Transform by child order; used only to parent cards, not to read the state.
- **Static state**: `instance`, `cardDatabase`, `cardDatabaseInGame`, `shuffledCardDatabaseInGame`, `deckCards`. The first two are rebuilt at every `Start`; the others are never cleared or reset.
- `DECK_LIMIT = 40` (field, not `const`), `9` duplicates and `10` for the king are hard-coded.
- **Used by**: [CardControllerPanchina](#cardcontrollerpanchina) (`instance.PlayPath`).

## CardControllerPanchina

`Assets/Scripts/Panchina/CardController.cs`. On `OnMouseDown`, when clicks are not blocked ([GameManager](./shared.md#gamemanager)), blocks them, runs `DeckManagerPanchina.instance.PlayPath(card)` and unblocks them in a `finally`.

- **Prefab**: `Assets/Prefab/3DCardPanchina.prefab` (with [CardDisplay3D](./shared.md#carddisplay3d)).

## CardAnimationsPanchina

`Assets/Scripts/Panchina/CardAnimations.cs`. Static DOTween helpers for the Panchina scene:

- `MoveAnimation(card, target, duration)`: lift and move; used to deal.
- `PathMoveAnimation(card, displaced, target, liftCard, duration)`: one move of a path, same look as the first POC: the clicked card is lifted (first move only), the card flips face up, the displaced card (if any) is lifted, then the card moves to the target. `SetLink` on the card.
- `FlipAnimation`: unused.
- **Used by**: [DeckManagerPanchina](#deckmanagerpanchina).

## CardSlot

`Assets/Scripts/Panchina/CardSlot.cs`. `int index` on each of the 40 slots (`Assets/Prefab/CardSlot.prefab` instances in the Panchina scene). **Never read by the code**: slots are found by child order. The scene values are not in child order, by choice (test data).

## CardSlotContainer

`Assets/Scripts/Panchina/CardSlotContainer.cs`. `List<GameObject> CardSlotList` on the slot container of the Panchina scene. **Never read by the code.**
