---
tipo: info
aggiornato: 2026-10-11
verificato: 2026-10-11
fonti:
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Shared scripts

Scripts in `Assets/Scripts/Shared/`: card data, the 3D card, the first POC (plain 4-kings solitaire) and small helpers. Despite the folder name, `DeckManager3D`, `CardController` and `CardAnimations` are the **first POC**, used only by the twin scenes (see [scenes](./scenes.md)); the Panchina scene has its own copies ([panchina](./panchina.md)). No namespace, no assembly definition: everything compiles into `Assembly-CSharp`. Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [CardData](#carddata) | `Assets/Scripts/Shared/Cards/CardData.cs` | ScriptableObject + enum `Seme` |
| [CardDataInGame](#carddataingame) | `Assets/Scripts/Shared/Cards/CardDataInGame.cs` | plain C# class |
| [CardGenerator](#cardgenerator) | `Assets/Scripts/Shared/Cards/CardGenerator.cs` | EditorWindow (menu item) |
| [CardDisplay3D](#carddisplay3d) | `Assets/Scripts/Shared/Cards3D/CardDisplay.cs` | MonoBehaviour |
| [CardController](#cardcontroller) | `Assets/Scripts/Shared/Cards3D/CardController.cs` | MonoBehaviour |
| [DeckManager3D](#deckmanager3d) | `Assets/Scripts/Shared/Cards3D/DeckManager.cs` | MonoBehaviour |
| [CardAnimations](#cardanimations) | `Assets/Scripts/Shared/Animations/CardAnimations.cs` | static class (DOTween) |
| [GameManager](#gamemanager) | `Assets/Scripts/Shared/GameManager.cs` | MonoBehaviour |
| [CameraOrbit](#cameraorbit) | `Assets/Scripts/Shared/Camera/CameraOrbit.cs` | MonoBehaviour, unused |

File names don't always match class names (`CardDisplay.cs` → `CardDisplay3D`, `DeckManager.cs` → `DeckManager3D`): Unity shows no warning, search by class name.

## CardData

`Assets/Scripts/Shared/Cards/CardData.cs`. ScriptableObject of one Neapolitan card: `nome` (string), `valore` (1–10, 10 = king), `seme` (enum `Seme { Denari, Coppe, Spade, Bastoni }`), `immagineFronte` (Sprite). Create menu: `Carte/Carta Napoletana`.

- **Assets**: 40 instances in `Assets/Instances/Cards/` (`<valore>_<Seme>.asset`), created by [CardGenerator](#cardgenerator) from `Assets/Images/Cards/Fronte/*.jpg`.
- **Used by**: every class of this page and of [panchina](./panchina.md), through [CardDataInGame](#carddataingame).
- Field names are Italian by choice and stay so (`AGENTS.md`).

## CardDataInGame

`Assets/Scripts/Shared/Cards/CardDataInGame.cs`. A `CardData` plus `index`: the card's **place on the table** (0–39, row = suit, column = value − 1, from the order of the image files). The path moves a card to the slot of its `index`.

- **Used by**: [CardGenerator](#cardgenerator) (creates them), [DeckManager3D](#deckmanager3d), [CardDisplay3D](#carddisplay3d), and the Panchina copies.

## CardGenerator

`Assets/Scripts/Shared/Cards/CardGenerator.cs`. Editor menu `Tools/Genera Carte Napoletane`: reads the `.jpg` files of `Assets/Images/Cards/Fronte`, creates one [CardData](#carddata) asset each in `Assets/Instances/Cards/` (`AssetDatabase.CreateAsset`) and fills `DeckManager3D.cardDatabase` / `cardDatabaseInGame`.

- **Called at runtime** by `DeckManager3D.Start`, not only from the menu: every Play rewrites the 40 assets (same content and GUIDs; git may list them as modified because of line endings).
- Suit and value come from the **file order** (`i / 10`, `i % 10`): it relies on the `01_`…`40_` prefixes and on `Directory.GetFiles` returning them sorted.
- Uses `UnityEditor` outside an `Editor/` folder: a player build does not compile. See [known-issues](./known-issues.md#block-a--card-generation-and-editor-code).
- Copy: `CardGeneratorPanchina` in [panchina](./panchina.md#cardgeneratorpanchina).

## CardDisplay3D

`Assets/Scripts/Shared/Cards3D/CardDisplay.cs`. Root component of the 3D card. Fields: `cardData` ([CardDataInGame](#carddataingame), set by `Setup`), `fronte` / `retro` (child GameObjects), `isFlipped` (true once the card took part in a path).

- `Setup(data)` sets the front texture on `fronte`'s material (`_BaseMap`, URP Lit); `meshRenderer.material` creates one material instance per card.
- **Prefabs**: `Assets/Prefab/3DCard.prefab`, `Assets/Prefab/3DCardPanchina.prefab`.
- **Used by**: [DeckManager3D](#deckmanager3d), [CardController](#cardcontroller), [CardAnimations](#cardanimations) and the Panchina copies.

## CardController

`Assets/Scripts/Shared/Cards3D/CardController.cs`. On `OnMouseDown` of a face-down card, when clicks are not blocked ([GameManager](#gamemanager)), runs the path: `CardAnimations.MoveCards(card, DeckManager3D.deckCards[card.index])`.

- **Prefab**: `Assets/Prefab/3DCard.prefab`.
- `OnMouseDown` works in practice (the POCs are played by clicking) although the project enables only the Input System package.
- `async void`, no cancellation token.

## DeckManager3D

`Assets/Scripts/Shared/Cards3D/DeckManager.cs`. First-POC game setup, in `Start`: blocks clicks, calls [CardGenerator](#cardgenerator), shuffles the 40 cards, instantiates `cardPrefab` as a pile under `deckPosition`, animates each card to a 4×10 grid (`x = -5 + 1.2·(i % 10)`, `z = 4.5 - 1.5·(i / 10)`), unblocks clicks.

- **Serialized fields**: `cardPrefab` (the `3DCard` prefab), `deckPosition` (Transform of the pile).
- **Static state**: `cardDatabase`, `cardDatabaseInGame`, `shuffledCardDatabaseInGame` (the same list object as `cardDatabaseInGame`, shuffled in place), `deckCards` (instantiated cards, in grid order). Never cleared: safe only because Play mode reloads the domain (see [scenes](./scenes.md#project-settings)).
- **Scenes**: `Assets/Scenes/SampleScene.unity`, `Assets/Scenes/ScenaIndex40.unity`, `Assets/Scenes/ScenaTribuna.unity`.
- **Used by**: [CardController](#cardcontroller) and [CardAnimations](#cardanimations) read `deckCards`; [CardGenerator](#cardgenerator) fills the databases.

## CardAnimations

`Assets/Scripts/Shared/Animations/CardAnimations.cs`. Static DOTween helpers ([DOTween docs](https://dotween.demigiant.com/documentation.php)):

- `MoveAnimation(card, target, duration)`: lift and move, `SetLink(card)`. Used by [DeckManager3D](#deckmanager3d) to deal.
- `MoveCards(start, end, duration)`: the **path**. Lifts the clicked card, then repeatedly moves the current card onto the position of the next one (`MoveCardsAtom`) and continues with the card found at `DeckManager3D.deckCards[index]`, until it gets back to the clicked card's index; the last card flips and goes to the starting position. Marks every card it touches `isFlipped`.
- `MoveCardsAtom`: one step of the path; its `Sequence` has no `SetLink`.
- `FlipAnimation`: unused.
- **Used by**: [CardController](#cardcontroller), [DeckManager3D](#deckmanager3d). Copy: `CardAnimationsPanchina`.

## GameManager

`Assets/Scripts/Shared/GameManager.cs`. Only `public static bool isClickBlocked`: true while dealing or while a path runs. A GameObject with this component is in all four scenes, but nothing reads the instance.

- **Used by**: [CardController](#cardcontroller), [DeckManager3D](#deckmanager3d), and the Panchina copies.

## CameraOrbit

`Assets/Scripts/Shared/Camera/CameraOrbit.cs`. Orbits the camera around `target` with the right mouse button. **Not in any scene**, and uses the legacy `UnityEngine.Input`, which throws with the Input System–only setting.

## Open questions

- Does the generator rewriting assets at every Play change anything besides line endings on other machines (e.g. macOS)?
