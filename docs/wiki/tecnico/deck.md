---
tipo: info
aggiornato: 2026-10-16
verificato: 2026-10-16
fonti:
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/index40/2026-10-16-obiettivo.md
  - raw/compiled/stats/2026-10-15-obiettivo.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/stats/2026-10-16-sessione.md
---

# Deck scripts

Scripts in `Assets/Scripts/Shared/Deck/` and the preset generator in `Assets/Scripts/Shared/Editor/`: the deck of one game of the table scenes (Panchina, Tribuna, Index40), built from a **preset** with a **seed**, dealt as the scene's [ExcessCards](#excesscards) mode says. Plain C# except the `DeckPreset` asset, so the [autoplay](./stats.md#gamesimulator) builds decks exactly as the scenes do. The presets are the deck sizes being tried for [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [DeckRules](#deckrules) | `Assets/Scripts/Shared/Deck/DeckRules.cs` | static class (constants) |
| [ExcessCards](#excesscards) | `Assets/Scripts/Shared/Deck/ExcessCards.cs` | enum |
| [DeckPreset](#deckpreset) | `Assets/Scripts/Shared/Deck/DeckPreset.cs` | ScriptableObject |
| [DeckBuilder](#deckbuilder) | `Assets/Scripts/Shared/Deck/DeckBuilder.cs` | static class |
| [DeckDeal](#deckdeal) | `Assets/Scripts/Shared/Deck/DeckDeal.cs` | plain C# class |
| [IDuplicateIndexRule, NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule) | `Assets/Scripts/Shared/Deck/IDuplicateIndexRule.cs`, `Assets/Scripts/Shared/Deck/NextFreeIndexRule.cs` | interface, plain C# class |
| [DeckPresetGenerator](#deckpresetgenerator) | `Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs` | static editor class (menu item) |

## DeckRules

`Assets/Scripts/Shared/Deck/DeckRules.cs`. Numbers of the deck and the table: `TableSlots = 40` (base slots), `KingValue = 10`, `KingCount = 4`, `BaseNonKingCards = 36`, `SlotsPerRow = 10`, `MaxExtraRows = 4` and `MaxTableSlots = 80` (index>40: one slot per added card, at most 4 extra rows).

- **Used by**: [DeckBuilder](#deckbuilder), [DeckPreset](#deckpreset), [NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule), [TableState](./table.md#tablestate) (`FaceUpKings`), [TableDeckManager](./table.md#tabledeckmanager) (`SlotPosition`), [GameLogFormatter](./stats.md#gamelogformatter), [DeckPresetGenerator](#deckpresetgenerator).

## ExcessCards

`Assets/Scripts/Shared/Deck/ExcessCards.cs`. What happens to the cards beyond the 40 base slots, the [deck-modification idea](../business/modifica-del-mazzo.md) each scene tries: `Bench` (panchina: face down in the pile, they fill the holes left by paths), `Discard` (tribuna: thrown away), `Index` (index>40: every added card gets its own index and slot). Serialized as `TableDeckManager.excessCards`.

- **Used by**: [DeckBuilder](#deckbuilder), [DeckDeal](#deckdeal) (`BenchCards`), [TableDeckManager](./table.md#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator), [GameLogFormatter](./stats.md#gamelogformatter), [TableSimulationRunner](./stats.md#tablesimulationrunner).

## DeckPreset

`Assets/Scripts/Shared/Deck/DeckPreset.cs`. A deck to try: the 36 non-king cards with `removedCards` taken out (0–36) and `duplicates` copies added; the 4 kings are always added on top. `NonKingCards` (read-only in the inspector) = 36 − removed + duplicates. **Which** cards are removed or duplicated is drawn at every game by [DeckBuilder](#deckbuilder). Create menu: `Carte/Deck Preset`; `Set` exists only in the Editor, for the generator.

- **Assets** in `Assets/Instances/DeckPresets/` (created by [DeckPresetGenerator](#deckpresetgenerator)):

  | Asset | Removed | Duplicates | Non-king cards | Deck |
  |---|---|---|---|---|
  | `Preset32.asset` | 9 | 5 | 32 | 36 (4 holes on the table) |
  | `Preset45.asset` | 5 | 14 | 45 | 49 |
  | `Preset60.asset` | 5 | 29 | 60 | 64 |
  | `Preset71.asset` (was `Preset80`, 49 duplicates) | 5 | 40 | 71 | 75 |

  At most 40 duplicates (`MaxExtraRows` rows of 10), so that index>40 can deal every preset.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager) (`deckPreset`: `Preset32` in Panchina, `Preset71` in Tribuna, `Preset45` in Index40), [TableSimulationRunner](./stats.md#tablesimulationrunner) (`presets`, all four in the `SimulationButton` prefab), [GameSimulator](./stats.md#gamesimulator).

## DeckBuilder

`Assets/Scripts/Shared/Deck/DeckBuilder.cs`. Returns the [DeckDeal](#deckdeal) of one game; the same `System.Random` seed gives the same deal. `Build(excess, baseCards, preset, random)` picks the deal of an [ExcessCards](#excesscards) mode: `BuildIndexed` with [NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule) for `Index`, `Build(baseCards, removedCount, duplicateCount, random)` otherwise.

- Checks that the base cards have exactly 4 kings (otherwise throws), and the counts (throws when out of range or with nothing left to duplicate).
- Removes `removedCount` distinct non-king cards at random; adds `duplicateCount` copies of the remaining ones, **spread out**: every card gets a copy before any gets a second one (each copy a new [CardDataInGame](./shared.md#carddataingame) instance, never a king).
- **Panchina and tribuna** (`Build`): shuffles; the first 36 non-king cards and the 4 kings are shuffled on the **first** slots; with fewer cards the **last** slots are holes (`null`; `Preset32`: slots 36–39). The non-king cards beyond the table are the **excess**, in draw order.
- **Index>40** (`BuildIndexed(baseCards, removedCount, duplicateCount, indexRule, random)`): each duplicate gets its index from the `indexRule`; the table has a slot for every index (40 + duplicates with the next-free rule, at most `MaxTableSlots`). The slots of the **removed** cards stay holes; every other card, kings included, is shuffled on the remaining slots; no excess. Throws when an added index is a base index, out of range, repeated, or leaves gaps.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator).

## DeckDeal

`Assets/Scripts/Shared/Deck/DeckDeal.cs`. Result of [DeckBuilder](#deckbuilder): `TableSlots` (card dealt face down to each slot, `null` for a hole; 40 slots, up to 80 in index>40), `Excess` (bench of the panchina, discarded in the tribuna, empty in index>40), `BenchCards(excess)` (the excess in the panchina, none otherwise), `Removed`, `Duplicates`; computed `TableCards`, `Holes`, `TableDuplicates` (table cards whose slot is already claimed by another copy of the same card), `DeckSize` (kings included).

- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameLogFormatter](./stats.md#gamelogformatter), [GameSimulator](./stats.md#gamesimulator), [SimulatedGame](./stats.md#simulatedgame-simulatedpath), [SimulationCsv](./stats.md#simulationcsv).

## IDuplicateIndexRule, NextFreeIndexRule

`Assets/Scripts/Shared/Deck/IDuplicateIndexRule.cs`: `IndexOf(original, duplicateNumber)`, the index (and so the slot) given to each card added to the deck in index>40, `duplicateNumber` counting the added cards from 0. Meant for "index modification" rules that vary the table configuration when the deck is built (rule being tried, see [Modifica del mazzo](../business/modifica-del-mazzo.md)). `Assets/Scripts/Shared/Deck/NextFreeIndexRule.cs`: the only rule today, `40 + duplicateNumber` (40, 41, …), whatever the card.

- **Used by**: [DeckBuilder](#deckbuilder) (`BuildIndexed`).

## DeckPresetGenerator

`Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs`. Editor only. Menu `Tools/Genera Preset Mazzo`: creates the four [DeckPreset](#deckpreset) assets in `Assets/Instances/DeckPresets/` (and the folder); presets that already exist are kept as they are (GUIDs unchanged). The four presets are a literal table in the class.
