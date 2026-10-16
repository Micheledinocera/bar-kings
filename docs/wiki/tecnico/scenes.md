---
tipo: info
aggiornato: 2026-10-16
verificato: 2026-10-16
consolidato: 2026-10-16
fonti:
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/index40/2026-10-16-obiettivo.md
  - raw/compiled/stats/2026-10-16-sessione.md
  - raw/compiled/tribuna/2026-10-15-implementazione.md
  - raw/compiled/tribuna/2026-10-15-obiettivo.md
  - raw/compiled/panchina-reprise/2026-10-15-implementazione.md
  - raw/compiled/panchina-reprise/2026-10-15-obiettivo.md
  - raw/compiled/unity-mcp/2026-10-15-test-e-controllo.md
  - raw/compiled/panchina-start/2026-10-11-fix-path.md
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/card-generator/2026-10-11-modifiche-editor.md
  - raw/compiled/git-line-endings/2026-10-11-obiettivo.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Scenes, prefabs and data assets

Scenes in `Assets/Scenes/`, prefabs in `Assets/Prefab/`, card assets in `Assets/Instances/Cards/`, deck presets in `Assets/Instances/DeckPresets/`, and the project settings that the code depends on. Scripts: [shared](./shared.md), [table](./table.md), [deck](./deck.md), [stats](./stats.md).

The three scenes of the [deck modification ideas](../business/modifica-del-mazzo.md) (`Panchina`, `ScenaTribuna`, `ScenaIndex40`) are **temporary**: they let each idea be explored without breaking the previous version. When one idea is chosen, the other scenes and the code only they use are deleted.

| Scene | Path | Deck manager | Card prefab |
|---|---|---|---|
| [SampleScene](#samplescene) | `Assets/Scenes/SampleScene.unity` | [DeckManager3D](./shared.md#deckmanager3d) | `3DCard` |
| [Panchina](#panchina-scenatribuna-and-scenaindex40) | `Assets/Scenes/Panchina.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Bench`, `Preset32`) | `3DCardTable` |
| [ScenaTribuna](#panchina-scenatribuna-and-scenaindex40) | `Assets/Scenes/ScenaTribuna.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Discard`, `Preset71`) | `3DCardTable` |
| [ScenaIndex40](#panchina-scenatribuna-and-scenaindex40) | `Assets/Scenes/ScenaIndex40.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Index`, `Preset45`) | `3DCardTable` |

## SampleScene

The **first POC** (plain 4-kings solitaire): camera, light, a `GameManager` GameObject ([GameManager](./shared.md#gamemanager)) and [DeckManager3D](./shared.md#deckmanager3d), which deals the 40 cards onto a 4×10 grid at Play.

All four scenes are in the Build Settings (`ProjectSettings/EditorBuildSettings.asset`): `SampleScene`, `Panchina`, `ScenaTribuna`, `ScenaIndex40`.

## Panchina, ScenaTribuna and ScenaIndex40

Same structure, same [table scripts](./table.md); they differ in `TableDeckManager.excessCards` (`Bench`, `Discard`, `Index`) and `deckPreset` (see the table above, [DeckPreset](./deck.md#deckpreset)); all deal with a random seed and use `pathMoveDuration` 0.05. ScenaTribuna's slots, UI and EventSystem were copied from Panchina; ScenaIndex40's UI and EventSystem too. Main GameObjects:

- `Tavolo`: the table;
- `SlotContainer`: 40 instances of `Assets/Prefab/CardSlot.prefab` in Panchina and ScenaTribuna (with [CardSlotContainer](./table.md#cardslotcontainer) and [CardSlot](./table.md#cardslot)), **80** in ScenaIndex40 (`Slot0`…`Slot79`, 8 rows, no `CardSlot` components; the slots beyond the deal are hidden at Play); must be child 0 of the deck manager's `deckPosition` (`CardsContainer`), with the slots in grid order, because [TableDeckManager](./table.md#tabledeckmanager) finds the slot Transforms by child index to parent the cards (the table state itself is in [TableState](./table.md#tablestate));
- `CardsContainer`, `GameManager` (with [GameManager](./shared.md#gamemanager) and [TableDeckManager](./table.md#tabledeckmanager)), `Main Camera`, `Directional Light`; many GameObjects have no name. In ScenaIndex40 the camera is higher and shifted (`(-1.6, 21.65, -12.75)`, same 60° angle) so that the 8 rows fit to the right of the game log;
- `UI`: Canvas (Screen Space Overlay, scaled with screen size) with:
  - `GameLog` (instance of `Assets/Prefab/GameLog.prefab`, left side): the game log, assigned to `TableDeckManager.gameLog`;
  - `ResetButton` (TextMeshPro text "Reset", top right), whose persistent `OnClick` calls `TableDeckManager.ResetTable` on the scene's own `GameManager` ([TableDeckManager](./table.md#tabledeckmanager));
  - `SimulationButton` (instance of `Assets/Prefab/SimulationButton.prefab`, under the Reset button), with the scene's `TableDeckManager` and `GameLog` as overrides of its [TableSimulationRunner](./stats.md#tablesimulationrunner);
- `EventSystem` with `InputSystemUIInputModule` (the Input System UI module, the only input handler enabled).

## Prefabs

- `Assets/Prefab/3DCard.prefab`: root with [CardDisplay3D](./shared.md#carddisplay3d) and [CardController](./shared.md#cardcontroller); children `Fronte` and `Retro` (`Mat_Carta_Fronte`, `Mat_Carta_Retro`).
- `Assets/Prefab/3DCardTable.prefab` (was `3DCardPanchina`): a **copy** of `3DCard` (not a Prefab Variant) with [TableCardController](./table.md#tablecardcontroller) instead of `CardController`; used by the three table scenes.
- `Assets/Prefab/CardSlot.prefab`: `CardSlot` root with a `Cube` child (`Mat_Card_Slot`).
- `Assets/Prefab/GameLog.prefab`: scrollable panel (`ScrollRect`, `Viewport` with `RectMask2D`, `Content` with a TMP `Text`, vertical `Scrollbar`) anchored to the left of the Canvas, 520 px wide, with [GameLogView](./stats.md#gamelogview).
- `Assets/Prefab/SimulationButton.prefab`: a copy of the `ResetButton` look (text "Simula") with [TableSimulationRunner](./stats.md#tablesimulationrunner) on the root, its own `Button` and the four presets; `OnClick` calls `Run`. `tableDeckManager` and `gameLog` are set on the scene instances.

## Card assets

`Assets/Instances/Cards/<valore>_<Seme>.asset`: the 40 [CardData](./shared.md#carddata), fronts from `Assets/Images/Cards/Fronte/` (`01_…` to `40_…jpg`: Denari, Coppe, Spade, Bastoni; ace to ten each). Created only by the menu `Tools/Genera Carte Napoletane` ([CardGenerator](./shared.md#cardgenerator)), which skips existing ones; Play no longer touches them.

`Assets/Instances/CardDatabase.asset`: the [CardDatabase](./shared.md#carddatabase) with the 40 cards in table order. Assigned to the `deckDatabase` field of the deck manager in **all four scenes**; a new scene with a deck manager needs it too.

`Assets/Instances/DeckPresets/Preset32.asset`, `Preset45.asset`, `Preset60.asset`, `Preset71.asset` (was `Preset80`, same GUID): the [DeckPreset](./deck.md#deckpreset) assets, created only by the menu `Tools/Genera Preset Mazzo` ([DeckPresetGenerator](./deck.md#deckpresetgenerator)).

## Project settings

Settings the code depends on; change them only saying so (`AGENTS.md`).

| Setting | Value | Where | Effect on the code |
|---|---|---|---|
| Active Input Handling | Input System Package only | `ProjectSettings/ProjectSettings.asset` (`activeInputHandler: 1`) | Legacy `UnityEngine.Input` throws ([CameraOrbit](./shared.md#cameraorbit)); `OnMouseDown` still works |
| Enter Play Mode Options | enabled, reload domain and scene | `ProjectSettings/EditorSettings.asset` (`m_EnterPlayModeOptions: 0`) | Static lists are reset at every Play; with domain reload off they would grow at every Play ([Unity docs](https://docs.unity3d.com/Manual/configurable-enter-play-mode.html)) |
| Render pipeline | URP | `Assets/Settings/` | Card material texture is `_BaseMap` |
| DOTween settings | `Assets/Resources/DOTweenSettings.asset` | — | Rewritten by DOTween itself; not project code |
| Unity version | `6000.4.9f1` | `ProjectSettings/ProjectVersion.txt` | — |

Line endings: LF everywhere, matching what Unity writes: `.gitattributes` (git), `.csharpierrc.json` (CSharpier, `endOfLine: lf`), `.editorconfig` (editors). Versioned, so valid for every clone; an older clone needs a one-time re-checkout (`CONTRIBUTING.md`).

TextMeshPro's dynamic fallback font (`Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`) is versioned, because the default `LiberationSans SDF` references it: it changes when a text shows characters missing from the atlas, so the code's texts stay ASCII ([GameLogFormatter](./stats.md#gamelogformatter)). Simulation output goes to `Simulations/` in the project root, git-ignored ([TableSimulationRunner](./stats.md#tablesimulationrunner)).
