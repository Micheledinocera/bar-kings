---
tipo: info
aggiornato: 2026-10-15
verificato: 2026-10-15
consolidato: 2026-10-11
fonti:
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

Scenes in `Assets/Scenes/`, prefabs in `Assets/Prefab/`, card assets in `Assets/Instances/Cards/`, and the project settings that the code depends on. Scripts: [shared](./shared.md), [table](./table.md).

The three scenes of the [deck modification ideas](../business/modifica-del-mazzo.md) (`Panchina`, `ScenaTribuna`, `ScenaIndex40`) are **temporary**: they let each idea be explored without breaking the previous version. When one idea is chosen, the other scenes and the code only they use are deleted.

| Scene | Path | Deck manager | Card prefab |
|---|---|---|---|
| [SampleScene](#samplescene-scenaindex40) | `Assets/Scenes/SampleScene.unity` | [DeckManager3D](./shared.md#deckmanager3d) | `3DCard` |
| [ScenaIndex40](#samplescene-scenaindex40) | `Assets/Scenes/ScenaIndex40.unity` | [DeckManager3D](./shared.md#deckmanager3d) | `3DCard` |
| [ScenaTribuna](#panchina-and-scenatribuna) | `Assets/Scenes/ScenaTribuna.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Discard`) | `3DCardTable` |
| [Panchina](#panchina-and-scenatribuna) | `Assets/Scenes/Panchina.unity` | [TableDeckManager](./table.md#tabledeckmanager) (`excessCards = Bench`) | `3DCardTable` |

## SampleScene, ScenaIndex40

Identical copies of the **first POC** (plain 4-kings solitaire): camera, light, a `GameManager` GameObject ([GameManager](./shared.md#gamemanager)) and [DeckManager3D](./shared.md#deckmanager3d), which deals the 40 cards onto a 4×10 grid at Play. Nothing specific to index>40 yet.

All four scenes are in the Build Settings (`ProjectSettings/EditorBuildSettings.asset`): `SampleScene`, `Panchina`, `ScenaTribuna`, `ScenaIndex40`.

## Panchina and ScenaTribuna

Same structure, same [table scripts](./table.md); they differ only in `TableDeckManager.excessCards` (`Bench` in Panchina, `Discard` in ScenaTribuna). ScenaTribuna's slots, UI and EventSystem were copied from Panchina. Main GameObjects:

- `Tavolo`: the table;
- `SlotContainer`: [CardSlotContainer](./table.md#cardslotcontainer) with 40 instances of `Assets/Prefab/CardSlot.prefab` ([CardSlot](./table.md#cardslot)); must be child 0 of the deck manager's `deckPosition` (`CardsContainer`), with the slots in grid order, because [TableDeckManager](./table.md#tabledeckmanager) finds the slot Transforms by child index to parent the cards (the table state itself is in [TableState](./table.md#tablestate));
- `CardsContainer`, `GameManager` (with [GameManager](./shared.md#gamemanager) and [TableDeckManager](./table.md#tabledeckmanager)), `Main Camera`, `Directional Light`; many GameObjects have no name;
- `UI`: Canvas (Screen Space Overlay, scaled with screen size) with `ResetButton` (TextMeshPro text "Reset", top right), whose persistent `OnClick` calls `TableDeckManager.ResetTable` on the scene's own `GameManager` ([TableDeckManager](./table.md#tabledeckmanager));
- `EventSystem` with `InputSystemUIInputModule` (the Input System UI module, the only input handler enabled).

## Prefabs

- `Assets/Prefab/3DCard.prefab`: root with [CardDisplay3D](./shared.md#carddisplay3d) and [CardController](./shared.md#cardcontroller); children `Fronte` and `Retro` (`Mat_Carta_Fronte`, `Mat_Carta_Retro`).
- `Assets/Prefab/3DCardTable.prefab` (was `3DCardPanchina`): a **copy** of `3DCard` (not a Prefab Variant) with [TableCardController](./table.md#tablecardcontroller) instead of `CardController`; used by Panchina and ScenaTribuna.
- `Assets/Prefab/CardSlot.prefab`: `CardSlot` root with a `Cube` child (`Mat_Card_Slot`).

## Card assets

`Assets/Instances/Cards/<valore>_<Seme>.asset`: the 40 [CardData](./shared.md#carddata), fronts from `Assets/Images/Cards/Fronte/` (`01_…` to `40_…jpg`: Denari, Coppe, Spade, Bastoni; ace to ten each). Created only by the menu `Tools/Genera Carte Napoletane` ([CardGenerator](./shared.md#cardgenerator)), which skips existing ones; Play no longer touches them.

`Assets/Instances/CardDatabase.asset`: the [CardDatabase](./shared.md#carddatabase) with the 40 cards in table order. Assigned to the `deckDatabase` field of the deck manager in **all four scenes**; a new scene with a deck manager needs it too.

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
