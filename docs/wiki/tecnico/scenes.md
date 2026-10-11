---
tipo: info
aggiornato: 2026-10-11
verificato: 2026-10-11
fonti:
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/card-generator/2026-10-11-modifiche-editor.md
  - raw/compiled/git-line-endings/2026-10-11-obiettivo.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Scenes, prefabs and data assets

Scenes in `Assets/Scenes/`, prefabs in `Assets/Prefab/`, card assets in `Assets/Instances/Cards/`, and the project settings that the code depends on. Scripts: [shared](./shared.md), [panchina](./panchina.md).

The three scenes of the [deck modification ideas](../business/modifica-del-mazzo.md) (`Panchina`, `ScenaTribuna`, `ScenaIndex40`) are **temporary**: they let each idea be explored without breaking the previous version. When one idea is chosen, the other scenes and the code only they use are deleted.

| Scene | Path | Deck manager | Card prefab |
|---|---|---|---|
| [SampleScene](#samplescene-scenaindex40-scenatribuna) | `Assets/Scenes/SampleScene.unity` | [DeckManager3D](./shared.md#deckmanager3d) | `3DCard` |
| [ScenaIndex40](#samplescene-scenaindex40-scenatribuna) | `Assets/Scenes/ScenaIndex40.unity` | [DeckManager3D](./shared.md#deckmanager3d) | `3DCard` |
| [ScenaTribuna](#samplescene-scenaindex40-scenatribuna) | `Assets/Scenes/ScenaTribuna.unity` | [DeckManager3D](./shared.md#deckmanager3d) | `3DCard` |
| [Panchina](#panchina) | `Assets/Scenes/Panchina.unity` | [DeckManagerPanchina](./panchina.md#deckmanagerpanchina) | `3DCardPanchina` |

## SampleScene, ScenaIndex40, ScenaTribuna

Identical copies of the **first POC** (plain 4-kings solitaire): camera, light, a `GameManager` GameObject ([GameManager](./shared.md#gamemanager)) and [DeckManager3D](./shared.md#deckmanager3d), which deals the 40 cards onto a 4×10 grid at Play. Nothing specific to tribuna or index>40 yet. Only `SampleScene` is in the Build Settings.

## Panchina

The only scene worked on. Main GameObjects:

- `Tavolo`: the table;
- `SlotContainer`: [CardSlotContainer](./panchina.md#cardslotcontainer) with 40 instances of `Assets/Prefab/CardSlot.prefab` ([CardSlot](./panchina.md#cardslot)); must be child 0 of the deck manager's `deckPosition`, and every slot's child 0 must be its `Cube`, because [DeckManagerPanchina](./panchina.md#deckmanagerpanchina) finds slots and cards by child index;
- `CardsContainer`, `GameManager`, `Main Camera`, `Directional Light`; many GameObjects have no name.

## Prefabs

- `Assets/Prefab/3DCard.prefab`: root with [CardDisplay3D](./shared.md#carddisplay3d) and [CardController](./shared.md#cardcontroller); children `Fronte` and `Retro` (`Mat_Carta_Fronte`, `Mat_Carta_Retro`).
- `Assets/Prefab/3DCardPanchina.prefab`: a **copy** of `3DCard` (not a Prefab Variant) with [CardControllerPanchina](./panchina.md#cardcontrollerpanchina) instead of `CardController`.
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
