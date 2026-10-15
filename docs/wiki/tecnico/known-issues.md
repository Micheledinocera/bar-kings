---
tipo: info
aggiornato: 2026-10-15
verificato: 2026-10-15
consolidato: 2026-10-15
fonti:
  - raw/compiled/unity-mcp/2026-10-15-test-e-controllo.md
  - raw/compiled/panchina-start/2026-10-11-fix-path.md
  - raw/compiled/card-generator/2026-10-11-obiettivo.md
  - raw/compiled/study-new-implementations/2026-10-11-analisi-codice.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Known issues

Issues of the project code, grouped in blocks: one block per area meant to be fixed in one code feature. Planned order (study-new-implementations): block G (path into a hole) first, as soon as its rule is set; block A (card generation) is done; next cleanup and conventions (**F**; formatting is done), then a shared core (**B** + **C**, with **D** and **E** inside it) before working on tribuna and index>40. Mixed Italian/English identifiers are **not** an issue (`AGENTS.md`). Detail: [shared](./shared.md), [panchina](./panchina.md), [scenes](./scenes.md).

| Block | Items | Tags | Waits for |
|---|---|---|---|
| [B — Shared / Panchina duplication](#block-b--shared--panchina-duplication) | 3 | duplicate, debt | — |
| [C — Game state in hierarchy and statics](#block-c--game-state-in-hierarchy-and-statics) | 4 | debt, dead-code | — |
| [D — Input](#block-d--input) | 1 | debt | — |
| [E — Async and animations](#block-e--async-and-animations) | 5 | debt, bug, dead-code, performance | — |
| [F — Cleanup and conventions](#block-f--cleanup-and-conventions) | 4 | dead-code, debt | — |
| [G — Panchina path rules](#block-g--panchina-path-rules) | 1 | bug, decision | [Modifica del mazzo](../business/modifica-del-mazzo.md) |

## Block B — Shared / Panchina duplication

- `duplicate` — `DeckManager`, `CardController`, `CardAnimations` copied into `Assets/Scripts/Panchina/` with a `Panchina` suffix; tribuna and index>40 would add two more copies. [panchina](./panchina.md)
- `duplicate` — `Assets/Prefab/3DCardPanchina.prefab` is a copy of `Assets/Prefab/3DCard.prefab`, not a Prefab Variant. [scenes](./scenes.md#prefabs)
- `debt` — `Assets/Scripts/Shared/` holds the first POC (`DeckManager3D`, `CardController`, `CardAnimations`), used only by the twin scenes. [shared](./shared.md)

## Block C — Game state in hierarchy and statics

- `debt` — Slot Transforms found by child order (`GetSlotByIndex`: `GetChild(0).GetChild(i)`); only used to parent cards, the table state is in [PanchinaTable](./panchina.md#panchinatable). `Assets/Scripts/Panchina/DeckManager.cs` — [DeckManagerPanchina](./panchina.md#deckmanagerpanchina)
- `dead-code` — `CardSlot.index` and `CardSlotContainer.CardSlotList` never read. `Assets/Scripts/Panchina/CardSlot.cs`, `Assets/Scripts/Panchina/CardSlotContainer.cs` — [CardSlot](./panchina.md#cardslot)
- `debt` — Public static lists, flag and singleton never cleared; safe only while Play mode reloads the domain. `Assets/Scripts/Shared/Cards3D/DeckManager.cs`, `Assets/Scripts/Panchina/DeckManager.cs`, `Assets/Scripts/Shared/GameManager.cs` — [scenes](./scenes.md#project-settings)
- `debt` — `GameManager` is a MonoBehaviour placed in every scene but holds only a static field. `Assets/Scripts/Shared/GameManager.cs` — [GameManager](./shared.md#gamemanager)

## Block D — Input

- `debt` — Clicks go through `OnMouseDown`; `Assets/InputSystem_Actions.inputactions` is unused. Works today; to move to the Input System when the card view is rewritten. `Assets/Scripts/Shared/Cards3D/CardController.cs`, `Assets/Scripts/Panchina/CardController.cs` — [CardController](./shared.md#cardcontroller)

## Block E — Async and animations

- `debt` — `async void` and `System.Threading.Tasks.Task` instead of UniTask; the click path has no cancellation token. `Assets/Scripts/Shared/Animations/CardAnimations.cs`, `Assets/Scripts/Shared/Cards3D/CardController.cs` (Panchina: `Start` and `OnMouseDown` are `async void`, `MoveCards` returns `System.Threading.Tasks.Task`; the path is a cancellable `UniTask`) — [CardAnimations](./shared.md#cardanimations)
- `bug` — Panchina: when the scene is destroyed during an animation, the `OperationCanceledException` reaches the `async void` entry points (`Start`, `OnMouseDown`) and is logged as an error. `Assets/Scripts/Panchina/DeckManager.cs`, `Assets/Scripts/Panchina/CardController.cs` — [DeckManagerPanchina](./panchina.md#deckmanagerpanchina)
- `bug` — `MoveCardsAtom` sequences have no `SetLink`: tweens outlive a destroyed card. `Assets/Scripts/Shared/Animations/CardAnimations.cs` — [CardAnimations](./shared.md#cardanimations)
- `dead-code` — `FlipAnimation` never called; unused `using`s. `Assets/Scripts/Shared/Animations/CardAnimations.cs`, `Assets/Scripts/Panchina/CardAnimations.cs` — [CardAnimations](./shared.md#cardanimations)
- `performance` — `meshRenderer.material` creates a material instance per card, never destroyed. `Assets/Scripts/Shared/Cards3D/CardDisplay.cs` — [CardDisplay3D](./shared.md#carddisplay3d)

## Block F — Cleanup and conventions

- `dead-code` — `Assets/Scripts/ScriptProva.cs` (test script) and `Assets/Scripts/Shared/Camera/CameraOrbit.cs` (in no scene, legacy input) — [CameraOrbit](./shared.md#cameraorbit)
- `debt` — File names differ from class names (`CardDisplay.cs` → `CardDisplay3D`, …); no warning in Unity. [shared](./shared.md)
- `debt` — Magic numbers: grid layout, deck size, duplicates count, king value. `Assets/Scripts/Panchina/DeckManager.cs` — [DeckManagerPanchina](./panchina.md#deckmanagerpanchina)
- `debt` — `com.unity.ai.assistant` (the `unity-mcp` server) is a pre-release, `2.20.0-pre.2`: move to the stable version when it is out. `Packages/manifest.json` — [Accesso dell'agent all'Unity Editor](../progetto/accesso-editor-agent.md)

## Block G — Panchina path rules

- `bug` `decision` — A path whose card in hand belongs to a **hole** (a slot left empty by a path stacked on a duplicate) throws `ArgumentOutOfRangeException`: `PlayPath` takes the top card of an empty stack. Reproduced in the Editor with a 4-slot table; frequent in play (9 duplicates). The fix needs the rule for a card reaching a hole (open question of [Modifica del mazzo](../business/modifica-del-mazzo.md#domande-aperte)). `Assets/Scripts/Panchina/PanchinaTable.cs` — [PanchinaTable](./panchina.md#panchinatable)
