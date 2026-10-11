---
tipo: info
aggiornato: 2026-10-11
verificato: 2026-10-11
fonti:
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Panchina scripts

Scripts in `Assets/Scripts/Panchina/`, used by the scene `Assets/Scenes/Panchina.unity`: the POC of the **panchina** idea of [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). Four of them are **copies** of [shared](./shared.md) classes with a `Panchina` suffix; the shared [CardData](./shared.md#carddata), [CardDataInGame](./shared.md#carddataingame), [CardDisplay3D](./shared.md#carddisplay3d) and [GameManager](./shared.md#gamemanager) are reused. Issues: [known-issues](./known-issues.md).

| Class | Path | Copy of |
|---|---|---|
| [DeckManagerPanchina](#deckmanagerpanchina) | `Assets/Scripts/Panchina/DeckManager.cs` | [DeckManager3D](./shared.md#deckmanager3d) |
| [CardControllerPanchina](#cardcontrollerpanchina) | `Assets/Scripts/Panchina/CardController.cs` | [CardController](./shared.md#cardcontroller) |
| [CardAnimationsPanchina](#cardanimationspanchina) | `Assets/Scripts/Panchina/CardAnimations.cs` | [CardAnimations](./shared.md#cardanimations) |
| [CardGeneratorPanchina](#cardgeneratorpanchina) | `Assets/Scripts/Panchina/CardGenerator.cs` | [CardGenerator](./shared.md#cardgenerator) |
| [CardSlot](#cardslot) | `Assets/Scripts/Panchina/CardSlot.cs` | — |
| [CardSlotContainer](#cardslotcontainer) | `Assets/Scripts/Panchina/CardSlotContainer.cs` | — |

## DeckManagerPanchina

`Assets/Scripts/Panchina/DeckManager.cs`. Game setup of the Panchina scene, in `Start`:

1. sets the singleton `instance`, blocks clicks, calls [CardGeneratorPanchina](#cardgeneratorpanchina);
2. builds the deck: the 36 non-king cards, plus **duplicates** of the first 9 cards of the database (ace to nine of Denari, always the same), shuffled and cut to 36, plus the 4 kings (`valore == 10`), shuffled again (40 cards). Duplicates and therefore missing cards are **intended test cases**: the panchina exists to handle them;
3. instantiates `cardPrefab` as a pile, deals each card to the 4×10 grid and makes it a **child of slot `i`** (`GetSlotByIndex(i)`), unblocks clicks.

- **Serialized fields**: `cardPrefab` (`Assets/Prefab/3DCardPanchina.prefab`), `deckPosition` (its child 0 must be the slot container, see [scenes](./scenes.md#panchina)). In the scene YAML the component still has the old class identifier `DeckManager3DTest`.
- `GetSlotByIndex(index)`: `deckPosition.GetChild(0).GetChild(index)`. Table state is read from the hierarchy; during a path cards move but **don't change parent**, so after the first path "the card in slot X" is the card dealt there, not the one that is there now.
- **Static state**: `instance`, `cardDatabase`, `cardDatabaseInGame`, `shuffledCardDatabaseInGame`, `deckCards`; never cleared or reset.
- `DECK_LIMIT = 40` (field, not `const`), `9` duplicates and `10` for the king are hard-coded.
- **Used by**: [CardAnimationsPanchina](#cardanimationspanchina) (`instance.GetSlotByIndex`), [CardGeneratorPanchina](#cardgeneratorpanchina) (fills the databases).

## CardControllerPanchina

`Assets/Scripts/Panchina/CardController.cs`. On `OnMouseDown` of a face-down card, when clicks are not blocked, runs `CardAnimationsPanchina.MoveCards(card)`.

- **Prefab**: `Assets/Prefab/3DCardPanchina.prefab` (with [CardDisplay3D](./shared.md#carddisplay3d)).

## CardAnimationsPanchina

`Assets/Scripts/Panchina/CardAnimations.cs`. Same helpers as [CardAnimations](./shared.md#cardanimations), with a different path rule in `MoveCards(start)`:

- the next card is the **second child of the slot** of the current card's `index` (`GetSlotByIndex(index).GetChild(1)`: child 0 is the slot's `Cube`);
- the path stops when it gets back to the starting index **or meets a card already flipped** (`isFlipped`);
- the last card flips and moves to the clicked card's starting position.

The stop rule is one of the open questions of [Gameplay di base](../business/gameplay-di-base.md).

## CardGeneratorPanchina

`Assets/Scripts/Panchina/CardGenerator.cs`. Menu `Tools/Genera Carte Napoletane Panchina`. Identical to [CardGenerator](./shared.md#cardgenerator) (same asset paths, same problems), but fills the `DeckManagerPanchina` databases. Called at runtime by [DeckManagerPanchina](#deckmanagerpanchina).

## CardSlot

`Assets/Scripts/Panchina/CardSlot.cs`. `int index` on each of the 40 slots (`Assets/Prefab/CardSlot.prefab` instances in the Panchina scene). **Never read by the code**: slots are found by child order. The scene values are not in child order, by choice (test data).

## CardSlotContainer

`Assets/Scripts/Panchina/CardSlotContainer.cs`. `List<GameObject> CardSlotList` on the slot container of the Panchina scene. **Never read by the code.**
