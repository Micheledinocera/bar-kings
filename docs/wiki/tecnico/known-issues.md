---
tipo: info
aggiornato: 2026-10-16
verificato: 2026-10-16
consolidato: 2026-10-15
fonti:
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

Issues of the project code, grouped in blocks: one block per area meant to be fixed in one code feature. Planned order (study-new-implementations): blocks A (card generation), E (async and animations) and G (path into a hole) are done; next cleanup and conventions (**F**; formatting is done), then a shared core (**B** + **C**, with **D** inside it) before working on tribuna and index>40. Mixed Italian/English identifiers are **not** an issue (`AGENTS.md`). Detail: [shared](./shared.md), [table](./table.md), [deck](./deck.md), [stats](./stats.md), [scenes](./scenes.md).

| Block | Items | Tags | Waits for |
|---|---|---|---|
| [B — Shared / Panchina duplication](#block-b--shared--panchina-duplication) | 3 | duplicate, debt | — |
| [C — Game state in hierarchy and statics](#block-c--game-state-in-hierarchy-and-statics) | 4 | debt, dead-code | — |
| [D — Input](#block-d--input) | 1 | debt | — |
| [F — Cleanup and conventions](#block-f--cleanup-and-conventions) | 4 | dead-code, debt | — |
| [H — Stats and simulation](#block-h--stats-and-simulation) | 2 | ux, duplicate | — |

## Block B — Shared / Panchina duplication

- `duplicate` — `DeckManager3D`, `CardController`, `CardAnimations` (first POC) copied into `Assets/Scripts/Shared/Table/` as `TableDeckManager`, `TableCardController`, `TableCardAnimations`; panchina and tribuna share the copies, the first POC remains for `SampleScene` and `ScenaIndex40`. [table](./table.md)
- `duplicate` — `Assets/Prefab/3DCardTable.prefab` is a copy of `Assets/Prefab/3DCard.prefab`, not a Prefab Variant. [scenes](./scenes.md#prefabs)
- `debt` — `Assets/Scripts/Shared/` holds the first POC (`DeckManager3D`, `CardController`, `CardAnimations`), used only by the twin scenes. [shared](./shared.md)

## Block C — Game state in hierarchy and statics

- `debt` — Slot Transforms found by child order (`GetSlotByIndex`: `GetChild(0).GetChild(i)`); only used to parent cards, the table state is in [TableState](./table.md#tablestate). `Assets/Scripts/Shared/Table/TableDeckManager.cs` — [TableDeckManager](./table.md#tabledeckmanager)
- `dead-code` — `CardSlot.index` and `CardSlotContainer.CardSlotList` never read. `Assets/Scripts/Shared/Table/CardSlot.cs`, `Assets/Scripts/Shared/Table/CardSlotContainer.cs` — [CardSlot](./table.md#cardslot)
- `debt` — Public static lists, flag and singletons not reset on entering Play mode (the first POC's lists are never cleared; the table scenes keep only the `TableDeckManager.instance` singleton); safe only while Play mode reloads the domain. `Assets/Scripts/Shared/Cards3D/DeckManager.cs`, `Assets/Scripts/Shared/Table/TableDeckManager.cs`, `Assets/Scripts/Shared/GameManager.cs` — [scenes](./scenes.md#project-settings)
- `debt` — `GameManager` is a MonoBehaviour placed in every scene but holds only a static field. `Assets/Scripts/Shared/GameManager.cs` — [GameManager](./shared.md#gamemanager)

## Block D — Input

- `debt` — Clicks go through `OnMouseDown`; `Assets/InputSystem_Actions.inputactions` is unused. Works today; to move to the Input System when the card view is rewritten. `Assets/Scripts/Shared/Cards3D/CardController.cs`, `Assets/Scripts/Shared/Table/TableCardController.cs` — [CardController](./shared.md#cardcontroller)

## Block F — Cleanup and conventions

- `dead-code` — `Assets/Scripts/ScriptProva.cs` (test script) and `Assets/Scripts/Shared/Camera/CameraOrbit.cs` (in no scene, legacy input) — [CameraOrbit](./shared.md#cameraorbit)
- `debt` — File names differ from class names (`CardDisplay.cs` → `CardDisplay3D`, …); no warning in Unity. [shared](./shared.md)
- `debt` — Magic numbers: grid layout of `SlotPosition` (deck and table numbers are in `DeckRules`, deck sizes in the presets). `Assets/Scripts/Shared/Table/TableDeckManager.cs` — [TableDeckManager](./table.md#tabledeckmanager)
- `debt` — `com.unity.ai.assistant` (the `unity-mcp` server) is a pre-release, `2.20.0-pre.2`: move to the stable version when it is out. `Packages/manifest.json` — [Accesso dell'agent all'Unity Editor](../progetto/accesso-editor-agent.md)

## Block H — Stats and simulation

- `ux` — The simulation stops while the Editor is in the background: it yields a frame every 50 games and the Editor runs no frames unfocused (`Application.runInBackground` off). Run it with the Editor focused, or run it without yielding. `Assets/Scripts/Shared/Stats/TableSimulationRunner.cs` — [TableSimulationRunner](./stats.md#tablesimulationrunner)
- `duplicate` — `TableDeckManager.Log` and `TableSimulationRunner.Log` are the same helper (Console plus optional `GameLogView`). `Assets/Scripts/Shared/Table/TableDeckManager.cs`, `Assets/Scripts/Shared/Stats/TableSimulationRunner.cs` — [GameLogView](./stats.md#gamelogview)
