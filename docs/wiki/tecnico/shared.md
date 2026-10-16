---
tipo: info
aggiornato: 2026-10-16
verificato: 2026-10-16
consolidato: 2026-10-15
fonti:
  - raw/compiled/demo-1/2026-10-16-obiettivo.md
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

Scripts in `Assets/Scripts/Shared/`: card data and the card database, the card generator (editor only), the 3D card, the click flag and the scene loader of the Menu. The table scenes have their own scripts in `Assets/Scripts/Shared/Table/` ([table](./table.md)). The sub-folders `Deck/` and `Stats/` have their own pages: [deck](./deck.md), [stats](./stats.md). No namespace, no assembly definition: everything compiles into `Assembly-CSharp`. Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [CardData](#carddata) | `Assets/Scripts/Shared/Cards/CardData.cs` | ScriptableObject + enum `Seme` |
| [CardDataInGame](#carddataingame) | `Assets/Scripts/Shared/Cards/CardDataInGame.cs` | plain C# class |
| [CardDatabase](#carddatabase) | `Assets/Scripts/Shared/Cards/CardDatabase.cs` | ScriptableObject |
| [CardGenerator](#cardgenerator) | `Assets/Scripts/Shared/Editor/CardGenerator.cs` | static editor class (menu item) |
| [CardDisplay3D](#carddisplay3d) | `Assets/Scripts/Shared/Cards3D/CardDisplay.cs` | MonoBehaviour |
| [GameManager](#gamemanager) | `Assets/Scripts/Shared/GameManager.cs` | MonoBehaviour |
| [SceneLoader](#sceneloader) | `Assets/Scripts/Shared/Menu/SceneLoader.cs` | MonoBehaviour |

File names don't always match class names (`CardDisplay.cs` → `CardDisplay3D`): Unity shows no warning, search by class name.

## CardData

`Assets/Scripts/Shared/Cards/CardData.cs`. ScriptableObject of one Neapolitan card: `nome` (string), `valore` (1–10, 10 = king), `seme` (enum `Seme { Denari, Coppe, Spade, Bastoni }`), `immagineFronte` (Sprite). Create menu: `Carte/Carta Napoletana`.

- **Assets**: 40 instances in `Assets/Instances/Cards/` (`<valore>_<Seme>.asset`), created by [CardGenerator](#cardgenerator) from `Assets/Images/Cards/Fronte/*.jpg`, listed in [CardDatabase](#carddatabase).
- **Used by**: [CardDatabase](#carddatabase), and every class of this page and of [table](./table.md) through [CardDataInGame](#carddataingame).
- Field names are Italian by choice and stay so (`AGENTS.md`).

## CardDataInGame

`Assets/Scripts/Shared/Cards/CardDataInGame.cs`. A `CardData` plus `index`: the card's **place on the table** (0–39, row = suit, column = value − 1, from the order of the image files). The path moves a card to the slot of its `index`.

- **Used by**: [CardDatabase](#carddatabase) (`CreateCardsInGame` creates them), [CardDisplay3D](#carddisplay3d), the [table](./table.md) classes, [DeckBuilder](./deck.md#deckbuilder) and the [stats](./stats.md) classes.

## CardDatabase

`Assets/Scripts/Shared/Cards/CardDatabase.cs`. ScriptableObject listing the 40 [CardData](#carddata) in **table order**: a card's position in `Cards` is its `index` on the table. Create menu: `Carte/Card Database`.

- **Asset**: `Assets/Instances/CardDatabase.asset`, created and kept in sync by [CardGenerator](#cardgenerator); don't reorder it by hand, the order is the table layout.
- `CreateCardsInGame()`: a new list of [CardDataInGame](#carddataingame) with `index` = position. `SetCards` exists only in the Editor (`#if UNITY_EDITOR`), for the generator.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager) through its serialized field `deckDatabase`, assigned in the three table scenes ([scenes](./scenes.md)); [TableSimulationRunner](./stats.md#tablesimulationrunner) through `TableDeckManager.DeckDatabase`.

## CardGenerator

`Assets/Scripts/Shared/Editor/CardGenerator.cs`. Editor only (in an `Editor/` folder, not in the player build). Menu `Tools/Genera Carte Napoletane`; never called at runtime. Run it when the card images change:

- sorts the `.jpg` files of `Assets/Images/Cards/Fronte` by name and derives suit and value from the position (`01_`…`40_`: 10 cards per suit in `Seme` order, ace to king);
- creates the [CardData](#carddata) assets that **don't exist yet** in `Assets/Instances/Cards/`; existing ones are kept as they are (no overwrite, GUIDs unchanged);
- creates or updates [CardDatabase](#carddatabase) (`Assets/Instances/CardDatabase.asset`) with the 40 cards in order, and logs how many cards were new.

## CardDisplay3D

`Assets/Scripts/Shared/Cards3D/CardDisplay.cs`. Root component of the 3D card. Fields: `cardData` ([CardDataInGame](#carddataingame), set by `Setup`), `fronte` / `retro` (child GameObjects), `isFlipped` (true once the card took part in a path).

- `Setup(data)` sets the front texture (`_BaseMap`, URP Lit) on `fronte`'s renderer through a `MaterialPropertyBlock`: every card shares the material asset, no instance per card ([MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html)).
- **Prefab**: `Assets/Prefab/3DCardTable.prefab`.
- **Used by**: the [table](./table.md) classes ([TableDeckManager](./table.md#tabledeckmanager), [TableCardController](./table.md#tablecardcontroller), [TableCardAnimations](./table.md#tablecardanimations)).

## GameManager

`Assets/Scripts/Shared/GameManager.cs`. Only `public static bool isClickBlocked`: true while dealing or while a path runs. A GameObject with this component is in the three table scenes (not in Menu), but nothing reads the instance. Leaving a scene while cards are dealt leaves it true; the next table scene sets it again at setup.

- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [TableCardController](./table.md#tablecardcontroller).

## SceneLoader

`Assets/Scripts/Shared/Menu/SceneLoader.cs`. `Load()` loads the scene named `sceneName` (`SceneManager.LoadScene`; the scene must be in the Build Settings). On the root of `Assets/Prefab/SceneButton.prefab`, whose `OnClick` calls `Load` ([scenes](./scenes.md#prefabs)).

- **Used by**: the three buttons of the Menu (`sceneName` `Panchina`, `Tribuna`, `Index40`) and the `BackButton` of the table scenes (`Menu`, the prefab default).
