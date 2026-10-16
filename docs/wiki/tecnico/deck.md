---
tipo: info
aggiornato: 2026-10-16
verificato: 2026-10-16
fonti:
  - raw/compiled/stats/2026-10-15-obiettivo.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/stats/2026-10-16-sessione.md
---

# Deck scripts

Scripts in `Assets/Scripts/Shared/Deck/` and the preset generator in `Assets/Scripts/Shared/Editor/`: the deck of one game of the table scenes (Panchina, Tribuna), built from a **preset** with a **seed**. Plain C# except the `DeckPreset` asset, so the [autoplay](./stats.md#gamesimulator) builds decks exactly as the scenes do. The presets are the deck sizes being tried for [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [DeckRules](#deckrules) | `Assets/Scripts/Shared/Deck/DeckRules.cs` | static class (constants) |
| [DeckPreset](#deckpreset) | `Assets/Scripts/Shared/Deck/DeckPreset.cs` | ScriptableObject |
| [DeckBuilder](#deckbuilder) | `Assets/Scripts/Shared/Deck/DeckBuilder.cs` | static class |
| [DeckDeal](#deckdeal) | `Assets/Scripts/Shared/Deck/DeckDeal.cs` | plain C# class |
| [DeckPresetGenerator](#deckpresetgenerator) | `Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs` | static editor class (menu item) |

## DeckRules

`Assets/Scripts/Shared/Deck/DeckRules.cs`. Numbers of the deck and the table: `TableSlots = 40`, `KingValue = 10`, `KingCount = 4`, `BaseNonKingCards = 36`.

- **Used by**: [DeckBuilder](#deckbuilder), [DeckPreset](#deckpreset), [DeckDeal](#deckdeal), [TableState](./table.md#tablestate) (`FaceUpKings`), [GameLogFormatter](./stats.md#gamelogformatter), [GameSimulator](./stats.md#gamesimulator).

## DeckPreset

`Assets/Scripts/Shared/Deck/DeckPreset.cs`. A deck to try: the 36 non-king cards with `removedCards` taken out (0–36) and `duplicates` copies added; the 4 kings are always added on top. `NonKingCards` (read-only in the inspector) = 36 − removed + duplicates. **Which** cards are removed or duplicated is drawn at every game by [DeckBuilder](#deckbuilder). Create menu: `Carte/Deck Preset`; `Set` exists only in the Editor, for the generator.

- **Assets** in `Assets/Instances/DeckPresets/` (created by [DeckPresetGenerator](#deckpresetgenerator)):

  | Asset | Removed | Duplicates | Non-king cards | Deck |
  |---|---|---|---|---|
  | `Preset32.asset` | 9 | 5 | 32 | 36 (4 holes on the table) |
  | `Preset45.asset` | 5 | 14 | 45 | 49 |
  | `Preset60.asset` | 5 | 29 | 60 | 64 |
  | `Preset80.asset` | 5 | 49 | 80 | 84 |

- **Used by**: [TableDeckManager](./table.md#tabledeckmanager) (`deckPreset`, `Preset45` in both scenes), [TableSimulationRunner](./stats.md#tablesimulationrunner) (`presets`, all four in the `SimulationButton` prefab), [GameSimulator](./stats.md#gamesimulator).

## DeckBuilder

`Assets/Scripts/Shared/Deck/DeckBuilder.cs`. `Build(baseCards, removedCount, duplicateCount, random)` returns the [DeckDeal](#deckdeal) of one game; the same `System.Random` seed gives the same deal.

- Checks that the base cards have exactly 4 kings (otherwise throws), and the counts (throws when out of range or with nothing left to duplicate).
- Removes `removedCount` distinct non-king cards at random; adds `duplicateCount` copies of the remaining ones, **spread out**: every card gets a copy before any gets a second one (each copy a new [CardDataInGame](./shared.md#carddataingame) instance, never a king).
- Shuffles; the first 36 non-king cards go to the table with the 4 kings, on random slots; with fewer cards the free slots are **holes** (`null`). The non-king cards beyond the table are the **excess**, in draw order.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator).

## DeckDeal

`Assets/Scripts/Shared/Deck/DeckDeal.cs`. Result of [DeckBuilder](#deckbuilder): `TableSlots` (card dealt face down to each of the 40 slots, `null` for a hole), `Excess` (bench of the panchina, discarded in the tribuna), `Removed`, `Duplicates`; computed `TableCards`, `Holes`, `TableDuplicates` (table cards whose slot is already claimed by another copy of the same card), `DeckSize` (kings included).

- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameLogFormatter](./stats.md#gamelogformatter), [SimulatedGame](./stats.md#simulatedgame-simulatedpath), [SimulationCsv](./stats.md#simulationcsv).

## DeckPresetGenerator

`Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs`. Editor only. Menu `Tools/Genera Preset Mazzo`: creates the four [DeckPreset](#deckpreset) assets in `Assets/Instances/DeckPresets/` (and the folder); presets that already exist are kept as they are (GUIDs unchanged). The four presets are a literal table in the class.
