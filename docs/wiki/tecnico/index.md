# Technical

The project's own code and content: scripts in `Assets/Scripts/`, scenes, prefabs, data assets. Unity and plugin behaviour (Odin Inspector, DOTween, UniTask, TextMeshPro) is in their official docs, not here. Back to the [wiki index](../index.md).

- [Shared scripts](./shared.md): `Assets/Scripts/Shared/`: `CardData`, `CardDataInGame`, `CardDisplay3D`, the card generator and the first POC (`DeckManager3D`, `CardController`, `CardAnimations`), `GameManager`, `CameraOrbit`
- [Panchina scripts](./panchina.md): `Assets/Scripts/Panchina/`: copies of the shared POC with the panchina deck (duplicates, 40 cards) and path rule, `CardSlot`, `CardSlotContainer`
- [Scenes, prefabs and data assets](./scenes.md): the 4 scenes (3 identical first-POC copies + Panchina), card and slot prefabs, the 40 card assets, project settings the code depends on
- [Known issues](./known-issues.md): issues grouped in blocks A–F, one code feature each, with the planned fixing order
