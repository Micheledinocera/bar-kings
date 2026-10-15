# Technical

The project's own code and content: scripts in `Assets/Scripts/`, scenes, prefabs, data assets. Unity and plugin behaviour (Odin Inspector, DOTween, UniTask, TextMeshPro) is in their official docs, not here. Back to the [wiki index](../index.md).

- [Shared scripts](./shared.md): `Assets/Scripts/Shared/`: `CardData`, `CardDatabase`, `CardDataInGame`, `CardDisplay3D`, the card generator (editor menu) and the first POC (`DeckManager3D`, `CardController`, `CardAnimations`), `GameManager`, `CameraOrbit`
- [Panchina scripts](./panchina.md): `Assets/Scripts/Panchina/`: `PanchinaTable` (logical table state and path rule: duplicates stacked, holes), panchina deck that fills holes face down; deck manager (40 table cards with the 4 kings, duplicates beyond 40 to the panchina, reset) and adjustable animation speed, controller, animations, `CardSlot`, `CardSlotContainer`
- [Scenes, prefabs and data assets](./scenes.md): the 4 scenes (3 identical first-POC copies + Panchina), card and slot prefabs, the 40 card assets and the `CardDatabase`, project settings the code depends on
- [Known issues](./known-issues.md): issues grouped in blocks B–F (A done), one code feature each, with the planned fixing order
