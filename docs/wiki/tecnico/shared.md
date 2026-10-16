---
tipo: info
aggiornato: 2026-10-16
verificato: 2026-10-16
consolidato: 2026-10-15
fonti:
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/stats/2026-10-16-sessione.md
  - raw/compiled/tribuna/2026-10-15-implementazione.md
  - raw/compiled/tribuna/2026-10-15-obiettivo.md
  - raw/compiled/panchina-continue/2026-10-15-implementazione.md
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/card-generator/2026-10-11-scelta-card-database.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Shared scripts

Scripts in `Assets/Scripts/Shared/`: card data and the card database, the card generator (editor only), the 3D card, the first POC (plain 4-kings solitaire) and small helpers. Despite the folder name, `DeckManager3D`, `CardController` and `CardAnimations` are the **first POC**, used only by `SampleScene` (see [scenes](./scenes.md)); the Panchina, Tribuna and Index40 scenes use their copies in `Assets/Scripts/Shared/Table/` ([table](./table.md)). The sub-folders `Deck/` and `Stats/` have their own pages: [deck](./deck.md), [stats](./stats.md). No namespace, no assembly definition: everything compiles into `Assembly-CSharp`. Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [CardData](#carddata) | `Assets/Scripts/Shared/Cards/CardData.cs` | ScriptableObject + enum `Seme` |
| [CardDataInGame](#carddataingame) | `Assets/Scripts/Shared/Cards/CardDataInGame.cs` | plain C# class |
| [CardDatabase](#carddatabase) | `Assets/Scripts/Shared/Cards/CardDatabase.cs` | ScriptableObject |
| [CardGenerator](#cardgenerator) | `Assets/Scripts/Shared/Editor/CardGenerator.cs` | static editor class (menu item) |
| [CardDisplay3D](#carddisplay3d) | `Assets/Scripts/Shared/Cards3D/CardDisplay.cs` | MonoBehaviour |
| [CardController](#cardcontroller) | `Assets/Scripts/Shared/Cards3D/CardController.cs` | MonoBehaviour |
| [DeckManager3D](#deckmanager3d) | `Assets/Scripts/Shared/Cards3D/DeckManager.cs` | MonoBehaviour |
| [CardAnimations](#cardanimations) | `Assets/Scripts/Shared/Animations/CardAnimations.cs` | static class (DOTween) |
| [GameManager](#gamemanager) | `Assets/Scripts/Shared/GameManager.cs` | MonoBehaviour |
| [CameraOrbit](#cameraorbit) | `Assets/Scripts/Shared/Camera/CameraOrbit.cs` | MonoBehaviour, unused |

File names don't always match class names (`CardDisplay.cs` → `CardDisplay3D`, `DeckManager.cs` → `DeckManager3D`): Unity shows no warning, search by class name.

## CardData

`Assets/Scripts/Shared/Cards/CardData.cs`. ScriptableObject of one Neapolitan card: `nome` (string), `valore` (1–10, 10 = king), `seme` (enum `Seme { Denari, Coppe, Spade, Bastoni }`), `immagineFronte` (Sprite). Create menu: `Carte/Carta Napoletana`.

- **Assets**: 40 instances in `Assets/Instances/Cards/` (`<valore>_<Seme>.asset`), created by [CardGenerator](#cardgenerator) from `Assets/Images/Cards/Fronte/*.jpg`, listed in [CardDatabase](#carddatabase).
- **Used by**: [CardDatabase](#carddatabase), and every class of this page and of [table](./table.md) through [CardDataInGame](#carddataingame).
- Field names are Italian by choice and stay so (`AGENTS.md`).

## CardDataInGame

`Assets/Scripts/Shared/Cards/CardDataInGame.cs`. A `CardData` plus `index`: the card's **place on the table** (0–39, row = suit, column = value − 1, from the order of the image files). The path moves a card to the slot of its `index`.

- **Used by**: [CardDatabase](#carddatabase) (`CreateCardsInGame` creates them), [DeckManager3D](#deckmanager3d), [CardDisplay3D](#carddisplay3d), the [table](./table.md) copies, [DeckBuilder](./deck.md#deckbuilder) and the [stats](./stats.md) classes.

## CardDatabase

`Assets/Scripts/Shared/Cards/CardDatabase.cs`. ScriptableObject listing the 40 [CardData](#carddata) in **table order**: a card's position in `Cards` is its `index` on the table. Create menu: `Carte/Card Database`.

- **Asset**: `Assets/Instances/CardDatabase.asset`, created and kept in sync by [CardGenerator](#cardgenerator); don't reorder it by hand, the order is the table layout.
- `CreateCardsInGame()`: a new list of [CardDataInGame](#carddataingame) with `index` = position. `SetCards` exists only in the Editor (`#if UNITY_EDITOR`), for the generator.
- **Used by**: [DeckManager3D](#deckmanager3d) and [TableDeckManager](./table.md#tabledeckmanager) through their serialized field `deckDatabase`, assigned in all four scenes ([scenes](./scenes.md)); [TableSimulationRunner](./stats.md#tablesimulationrunner) through `TableDeckManager.DeckDatabase`.

## CardGenerator

`Assets/Scripts/Shared/Editor/CardGenerator.cs`. Editor only (in an `Editor/` folder, not in the player build). Menu `Tools/Genera Carte Napoletane`; never called at runtime. Run it when the card images change:

- sorts the `.jpg` files of `Assets/Images/Cards/Fronte` by name and derives suit and value from the position (`01_`…`40_`: 10 cards per suit in `Seme` order, ace to king);
- creates the [CardData](#carddata) assets that **don't exist yet** in `Assets/Instances/Cards/`; existing ones are kept as they are (no overwrite, GUIDs unchanged);
- creates or updates [CardDatabase](#carddatabase) (`Assets/Instances/CardDatabase.asset`) with the 40 cards in order, and logs how many cards were new.

## CardDisplay3D

`Assets/Scripts/Shared/Cards3D/CardDisplay.cs`. Root component of the 3D card. Fields: `cardData` ([CardDataInGame](#carddataingame), set by `Setup`), `fronte` / `retro` (child GameObjects), `isFlipped` (true once the card took part in a path).

- `Setup(data)` sets the front texture (`_BaseMap`, URP Lit) on `fronte`'s renderer through a `MaterialPropertyBlock`: every card shares the material asset, no instance per card ([MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html)).
- **Prefabs**: `Assets/Prefab/3DCard.prefab`, `Assets/Prefab/3DCardTable.prefab`.
- **Used by**: [DeckManager3D](#deckmanager3d), [CardController](#cardcontroller), [CardAnimations](#cardanimations) and the [table](./table.md) copies.

## CardController

`Assets/Scripts/Shared/Cards3D/CardController.cs`. On `OnMouseDown` of a face-down card, when clicks are not blocked ([GameManager](#gamemanager)), blocks them, runs the path `CardAnimations.MoveCards(card, DeckManager3D.deckCards[card.index], destroyCancellationToken)` and unblocks them in a `finally`.

- **Prefab**: `Assets/Prefab/3DCard.prefab`.
- `OnMouseDown` works in practice (the POCs are played by clicking) although the project enables only the Input System package.
- `OnMouseDown` calls an `async UniTaskVoid` method with `.Forget()`: a cancelled path is not logged as an error.

## DeckManager3D

`Assets/Scripts/Shared/Cards3D/DeckManager.cs`. First-POC game setup, in `Start`: blocks clicks, builds `cardDatabase` / `cardDatabaseInGame` from `deckDatabase` ([CardDatabase](#carddatabase)), shuffles the 40 cards, instantiates `cardPrefab` as a pile under `deckPosition`, animates each card to a 4×10 grid (`x = -5 + 1.2·(i % 10)`, `z = 4.5 - 1.5·(i / 10)`), unblocks clicks. `Start` runs this as an `async UniTaskVoid` with `.Forget()`, cancelled by `destroyCancellationToken` (`TweenCancelBehaviour.KillAndCancelAwait`: the await stops with the tween).

- **Serialized fields**: `cardPrefab` (the `3DCard` prefab), `deckPosition` (Transform of the pile), `deckDatabase` ([CardDatabase](#carddatabase); if left empty, `Start` throws).
- **Static state**: `cardDatabase`, `cardDatabaseInGame`, `shuffledCardDatabaseInGame` (the same list object as `cardDatabaseInGame`, shuffled in place), `deckCards` (instantiated cards, in grid order). The first two are rebuilt from `deckDatabase` at every `Start`; `deckCards` is never cleared: safe only because Play mode reloads the domain (see [scenes](./scenes.md#project-settings)).
- **Scenes**: `Assets/Scenes/SampleScene.unity`.
- **Used by**: [CardController](#cardcontroller) and [CardAnimations](#cardanimations) read `deckCards`.

## CardAnimations

`Assets/Scripts/Shared/Animations/CardAnimations.cs`. Static DOTween helpers ([DOTween docs](https://dotween.demigiant.com/documentation.php)):

- `MoveAnimation(card, target, duration)`: lift and move, `SetLink(card)`. Used by [DeckManager3D](#deckmanager3d) to deal.
- `MoveCards(start, end, cancellationToken, duration)` (`UniTask`): the **path**. Lifts the clicked card, then repeatedly moves the current card onto the position of the next one (`MoveCardsAtom`) and continues with the card found at `DeckManager3D.deckCards[index]`, until it gets back to the clicked card's index; the last card flips and goes to the starting position. Marks every card it touches `isFlipped`. Every tween is linked to its card; on cancellation it is killed and the await stops (`TweenCancelBehaviour.KillAndCancelAwait`), so nothing runs on destroyed cards.
- `MoveCardsAtom`: one step of the path, `SetLink(startingCard)`.
- **Used by**: [CardController](#cardcontroller), [DeckManager3D](#deckmanager3d). Copy: [TableCardAnimations](./table.md#tablecardanimations).

## GameManager

`Assets/Scripts/Shared/GameManager.cs`. Only `public static bool isClickBlocked`: true while dealing or while a path runs. A GameObject with this component is in all four scenes, but nothing reads the instance.

- **Used by**: [CardController](#cardcontroller), [DeckManager3D](#deckmanager3d), and the [table](./table.md) copies.

## CameraOrbit

`Assets/Scripts/Shared/Camera/CameraOrbit.cs`. Orbits the camera around `target` with the right mouse button. **Not in any scene**, and uses the legacy `UnityEngine.Input`, which throws with the Input System–only setting.
