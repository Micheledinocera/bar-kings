# Technical

The project's own code and content: scripts in `Assets/Scripts/`, scenes, prefabs, data assets. Unity and plugin behaviour (Odin Inspector, DOTween, UniTask, TextMeshPro) is in their official docs, not here. Back to the [wiki index](../index.md).

- [Shared scripts](./shared.md): `Assets/Scripts/Shared/`: `CardData`, `CardDatabase`, `CardDataInGame`, `CardDisplay3D`, the card generator (editor menu) and the first POC (`DeckManager3D`, `CardController`, `CardAnimations`), `GameManager`, `CameraOrbit`
- [Table scripts](./table.md): `Assets/Scripts/Shared/Table/`, shared by the Panchina and Tribuna scenes: `TableState` (logical table state and path rule: duplicates stacked, holes; bench that fills holes face down), `TableDeckManager` (40 table cards with the 4 kings, duplicates beyond 40 to the bench or discarded, reset), controller, animations, `CardSlot`, `CardSlotContainer`
- [Scenes, prefabs and data assets](./scenes.md): the 4 scenes (Panchina and ScenaTribuna on the table scripts, SampleScene and ScenaIndex40 first-POC copies), card and slot prefabs, the 40 card assets and the `CardDatabase`, project settings the code depends on
- [Known issues](./known-issues.md): issues grouped in blocks B–F (A done), one code feature each, with the planned fixing order
