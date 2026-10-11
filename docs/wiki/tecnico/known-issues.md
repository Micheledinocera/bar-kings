---
tipo: info
aggiornato: 2026-10-11
verificato: 2026-10-11
fonti:
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Known issues

Issues of the project code, grouped in blocks: one block per area meant to be fixed in one code feature. Planned order (study-new-implementations): block A (card generation) is done; next cleanup and conventions (**F**; formatting is done), then a shared core (**B** + **C**, with **D** and **E** inside it) before working on tribuna and index>40. Mixed Italian/English identifiers are **not** an issue (`AGENTS.md`). Detail: [shared](./shared.md), [panchina](./panchina.md), [scenes](./scenes.md).

| Block | Items | Tags | Waits for |
|---|---|---|---|
| [B — Shared / Panchina duplication](#block-b--shared--panchina-duplication) | 3 | duplicate, debt | — |
| [C — Game state in hierarchy and statics](#block-c--game-state-in-hierarchy-and-statics) | 4 | debt, dead-code | — |
| [D — Input](#block-d--input) | 1 | debt | — |
| [E — Async and animations](#block-e--async-and-animations) | 4 | debt, bug, dead-code, performance | — |
| [F — Cleanup and conventions](#block-f--cleanup-and-conventions) | 3 | dead-code, debt | — |

## Block B — Shared / Panchina duplication

- `duplicate` — `DeckManager`, `CardController`, `CardAnimations` copied into `Assets/Scripts/Panchina/` with a `Panchina` suffix; tribuna and index>40 would add two more copies. [panchina](./panchina.md)
- `duplicate` — `Assets/Prefab/3DCardPanchina.prefab` is a copy of `Assets/Prefab/3DCard.prefab`, not a Prefab Variant. [scenes](./scenes.md#prefabs)
- `debt` — `Assets/Scripts/Shared/` holds the first POC (`DeckManager3D`, `CardController`, `CardAnimations`), used only by the twin scenes. [shared](./shared.md)

## Block C — Game state in hierarchy and statics

- `debt` — Table state read from the Transform hierarchy (`GetChild(0).GetChild(i)`, then `GetChild(1)`); cards don't change parent during a path, so the hierarchy no longer matches the table after the first path. `Assets/Scripts/Panchina/DeckManager.cs`, `Assets/Scripts/Panchina/CardAnimations.cs` — [DeckManagerPanchina](./panchina.md#deckmanagerpanchina)
- `dead-code` — `CardSlot.index` and `CardSlotContainer.CardSlotList` never read. `Assets/Scripts/Panchina/CardSlot.cs`, `Assets/Scripts/Panchina/CardSlotContainer.cs` — [CardSlot](./panchina.md#cardslot)
- `debt` — Public static lists, flag and singleton never cleared; safe only while Play mode reloads the domain. `Assets/Scripts/Shared/Cards3D/DeckManager.cs`, `Assets/Scripts/Panchina/DeckManager.cs`, `Assets/Scripts/Shared/GameManager.cs` — [scenes](./scenes.md#project-settings)
- `debt` — `GameManager` is a MonoBehaviour placed in every scene but holds only a static field. `Assets/Scripts/Shared/GameManager.cs` — [GameManager](./shared.md#gamemanager)

## Block D — Input

- `debt` — Clicks go through `OnMouseDown`; `Assets/InputSystem_Actions.inputactions` is unused. Works today; to move to the Input System when the card view is rewritten. `Assets/Scripts/Shared/Cards3D/CardController.cs`, `Assets/Scripts/Panchina/CardController.cs` — [CardController](./shared.md#cardcontroller)

## Block E — Async and animations

- `debt` — `async void` and `System.Threading.Tasks.Task` instead of UniTask; the click path has no cancellation token. `Assets/Scripts/Shared/Animations/CardAnimations.cs`, `Assets/Scripts/Panchina/CardAnimations.cs` — [CardAnimations](./shared.md#cardanimations)
- `bug` — `MoveCardsAtom` sequences have no `SetLink`: tweens outlive a destroyed card. Same files — [CardAnimations](./shared.md#cardanimations)
- `dead-code` — `FlipAnimation` never called; unused `using`s. Same files — [CardAnimations](./shared.md#cardanimations)
- `performance` — `meshRenderer.material` creates a material instance per card, never destroyed. `Assets/Scripts/Shared/Cards3D/CardDisplay.cs` — [CardDisplay3D](./shared.md#carddisplay3d)

## Block F — Cleanup and conventions

- `dead-code` — `Assets/Scripts/ScriptProva.cs` (test script) and `Assets/Scripts/Shared/Camera/CameraOrbit.cs` (in no scene, legacy input) — [CameraOrbit](./shared.md#cameraorbit)
- `debt` — File names differ from class names (`CardDisplay.cs` → `CardDisplay3D`, …); no warning in Unity. [shared](./shared.md)
- `debt` — Magic numbers: grid layout, deck size, duplicates count, king value. `Assets/Scripts/Panchina/DeckManager.cs` — [DeckManagerPanchina](./panchina.md#deckmanagerpanchina)
