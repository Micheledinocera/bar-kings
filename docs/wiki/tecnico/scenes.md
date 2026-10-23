---
tipo: info
aggiornato: 2026-10-23
verificato: 2026-10-23
consolidato: 2026-10-16
fonti:
  - raw/compiled/preset-buchi/2026-10-23-implementazione.md
  - raw/compiled/preset-builder/2026-10-19-obiettivo.md
  - raw/compiled/build/2026-10-16-sistemazione.md
  - raw/compiled/build/2026-10-16-comando-build.md
  - raw/compiled/build/2026-10-16-modifiche-non-salvate.md
  - raw/compiled/demo-1/2026-10-16-build.md
  - raw/compiled/demo-1/2026-10-16-opzioni.md
  - raw/compiled/demo-1/2026-10-16-obiettivo.md
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

Scenes in `Assets/Scenes/`, prefabs in `Assets/Prefab/`, card assets in `Assets/Instances/Cards/`, deck presets in `Assets/Instances/DeckPresets/` and their library, and the project settings that the code depends on. Scripts: [shared](./shared.md), [table](./table.md), [deck](./deck.md), [stats](./stats.md).

The three scenes of the [deck modification ideas](../business/modifica-del-mazzo.md) (`Panchina`, `Tribuna`, `Index40`) are **temporary**: they let each idea be explored without breaking the previous version. When one idea is chosen, the other scenes and the code only they use are deleted.

The **Menu** opens first and leads to the three table scenes: a mini demo to show the three ideas and to try the builds.

| Scene | Path | Deck manager | Card prefab |
|---|---|---|---|
| [Menu](#menu) | `Assets/Scenes/Menu.unity` | — | — |
| [Panchina](#panchina-tribuna-and-index40) | `Assets/Scenes/Panchina.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Bench`) | `3DCardTable` |
| [Tribuna](#panchina-tribuna-and-index40) | `Assets/Scenes/Tribuna.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Discard`) | `3DCardTable` |
| [Index40](#panchina-tribuna-and-index40) | `Assets/Scenes/Index40.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Index`) | `3DCardTable` |

All four scenes are in the Build Settings (`ProjectSettings/EditorBuildSettings.asset`), in this order: `Menu` (the scene a build opens), `Panchina`, `Tribuna`, `Index40`.

## Menu

Scene to choose a table scene: `Main Camera` (solid green background), `Directional Light`, `UI` (Canvas, Screen Space Overlay, scaled with screen size) with the title "King Bar - demo" and three instances of `Assets/Prefab/SceneButton.prefab` (`PanchinaButton`, `TribunaButton`, `Index40Button`, [SceneLoader](./shared.md#sceneloader) `sceneName` `Panchina`, `Tribuna`, `Index40`), `EventSystem` with `InputSystemUIInputModule`. No lighting data, no `GameManager`.

## Panchina, Tribuna and Index40

Same structure, same [table scripts](./table.md); they differ in `TableDeckManager.excessCards` (`Bench`, `Discard`, `Index`). In all three `deckPreset` is empty, so the first game uses the first preset of the library (`presetLibrary`, `Assets/Instances/DeckPresetLibrary.asset`, set on each scene's `TableDeckManager`; [DeckPreset](./deck.md#deckpreset)); all deal with a random seed and start with the `Fast` animation speed. Tribuna's slots, UI and EventSystem were copied from Panchina; Index40's UI and EventSystem too. Each has its own lighting data (`Assets/Scenes/<scene>/LightingData.asset`, Generate Lighting). Main GameObjects:

- `Tavolo`: the table;
- `SlotContainer`: 40 instances of `Assets/Prefab/CardSlot.prefab` in Panchina and Tribuna (with [CardSlotContainer](./table.md#cardslotcontainer) and [CardSlot](./table.md#cardslot)), **80** in Index40 (`Slot0`…`Slot79`, 8 rows, no `CardSlot` components; the slots beyond the deal are hidden at Play); must be child 0 of the deck manager's `deckPosition` (`CardsContainer`), with the slots in grid order, because [TableDeckManager](./table.md#tabledeckmanager) finds the slot Transforms by child index to parent the cards (the table state itself is in [TableState](./table.md#tablestate));
- `CardsContainer`, `GameManager` (with [GameManager](./shared.md#gamemanager) and [TableDeckManager](./table.md#tabledeckmanager)), `Main Camera`, `Directional Light`; many GameObjects have no name. In Index40 the camera is higher and shifted (`(-1.6, 21.65, -12.75)`, same 60° angle) so that the 8 rows fit to the right of the game log;
- `UI`: Canvas (Screen Space Overlay, scaled with screen size) with:
  - `GameLog` (instance of `Assets/Prefab/GameLog.prefab`, left side): the game log, assigned to `TableDeckManager.gameLog`;
  - `ResetButton` (TextMeshPro text "Reset", top right), whose persistent `OnClick` calls `TableDeckManager.ResetTable` on the scene's own `GameManager` ([TableDeckManager](./table.md#tabledeckmanager));
  - in a column at the top right, from the top: `ResetButton`; `SimulationButton` (instance of `Assets/Prefab/SimulationButton.prefab`, with the scene's `TableDeckManager` and `GameLog` as overrides of its [TableSimulationRunner](./stats.md#tablesimulationrunner)); `LogToggleButton` (instance of `Assets/Prefab/LogToggleButton.prefab`, `OnClick` override to `GameLogView.ToggleVisible` of the scene's `GameLog`); the `OptionsButton` of `Options`; `BackButton` (instance of `Assets/Prefab/SceneButton.prefab`, back to the Menu);
  - `Options` (instance of `Assets/Prefab/Options.prefab`, last child so it is drawn on top), with the scene's `TableDeckManager` as override of its [OptionsPanel](./table.md#optionspanel);
- `EventSystem` with `InputSystemUIInputModule` (the Input System UI module, the only input handler enabled).

## Prefabs

- `Assets/Prefab/3DCardTable.prefab`: the card, root with [CardDisplay3D](./shared.md#carddisplay3d) and [TableCardController](./table.md#tablecardcontroller); children `Fronte` and `Retro` (`Mat_Carta_Fronte`, `Mat_Carta_Retro`); used by the three table scenes.
- `Assets/Prefab/CardSlot.prefab`: `CardSlot` root with a `Cube` child (`Mat_Card_Slot`).
- `Assets/Prefab/GameLog.prefab`: scrollable panel (`ScrollRect`, `Viewport` with `RectMask2D`, `Content` with a TMP `Text`, vertical `Scrollbar`) anchored to the left of the Canvas, 520 px wide, with [GameLogView](./stats.md#gamelogview).
- `Assets/Prefab/SimulationButton.prefab`: a copy of the `ResetButton` look (text "Simula") with [TableSimulationRunner](./stats.md#tablesimulationrunner) on the root, its own `Button` and the preset library (`Assets/Instances/DeckPresetLibrary.asset`), `games = 300`, both autoplay strategies; `OnClick` calls `Run`. `tableDeckManager` and `gameLog` are set on the scene instances.
- `Assets/Prefab/SceneButton.prefab`: the `ResetButton` look with [SceneLoader](./shared.md#sceneloader) (`sceneName` `Menu`, text "Back"); `OnClick` calls `Load`. Used for Back and, with overrides, for the Menu buttons.
- `Assets/Prefab/LogToggleButton.prefab`: the `ResetButton` look, text "Log", empty `OnClick` (set on the scene instances).
- `Assets/Prefab/Options.prefab`: full-canvas root (no graphic) with [OptionsPanel](./table.md#optionspanel), the `OptionsButton` (text "Opzioni") and a hidden `Panel` (dark, 400 px wide, at the left of the button column): status text, "Preset (dal prossimo Reset)" with `PresetScroll`, "Velocità animazioni" with `SpeedButtons`. `Panel` has a raycast-target `Image`, so UI clicks stop on it (cards ignore clicks over the UI, [TableCardController](./table.md#tablecardcontroller)). `PresetScroll`: vertical-only `ScrollRect` 400 px high (about 6 presets, mouse wheel and drag), `Viewport` (`RectMask2D`, transparent `Image` for the wheel) with `PresetButtons` as content (top-anchored, `ContentSizeFitter` on the height), and a vertical `Scrollbar` shown only when the presets do not fit (`AutoHideAndExpandViewport`); with few presets the area stays 400 px high. The preset library is set on the prefab (`presetLibrary`).
- `Assets/Prefab/OptionButton.prefab`: smaller button (360×60) for the entries of the options panel, instantiated by [OptionsPanel](./table.md#optionspanel).

## Card assets

`Assets/Instances/Cards/<valore>_<Seme>.asset`: the 40 [CardData](./shared.md#carddata), fronts from `Assets/Images/Cards/Fronte/` (`01_…` to `40_…jpg`: Denari, Coppe, Spade, Bastoni; ace to ten each). Created only by the menu `Tools/Genera Carte Napoletane` ([CardGenerator](./shared.md#cardgenerator)), which skips existing ones; Play no longer touches them.

`Assets/Instances/CardDatabase.asset`: the [CardDatabase](./shared.md#carddatabase) with the 40 cards in table order. Assigned to the `deckDatabase` field of the deck manager in the **three table scenes**; a new table scene needs it too.

`Assets/Instances/DeckPresets/`: the [DeckPreset](./deck.md#deckpreset) assets. Today four custom presets made with `Tools/Preset Builder` ([DeckPresetBuilderWindow](./deck.md#deckpresetbuilderwindow)), each with 8 chosen removed cards and one card with 5, 10 or 20 extra copies: `NO2-NO3+5-1`, `NO2-NO3+10-1`, `NO2-NO3+20-1`, `Xshape+10-1`. The standard presets (32, 45, 60, 71 non-king cards) and the shape presets were removed on 2026-10-23; `Tools/Genera Preset Mazzo` ([DeckPresetGenerator](./deck.md#deckpresetgenerator)) and `Tools/Genera Preset Forme` ([ShapePresetGenerator](./deck.md#shapepresetgenerator)) create them again.

`Assets/Instances/DeckPresetLibrary.asset`: the [DeckPresetLibrary](./deck.md#deckpresetlibrary), the presets shown by the options panel and run by the simulation (today the four custom presets); set in `Assets/Prefab/Options.prefab`, `Assets/Prefab/SimulationButton.prefab` and on the `TableDeckManager` of the three table scenes.

## Project settings

Settings the code depends on; change them only saying so (`AGENTS.md`).

| Setting | Value | Where | Effect on the code |
|---|---|---|---|
| Active Input Handling | Input System Package only | `ProjectSettings/ProjectSettings.asset` (`activeInputHandler: 1`) | Legacy `UnityEngine.Input` throws; `OnMouseDown` still works |
| Enter Play Mode Options | enabled, reload domain and scene | `ProjectSettings/EditorSettings.asset` (`m_EnterPlayModeOptions: 0`) | Static lists are reset at every Play; with domain reload off they would grow at every Play ([Unity docs](https://docs.unity3d.com/Manual/configurable-enter-play-mode.html)) |
| Render pipeline | URP | `Assets/Settings/` | Card material texture is `_BaseMap` |
| DOTween settings | `Assets/Resources/DOTweenSettings.asset` | — | Rewritten by DOTween itself; not project code |
| Unity version | `6000.4.9f1` | `ProjectSettings/ProjectVersion.txt` | — |

Line endings: LF everywhere, matching what Unity writes: `.gitattributes` (git), `.csharpierrc.json` (CSharpier, `endOfLine: lf`), `.editorconfig` (editors). Versioned, so valid for every clone; an older clone needs a one-time re-checkout (`CONTRIBUTING.md`).

TextMeshPro's dynamic fallback font (`Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`) is versioned, because the default `LiberationSans SDF` references it: it changes when a text shows characters missing from the atlas, so the code's texts stay ASCII ([GameLogFormatter](./stats.md#gamelogformatter)). Simulation output goes to `Simulations/` in the project root, git-ignored ([TableSimulationRunner](./stats.md#tablesimulationrunner)).

## Build

Windows build (Standalone, 64 bit) from the menu **`Tools/Build Windows`** ([WindowsBuilder](./shared.md#windowsbuilder)): the scenes enabled in the Build Settings, in `Builds/Windows/KingBar.exe` (git-ignored). `/close` runs it before every PR ([Definition of done](../progetto/definition-of-done.md)). Last known size: **145 MB**, 0 errors, 0 warnings (2026-10-16): about 80 MB is the Unity runtime and the managed DLLs (`UnityPlayer.dll`, `Managed/` with Odin, `MonoBleedingEdge`), about 41 MB textures. Use the menu item, not *File > Build*: only the menu item saves the Player Settings after the build (see below).

- **Textures**: BC compression needs sides that are multiples of 4; otherwise Unity falls back to an uncompressed format, even with compression on in the import settings. A jump of several MB in the build size is usually this.
  - card fronts (`Assets/Images/Cards/Fronte/`, 960x1586): max size 1024, high quality compression, so 620x1024 BC7 (about 0.6 MB). They stay Sprites ([CardData](./shared.md#carddata) `immagineFronte`);
  - `gingham_check_nor_gl_4k.exr` (base map of `Mat_Tavolo`, imported as Default), `gingham_check_rough_4k.exr` (bump of `Mat_Tavolo`), `gingham_check_disp_4k.png` (bump of `Mat_Carta_Retro`): NPOT scale "to nearest", max size 2048, so 2048x2048 compressed with mipmaps. The other five `gingham_check_*` are not used and are not in the build;
  - card back `Assets/Images/Cards/Carte_Napoletane_retro.jpg` (330x525): small, left uncompressed.
- **Settings written by a build**: committed as the build writes them, so a build leaves `git status` clean: URP shader prefiltering (`Assets/Settings/Universal Render Pipeline Asset.asset`), URP runtime settings (`Assets/UniversalRenderPipelineGlobalSettings.asset`), Standalone batching (`ProjectSettings/ProjectSettings.asset`), `m_Enabled: 1` in `ProjectSettings/UnityConnectSettings.asset` (the project is linked to a Unity cloud project and the Editor turns it back on; Analytics, Ads, Purchasing and Performance reporting stay off).
- **Input System preloaded actions**: when a build starts the Input System adds its project-wide actions (`Assets/InputSystem_Actions.inputactions`) to `preloadedAssets` and saves the Player Settings; when it ends it removes them in memory only. The committed state is the clean one (`preloadedAssets: []`), and `Tools/Build Windows` saves the assets after the build to write it back.
- Company in the Player Settings is still `DefaultCompany`, kept for now: `Player.log` and the CSV of a build go to `AppData\LocalLow\DefaultCompany\King Bar\`.
- Not done: IL2CPP and code stripping, which would reduce the runtime part of the size.
