---
tipo: info
aggiornato: 2026-10-19
verificato: 2026-10-19
fonti:
  - raw/compiled/preset-builder/2026-10-19-obiettivo.md
  - raw/compiled/preset-builder/2026-10-19-strumenti.md
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/index40/2026-10-16-obiettivo.md
  - raw/compiled/stats/2026-10-15-obiettivo.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/stats/2026-10-16-sessione.md
---

# Deck scripts

Scripts in `Assets/Scripts/Shared/Deck/` and the preset editor menus in `Assets/Scripts/Shared/Editor/`: the deck of one game of the table scenes (Panchina, Tribuna, Index40), built from a **preset** of the [preset library](#deckpresetlibrary) with a **seed**, dealt as the scene's [ExcessCards](#excesscards) mode says. Plain C# except the `DeckPreset` and `DeckPresetLibrary` assets, so the [autoplay](./stats.md#gamesimulator) builds decks exactly as the scenes do. The presets are the deck sizes being tried for [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [DeckRules](#deckrules) | `Assets/Scripts/Shared/Deck/DeckRules.cs` | static class (constants) |
| [ExcessCards](#excesscards) | `Assets/Scripts/Shared/Deck/ExcessCards.cs` | enum |
| [CardPick](#cardpick) | `Assets/Scripts/Shared/Deck/CardPick.cs` | enum |
| [DeckPreset](#deckpreset) | `Assets/Scripts/Shared/Deck/DeckPreset.cs` | ScriptableObject |
| [DeckPresetLibrary](#deckpresetlibrary) | `Assets/Scripts/Shared/Deck/DeckPresetLibrary.cs` | ScriptableObject |
| [DeckBuilder](#deckbuilder) | `Assets/Scripts/Shared/Deck/DeckBuilder.cs` | static class |
| [DeckDeal](#deckdeal) | `Assets/Scripts/Shared/Deck/DeckDeal.cs` | plain C# class |
| [IDuplicateIndexRule, NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule) | `Assets/Scripts/Shared/Deck/IDuplicateIndexRule.cs`, `Assets/Scripts/Shared/Deck/NextFreeIndexRule.cs` | interface, plain C# class |
| [DeckPresetGenerator](#deckpresetgenerator) | `Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs` | static editor class (menu item) |
| [DeckPresetBuilderWindow](#deckpresetbuilderwindow) | `Assets/Scripts/Shared/Editor/DeckPresetBuilderWindow.cs` | Odin editor window (menu item) |

## DeckRules

`Assets/Scripts/Shared/Deck/DeckRules.cs`. Numbers of the deck and the table: `TableSlots = 40` (base slots), `KingValue = 10`, `KingCount = 4`, `BaseNonKingCards = 36`, `SlotsPerRow = 10`, `MaxExtraRows = 4` and `MaxTableSlots = 80` (index>40: one slot per added card, at most 4 extra rows), `MaxDuplicates = 40` (duplicates of a preset: one per extra slot, so that index>40 can deal every preset).

- **Used by**: [DeckBuilder](#deckbuilder), [DeckPreset](#deckpreset), [NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule), [TableState](./table.md#tablestate) (`FaceUpKings`), [TableDeckManager](./table.md#tabledeckmanager) (`SlotPosition`), [GameLogFormatter](./stats.md#gamelogformatter), [DeckPresetGenerator](#deckpresetgenerator), [DeckPresetBuilderWindow](#deckpresetbuilderwindow).

## ExcessCards

`Assets/Scripts/Shared/Deck/ExcessCards.cs`. What happens to the cards beyond the 40 base slots, the [deck-modification idea](../business/modifica-del-mazzo.md) each scene tries: `Bench` (panchina: face down in the pile, they fill the holes left by paths), `Discard` (tribuna: thrown away), `Index` (index>40: every added card gets its own index and slot). Serialized as `TableDeckManager.excessCards`.

- **Used by**: [DeckBuilder](#deckbuilder), [DeckDeal](#deckdeal) (`BenchCards`), [TableDeckManager](./table.md#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator), [GameLogFormatter](./stats.md#gamelogformatter), [TableSimulationRunner](./stats.md#tablesimulationrunner).

## CardPick

`Assets/Scripts/Shared/Deck/CardPick.cs`. Which non-king cards a preset removes and duplicates: `Random` (drawn at every game; the default, and the value of the four standard presets) or `First` (the first ones in table order, `CardDataInGame.index`: the same cards at every game, only the deal changes). Rule being tried, see [Modifica del mazzo](../business/modifica-del-mazzo.md).

- **Used by**: [DeckPreset](#deckpreset) (`pick`), [DeckBuilder](#deckbuilder), [DeckPresetBuilderWindow](#deckpresetbuilderwindow).

## DeckPreset

`Assets/Scripts/Shared/Deck/DeckPreset.cs`. A deck to try: the 36 non-king cards with `removedCards` taken out (0–36) and `duplicates` copies added (0–`MaxDuplicates`); the 4 kings are always added on top. `NonKingCards` (read-only in the inspector) = 36 − removed + duplicates. **Which** cards are removed or duplicated: `pick` ([CardPick](#cardpick), toggle buttons in the inspector), applied by [DeckBuilder](#deckbuilder). Create menu `Carte/Deck Preset`: the asset is not added to the [library](#deckpresetlibrary), use the [Preset Builder](#deckpresetbuilderwindow) instead. `Set(removed, duplicates, pick = Random)` exists only in the Editor, for the menus.

- **Assets** in `Assets/Instances/DeckPresets/`, standard ones created by [DeckPresetGenerator](#deckpresetgenerator), all with `pick = Random`:

  | Asset | Removed | Duplicates | Non-king cards | Deck |
  |---|---|---|---|---|
  | `Preset32.asset` | 9 | 5 | 32 | 36 (4 holes on the table) |
  | `Preset45.asset` | 5 | 14 | 45 | 49 |
  | `Preset60.asset` | 5 | 29 | 60 | 64 |
  | `Preset71.asset` (was `Preset80`, 49 duplicates) | 5 | 40 | 71 | 75 |

  Custom presets of the [Preset Builder](#deckpresetbuilderwindow), same folder: `Prime18.asset` (18 removed, 18 duplicates, `First`: 36 non-king cards, 40 cards).
- **Used by**: [DeckPresetLibrary](#deckpresetlibrary), [TableDeckManager](./table.md#tabledeckmanager) (`deckPreset`: `Preset32` in Panchina, `Preset71` in Tribuna, `Preset45` in Index40), [OptionsPanel](./table.md#optionspanel) and [TableSimulationRunner](./stats.md#tablesimulationrunner) (through the library), [GameSimulator](./stats.md#gamesimulator).

## DeckPresetLibrary

`Assets/Scripts/Shared/Deck/DeckPresetLibrary.cs`. The presets to try: one list shared by the options panel and the simulation, so a new preset reaches both. Asset `Assets/Instances/DeckPresetLibrary.asset` (today the four standard presets and `Prime18`). Create menu: `Carte/Deck Preset Library`.

- `Presets` skips the entries of presets deleted from the project: they stay in the list as empty references, and one made [OptionsPanel](./table.md#optionspanel) throw on the first press of the options button.
- `Add(preset)` (Editor only) adds a preset once and drops the empty entries.
- **Created and filled by**: [DeckPresetGenerator](#deckpresetgenerator), [DeckPresetBuilderWindow](#deckpresetbuilderwindow).
- **Used by**: [OptionsPanel](./table.md#optionspanel) and [TableSimulationRunner](./stats.md#tablesimulationrunner) (`presetLibrary`, set in `Assets/Prefab/Options.prefab` and `Assets/Prefab/SimulationButton.prefab`).

## DeckBuilder

`Assets/Scripts/Shared/Deck/DeckBuilder.cs`. Returns the [DeckDeal](#deckdeal) of one game; the same `System.Random` seed gives the same deal. `Build(excess, baseCards, preset, random)` picks the deal of an [ExcessCards](#excesscards) mode: `BuildIndexed` with [NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule) for `Index`, `Build(baseCards, removedCount, duplicateCount, pick, random)` otherwise.

- Checks that the base cards have exactly 4 kings (otherwise throws), and the counts (throws when out of range or with nothing left to duplicate).
- Removes `removedCount` distinct non-king cards and adds `duplicateCount` copies of the remaining ones, **spread out**: every card gets a copy before any gets a second one (each copy a new [CardDataInGame](./shared.md#carddataingame) instance, never a king). Which cards, by [CardPick](#cardpick): `Random` after a shuffle of the non-king cards; `First` in table order (the first ones removed, the first kept ones duplicated). The table is shuffled in both cases; with `Random` a seed gives the same deal as before `CardPick` existed.
- **Panchina and tribuna** (`Build`): shuffles; the first 36 non-king cards and the 4 kings are shuffled on the **first** slots; with fewer cards the **last** slots are holes (`null`; `Preset32`: slots 36–39). The non-king cards beyond the table are the **excess**, in draw order.
- **Index>40** (`BuildIndexed(baseCards, removedCount, duplicateCount, pick, indexRule, random)`): each duplicate gets its index from the `indexRule`; the table has a slot for every index (40 + duplicates with the next-free rule, at most `MaxTableSlots`). The slots of the **removed** cards stay holes; every other card, kings included, is shuffled on the remaining slots; no excess. Throws when an added index is a base index, out of range, repeated, or leaves gaps.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator).

## DeckDeal

`Assets/Scripts/Shared/Deck/DeckDeal.cs`. Result of [DeckBuilder](#deckbuilder): `TableSlots` (card dealt face down to each slot, `null` for a hole; 40 slots, up to 80 in index>40), `Excess` (bench of the panchina, discarded in the tribuna, empty in index>40), `BenchCards(excess)` (the excess in the panchina, none otherwise), `Removed`, `Duplicates`; computed `TableCards`, `Holes`, `TableDuplicates` (table cards whose slot is already claimed by another copy of the same card), `DeckSize` (kings included).

- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameLogFormatter](./stats.md#gamelogformatter), [GameSimulator](./stats.md#gamesimulator), [SimulatedGame](./stats.md#simulatedgame-simulatedpath), [SimulationCsv](./stats.md#simulationcsv).

## IDuplicateIndexRule, NextFreeIndexRule

`Assets/Scripts/Shared/Deck/IDuplicateIndexRule.cs`: `IndexOf(original, duplicateNumber)`, the index (and so the slot) given to each card added to the deck in index>40, `duplicateNumber` counting the added cards from 0. Meant for "index modification" rules that vary the table configuration when the deck is built (rule being tried, see [Modifica del mazzo](../business/modifica-del-mazzo.md)). `Assets/Scripts/Shared/Deck/NextFreeIndexRule.cs`: the only rule today, `40 + duplicateNumber` (40, 41, …), whatever the card.

- **Used by**: [DeckBuilder](#deckbuilder) (`BuildIndexed`).

## DeckPresetGenerator

`Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs`. Editor only. Menu `Tools/Genera Preset Mazzo`: creates the four [DeckPreset](#deckpreset) assets in `Assets/Instances/DeckPresets/` (and the folder) and the [DeckPresetLibrary](#deckpresetlibrary) asset, and adds the four to the library. Presets that already exist are kept as they are (GUIDs unchanged); the library keeps its presets (custom ones included) and loses the entries of deleted ones, so rerunning the menu also cleans it. The four presets are a literal table in the class. `EnsurePresetsFolder` and `LoadOrCreateLibrary` are shared with the [Preset Builder](#deckpresetbuilderwindow).

## DeckPresetBuilderWindow

`Assets/Scripts/Shared/Editor/DeckPresetBuilderWindow.cs`. Editor only, an Odin `OdinEditorWindow` ([Odin docs](https://odininspector.com/documentation)). Menu `Tools/Preset Builder`: creates a custom [DeckPreset](#deckpreset) from removed cards, duplicates, [CardPick](#cardpick) and an optional name, and adds it to the [library](#deckpresetlibrary), so the options panel and the simulation use it next to the others.

- Shows the non-king cards, the deck size and the asset path. Checks the limits (removed 0–36, duplicates 0–`MaxDuplicates`, a card left to duplicate, a valid file name): on an error the button is disabled and an error box says why.
- Empty name: `Custom<non-king cards>` (`…First` with `First`), with a number when taken (`AssetDatabase.GenerateUniqueAssetPath`). A typed name that already exists is an error, never an overwrite. After creating, the name is cleared and an info box shows the asset created.
- A preset deleted by hand leaves an empty entry in the library: harmless, removed at the next create or by `Tools/Genera Preset Mazzo`.
