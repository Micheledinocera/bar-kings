---
tipo: info
aggiornato: 2026-10-29
verificato: 2026-10-29
consolidato: 2026-10-16
fonti:
  - raw/compiled/merge-rpg/2026-10-28-popup-e-config.md
  - raw/compiled/art-style/2026-10-25-implementazione.md
  - raw/compiled/preset-builder/2026-10-19-obiettivo.md
  - raw/compiled/build/2026-10-16-sistemazione.md
  - raw/compiled/build/2026-10-16-modifiche-non-salvate.md
  - raw/compiled/demo-1/2026-10-16-build.md
  - raw/compiled/demo-1/2026-10-16-obiettivo.md
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/stats/2026-10-16-sessione.md
  - raw/compiled/tribuna/2026-10-15-implementazione.md
  - raw/compiled/tribuna/2026-10-15-obiettivo.md
  - raw/compiled/panchina-reprise/2026-10-15-implementazione.md
  - raw/compiled/panchina-reprise/2026-10-15-obiettivo.md
  - raw/compiled/panchina-continue/2026-10-15-implementazione.md
  - raw/compiled/unity-mcp/2026-10-15-test-e-controllo.md
  - raw/compiled/panchina-start/2026-10-11-fix-path.md
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Known issues

Issues of the project code, grouped in blocks: one block per area meant to be fixed in one code feature. Planned order (study-new-implementations): blocks A (card generation), B (first-POC duplicates, removed with SampleScene), E (async and animations) and G (path into a hole) are done, and the build (**I**) is done except the company name; next cleanup and conventions (**F**; formatting is done), then **C** with **D** inside it. Mixed Italian/English identifiers are **not** an issue (`AGENTS.md`). Detail: [shared](./shared.md), [table](./table.md), [deck](./deck.md), [stats](./stats.md), [RPG](./rpg.md), [scenes](./scenes.md).

| Block | Items | Tags | Waits for |
|---|---|---|---|
| [C — Game state in hierarchy and statics](#block-c--game-state-in-hierarchy-and-statics) | 4 | debt, dead-code | — |
| [D — Input](#block-d--input) | 2 | debt, to-verify | — |
| [F — Cleanup and conventions](#block-f--cleanup-and-conventions) | 3 | debt | — |
| [H — Stats and simulation](#block-h--stats-and-simulation) | 2 | ux, duplicate | — |
| [I — Build](#block-i--build) | 1 | debt | — |
| [J — Bar scene HUD](#block-j--bar-scene-hud) | 3 | ux, debt | [Direzione artistica](../arte/direzione-artistica.md) |

## Block C — Game state in hierarchy and statics

- `debt` — Slot Transforms found by child order (`GetSlotByIndex`: `GetChild(0).GetChild(i)`); only used to parent cards, the table state is in [TableState](./table.md#tablestate). `Assets/Scripts/Shared/Table/TableDeckManager.cs` — [TableDeckManager](./table.md#tabledeckmanager)
- `dead-code` — `CardSlot.index` and `CardSlotContainer.CardSlotList` never read. `Assets/Scripts/Shared/Table/CardSlot.cs`, `Assets/Scripts/Shared/Table/CardSlotContainer.cs` — [CardSlot](./table.md#cardslot)
- `debt` — Static flag and singleton not reset on entering Play mode (`GameManager.isClickBlocked`, `TableDeckManager.instance`); safe only while Play mode reloads the domain. `Assets/Scripts/Shared/Table/TableDeckManager.cs`, `Assets/Scripts/Shared/GameManager.cs` — [scenes](./scenes.md#project-settings)
- `debt` — `GameManager` is a MonoBehaviour placed in every table scene but holds only a static field. `Assets/Scripts/Shared/GameManager.cs` — [GameManager](./shared.md#gamemanager)

## Block D — Input

- `debt` — Clicks go through `OnMouseDown`, which ignores the UI (a check of `EventSystem.IsPointerOverGameObject` stops clicks over the panels); `Assets/InputSystem_Actions.inputactions` is unused. Works today; to move to the Input System when the card view is rewritten. `Assets/Scripts/Shared/Table/TableCardController.cs` — [TableCardController](./table.md#tablecardcontroller)
- `to-verify` — `OnMouseDown` gets the click before the UI: the "Continua" button of the TribunaRpg fight popup also played the card below it, fixed with `TableDeckManager.TablePaused`. The options panel of the other scenes may have the same problem on the click that closes it. `Assets/Scripts/Shared/Table/TableCardController.cs` — [TableDeckManager](./table.md#tabledeckmanager)

## Block F — Cleanup and conventions

- `debt` — File names differ from class names (`CardDisplay.cs` → `CardDisplay3D`, …); no warning in Unity. [shared](./shared.md)
- `debt` — Magic numbers: grid layout of `SlotPosition` (deck and table numbers are in `DeckRules`, deck sizes in the presets). `Assets/Scripts/Shared/Table/TableDeckManager.cs` — [TableDeckManager](./table.md#tabledeckmanager)
- `debt` — `com.unity.ai.assistant` (the `unity-mcp` server) is a pre-release, `2.20.0-pre.2`: move to the stable version when it is out. `Packages/manifest.json` — [Accesso dell'agent all'Unity Editor](../progetto/accesso-editor-agent.md)

## Block H — Stats and simulation

- `ux` — The simulation stops while the Editor is in the background: it yields a frame every 50 games and the Editor runs no frames unfocused (`Application.runInBackground` off). Run it with the Editor focused, or run it without yielding; `Application.runInBackground = true` set at runtime during the Play session (not in the Player Settings) also keeps it running. `Assets/Scripts/Shared/Stats/TableSimulationRunner.cs` — [TableSimulationRunner](./stats.md#tablesimulationrunner)
- `duplicate` — `TableDeckManager.Log` and `TableSimulationRunner.Log` are the same helper (Console plus optional `GameLogView`). `Assets/Scripts/Shared/Table/TableDeckManager.cs`, `Assets/Scripts/Shared/Stats/TableSimulationRunner.cs` — [GameLogView](./stats.md#gamelogview)

## Block I — Build

- `debt` — Company in the Player Settings is `DefaultCompany`, which also names the folder of the build's logs and CSV; kept for now (user, 2026-10-16). `ProjectSettings/ProjectSettings.asset` — [scenes](./scenes.md#build)

## Block J — Bar scene HUD

Only if the Bar style is kept ([Direzione artistica](../arte/direzione-artistica.md), decision in analysis).

- `ux` — "Re scoperti" and the tombola card are refreshed at the end of a path, while the receipt and the road follow the cards one by one. `Assets/Scripts/Shared/Hud/KingCounterView.cs`, `Assets/Scripts/Shared/Hud/TombolaCardView.cs` — [KingCounterView](./hud.md#kingcounterview)
- `ux` — The grid lines of the tombola card (2 px gaps) look uneven when the Canvas is scaled down. `Assets/Prefab/TombolaCell.prefab` — [TombolaCardView](./hud.md#tombolacardview-tombolacellview)
- `debt` — The Tribuna buttons, the options panel and the game log are restyled with overrides on the scene instances, not with Prefab Variants. `Assets/Scenes/Bar.unity` — [scenes](./scenes.md#bar)
