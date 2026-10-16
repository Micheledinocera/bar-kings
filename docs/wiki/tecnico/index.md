# Technical

The project's own code and content: scripts in `Assets/Scripts/`, scenes, prefabs, data assets. Unity and plugin behaviour (Odin Inspector, DOTween, UniTask, TextMeshPro) is in their official docs, not here. Back to the [wiki index](../index.md).

- [Shared scripts](./shared.md): `Assets/Scripts/Shared/`: `CardData`, `CardDatabase`, `CardDataInGame`, `CardDisplay3D`, the card generator (editor menu) and the first POC (`DeckManager3D`, `CardController`, `CardAnimations`), `GameManager`, `CameraOrbit`
- [Table scripts](./table.md): `Assets/Scripts/Shared/Table/`, shared by the Panchina and Tribuna scenes: `TableState` (logical table state and path rule: duplicates stacked, holes; bench that fills holes face down), `TableDeckManager` (deals a deck preset with a seed, bench or discard beyond 40, game log, reset), controller, animations, `CardSlot`, `CardSlotContainer`
- [Deck scripts](./deck.md): `Assets/Scripts/Shared/Deck/`: `DeckRules`, `DeckPreset` (the 4 presets: 32, 45, 60, 80 non-king cards), `DeckBuilder` (seeded deck and deal), `DeckDeal`, and the preset generator (editor menu)
- [Stats scripts](./stats.md): `Assets/Scripts/Shared/Stats/`: game log (`GameLogFormatter`, `GameLogView`), autoplay simulation (`GameSimulator`, `SimulatedGame`, `SimulatedPath`) and its CSV output (`SimulationCsv`, `TableSimulationRunner`, the Simula button)
- [Scenes, prefabs and data assets](./scenes.md): the 4 scenes (Panchina and ScenaTribuna on the table scripts, SampleScene and ScenaIndex40 first-POC copies), card, slot, game log and simulation button prefabs, the 40 card assets, the `CardDatabase` and the deck presets, project settings the code depends on
- [Known issues](./known-issues.md): issues grouped in blocks B–H (A, E, G done), one code feature each, with the planned fixing order
