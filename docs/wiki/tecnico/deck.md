---
tipo: info
aggiornato: 2026-10-23
verificato: 2026-10-23
consolidato: 2026-10-23
fonti:
  - raw/compiled/preset-buchi/2026-10-23-implementazione.md
  - raw/compiled/preset-buchi/2026-10-23-obiettivo.md
  - raw/compiled/preset-builder/2026-10-19-obiettivo.md
  - raw/compiled/preset-builder/2026-10-19-strumenti.md
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/index40/2026-10-16-obiettivo.md
  - raw/compiled/stats/2026-10-15-obiettivo.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/stats/2026-10-16-sessione.md
---

# Deck scripts

Scripts in `Assets/Scripts/Shared/Deck/` and the preset editor menus in `Assets/Scripts/Shared/Editor/`: the deck of one game of the table scenes (Panchina, Tribuna, Index40), built from a **preset** of the [preset library](#deckpresetlibrary) with a **seed**, dealt as the scene's [ExcessCards](#excesscards) mode says. Plain C# except the `DeckPreset` and `DeckPresetLibrary` assets, so the [autoplay](./stats.md#gamesimulator) builds decks exactly as the scenes do. The presets are the decks being tried for [Modifica del mazzo](../business/modifica-del-mazzo.md) (decision in analysis). Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [DeckRules](#deckrules) | `Assets/Scripts/Shared/Deck/DeckRules.cs` | static class (constants) |
| [ExcessCards](#excesscards) | `Assets/Scripts/Shared/Deck/ExcessCards.cs` | enum |
| [CardPick](#cardpick) | `Assets/Scripts/Shared/Deck/CardPick.cs` | enum |
| [ChosenCopies](#chosencopies) | `Assets/Scripts/Shared/Deck/ChosenCopies.cs` | serializable class |
| [DeckPreset](#deckpreset) | `Assets/Scripts/Shared/Deck/DeckPreset.cs` | ScriptableObject |
| [DeckPresetLibrary](#deckpresetlibrary) | `Assets/Scripts/Shared/Deck/DeckPresetLibrary.cs` | ScriptableObject |
| [DeckBuilder](#deckbuilder) | `Assets/Scripts/Shared/Deck/DeckBuilder.cs` | static class |
| [DeckDeal](#deckdeal) | `Assets/Scripts/Shared/Deck/DeckDeal.cs` | plain C# class |
| [IDuplicateIndexRule, NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule) | `Assets/Scripts/Shared/Deck/IDuplicateIndexRule.cs`, `Assets/Scripts/Shared/Deck/NextFreeIndexRule.cs` | interface, plain C# class |
| [DeckPresetGenerator](#deckpresetgenerator) | `Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs` | static editor class (menu item) |
| [ShapePresetGenerator](#shapepresetgenerator) | `Assets/Scripts/Shared/Editor/ShapePresetGenerator.cs` | static editor class (menu item) |
| [DeckPresetBuilderWindow](#deckpresetbuilderwindow) | `Assets/Scripts/Shared/Editor/DeckPresetBuilderWindow.cs` | Odin editor window (menu item) |

## DeckRules

`Assets/Scripts/Shared/Deck/DeckRules.cs`. Numbers of the deck and the table: `TableSlots = 40` (base slots), `KingValue = 10`, `KingCount = 4`, `BaseNonKingCards = 36`, `SlotsPerRow = 10`, `BaseRows = 4` (the base grid: one row per suit in `Seme` order, one column per value, so slot = row × 10 + value − 1), `MaxExtraRows = 4` and `MaxTableSlots = 80` (index>40: one slot per added card, at most 4 extra rows), `MaxDuplicates = 40` (duplicates of a preset: one per extra slot, so that index>40 can deal every preset).

- **Used by**: [DeckBuilder](#deckbuilder), [DeckPreset](#deckpreset), [ChosenCopies](#chosencopies), [NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule), [TableState](./table.md#tablestate) (`FaceUpKings`), [TableDeckManager](./table.md#tabledeckmanager) (`SlotPosition`), [GameLogFormatter](./stats.md#gamelogformatter), [GameSimulator](./stats.md#gamesimulator), [TableShapes](./stats.md#tableshape-tableshapes-shapekind), [DeckPresetGenerator](#deckpresetgenerator), [ShapePresetGenerator](#shapepresetgenerator), [DeckPresetBuilderWindow](#deckpresetbuilderwindow).

## ExcessCards

`Assets/Scripts/Shared/Deck/ExcessCards.cs`. What happens to the cards beyond the 40 base slots, the [deck-modification idea](../business/modifica-del-mazzo.md) each scene tries: `Bench` (panchina: face down in the pile, they fill the holes left by paths), `Discard` (tribuna: thrown away), `Index` (index>40: every added card gets its own index and slot). Serialized as `TableDeckManager.excessCards`.

- **Used by**: [DeckBuilder](#deckbuilder), [DeckDeal](#deckdeal) (`BenchCards`), [TableDeckManager](./table.md#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator), [GameLogFormatter](./stats.md#gamelogformatter), [TableSimulationRunner](./stats.md#tablesimulationrunner).

## CardPick

`Assets/Scripts/Shared/Deck/CardPick.cs`. Which non-king cards a preset removes and duplicates **beyond the chosen ones**: `Random` (drawn at every game; the default) or `First` (the first ones in table order, `CardDataInGame.index`: the same cards at every game, only the deal changes). Rule being tried, see [Modifica del mazzo](../business/modifica-del-mazzo.md). The [Preset Builder](#deckpresetbuilderwindow) no longer sets it: only presets of the menus or edited in the inspector use it.

- **Used by**: [DeckPreset](#deckpreset) (`pick`), [DeckBuilder](#deckbuilder).

## ChosenCopies

`Assets/Scripts/Shared/Deck/ChosenCopies.cs`. A non-king card ([CardData](./shared.md#carddata) asset reference) that a preset always duplicates, with `extraCopies` (1–`MaxDuplicates`) added to the original: n copies in all = n − 1 extra copies.

- **Used by**: [DeckPreset](#deckpreset) (`chosenDuplicates`), [DeckBuilder](#deckbuilder), [ShapePresetGenerator](#shapepresetgenerator), [DeckPresetBuilderWindow](#deckpresetbuilderwindow).

## DeckPreset

`Assets/Scripts/Shared/Deck/DeckPreset.cs`. A deck to try: the 36 non-king cards with some removed and some duplicates added; the 4 kings are always added on top. Two ways to say which cards, which add up:

- **chosen cards**, the same at every game: `chosenRemoved` (list of [CardData](./shared.md#carddata)) and `chosenDuplicates` (list of [ChosenCopies](#chosencopies));
- **counts**: `removedCards` more removed (0–36) and `duplicates` more copies (0–`MaxDuplicates`), picked as `pick` ([CardPick](#cardpick), toggle buttons in the inspector) says. Copie-n and the "piena" shape presets use the counts next to the chosen cards.

`NonKingCards` (read-only in the inspector) = 36 − removed − chosen removed + duplicates + chosen copies. `NameOf(preset)` (static): the preset name, `"Normale"` for no preset (the normal deck, see [DeckBuilder](#deckbuilder)), used by the log and the options. Create menu `Carte/Deck Preset`: the asset is not added to the [library](#deckpresetlibrary), use the [Preset Builder](#deckpresetbuilderwindow) instead. `Set(removed, duplicates, pick = Random)` and `SetChosen(removed, duplicated)` exist only in the Editor, for the menus.

- **Assets** in `Assets/Instances/DeckPresets/`: today the custom presets of the [Preset Builder](#deckpresetbuilderwindow) (see [scenes](./scenes.md#card-assets)). The standard presets of [DeckPresetGenerator](#deckpresetgenerator) (`Preset32`, `Preset45`, `Preset60`, `Preset71`: 9/5/5/5 removed, 5/14/29/40 duplicates, `Random`) and the shape presets of [ShapePresetGenerator](#shapepresetgenerator) were removed on 2026-10-23; their menus create them again.
- **Used by**: [DeckPresetLibrary](#deckpresetlibrary), [TableDeckManager](./table.md#tabledeckmanager) (`deckPreset`, empty in the three scenes: first preset of the library), [OptionsPanel](./table.md#optionspanel) and [TableSimulationRunner](./stats.md#tablesimulationrunner) (through the library), [DeckBuilder](#deckbuilder), [GameSimulator](./stats.md#gamesimulator), [SimulationResult](./stats.md#simulationresult).

## DeckPresetLibrary

`Assets/Scripts/Shared/Deck/DeckPresetLibrary.cs`. The presets to try: one list shared by the options panel, the simulation and the scenes (first preset of a game), so a new preset reaches all of them. Asset `Assets/Instances/DeckPresetLibrary.asset`. Create menu: `Carte/Deck Preset Library`.

- `Presets` skips the entries of presets deleted from the project: they stay in the list as empty references, and one made [OptionsPanel](./table.md#optionspanel) throw on the first press of the options button.
- `Add(preset)` (Editor only) adds a preset once and drops the empty entries.
- **Created and filled by**: [DeckPresetGenerator](#deckpresetgenerator), [ShapePresetGenerator](#shapepresetgenerator), [DeckPresetBuilderWindow](#deckpresetbuilderwindow).
- **Used by**: [OptionsPanel](./table.md#optionspanel) and [TableSimulationRunner](./stats.md#tablesimulationrunner) (`presetLibrary`, set in `Assets/Prefab/Options.prefab` and `Assets/Prefab/SimulationButton.prefab`), [TableDeckManager](./table.md#tabledeckmanager) (`presetLibrary`, set in the three table scenes).

## DeckBuilder

`Assets/Scripts/Shared/Deck/DeckBuilder.cs`. Returns the [DeckDeal](#deckdeal) of one game; the same `System.Random` seed gives the same deal. `Build(excess, baseCards, preset, random)` picks the deal of an [ExcessCards](#excesscards) mode: `BuildIndexed` with [NextFreeIndexRule](#iduplicateindexrule-nextfreeindexrule) for `Index`, `Build(baseCards, removedCount, duplicateCount, pick, random, chosenRemoved, chosenDuplicates)` otherwise. A `null` preset deals the **normal deck**: nothing removed or added.

- Checks that the base cards have exactly 4 kings (otherwise throws), and the counts (throws when out of range or with nothing left to duplicate). Throws when a chosen card is not a non-king card of the base deck, is chosen twice, or is both removed and duplicated.
- **Removed**: the chosen ones, then `removedCount` more among the cards not chosen. **Duplicated**: the chosen copies first, then `duplicateCount` copies of the kept cards, **spread out** (every card gets a copy before any gets a second one; each copy a new [CardDataInGame](./shared.md#carddataingame) instance, never a king). Which cards among the not chosen, by [CardPick](#cardpick): `Random` after a shuffle; `First` in table order (the first ones removed, the first kept ones duplicated; the chosen duplicates are kept last). The table is shuffled in both cases. Without chosen cards a seed gives the same deal as before they existed.
- **Panchina and tribuna** (`Build`): shuffles; the first 36 non-king cards and the 4 kings are shuffled on the **first** slots; with fewer cards the **last** slots are holes (`null`; e.g. 32 non-king cards: slots 36–39). The non-king cards beyond the table are the **excess**, in draw order.
- **Index>40** (`BuildIndexed(baseCards, removedCount, duplicateCount, pick, indexRule, random, chosenRemoved, chosenDuplicates)`): each duplicate gets its index from the `indexRule`, numbered across chosen and spread copies; the table has a slot for every index (40 + duplicates with the next-free rule, at most `MaxTableSlots`). The slots of the **removed** cards stay holes; every other card, kings included, is shuffled on the remaining slots; no excess. Throws when an added index is a base index, out of range, repeated, or leaves gaps.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameSimulator](./stats.md#gamesimulator).

## DeckDeal

`Assets/Scripts/Shared/Deck/DeckDeal.cs`. Result of [DeckBuilder](#deckbuilder): `TableSlots` (card dealt face down to each slot, `null` for a hole; 40 slots, up to 80 in index>40), `Excess` (bench of the panchina, discarded in the tribuna, empty in index>40), `BenchCards(excess)` (the excess in the panchina, none otherwise), `Removed`, `Duplicates`; computed `TableCards`, `Holes`, `TableDuplicates` (table cards whose slot is already claimed by another copy of the same card), `DeckSize` (kings included).

- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [GameLogFormatter](./stats.md#gamelogformatter), [GameSimulator](./stats.md#gamesimulator) (`Removed`: the targeted autoplay), [SimulatedGame](./stats.md#simulatedgame-simulatedpath), [SimulationCsv](./stats.md#simulationcsv).

## IDuplicateIndexRule, NextFreeIndexRule

`Assets/Scripts/Shared/Deck/IDuplicateIndexRule.cs`: `IndexOf(original, duplicateNumber)`, the index (and so the slot) given to each card added to the deck in index>40, `duplicateNumber` counting the added cards from 0. Meant for "index modification" rules that vary the table configuration when the deck is built (rule being tried, see [Modifica del mazzo](../business/modifica-del-mazzo.md)). `Assets/Scripts/Shared/Deck/NextFreeIndexRule.cs`: the only rule today, `40 + duplicateNumber` (40, 41, …), whatever the card.

- **Used by**: [DeckBuilder](#deckbuilder) (`BuildIndexed`).

## DeckPresetGenerator

`Assets/Scripts/Shared/Editor/DeckPresetGenerator.cs`. Editor only. Menu `Tools/Genera Preset Mazzo`: creates the four standard [DeckPreset](#deckpreset) assets (`Preset32`, `Preset45`, `Preset60`, `Preset71`) in `Assets/Instances/DeckPresets/` (and the folder) and the [DeckPresetLibrary](#deckpresetlibrary) asset, and adds the four to the library. Presets that already exist are kept as they are (GUIDs unchanged); the library keeps its presets (custom ones included) and loses the entries of deleted ones, so rerunning the menu also cleans it. The four presets are a literal table in the class. `EnsurePresetsFolder`, `LoadOrCreateLibrary` and `PresetsFolder` are shared with [ShapePresetGenerator](#shapepresetgenerator) and the [Preset Builder](#deckpresetbuilderwindow).

## ShapePresetGenerator

`Assets/Scripts/Shared/Editor/ShapePresetGenerator.cs`. Editor only. Menu `Tools/Genera Preset Forme`: creates the presets that measure the shapes of holes in the tribuna ([Modifica del mazzo](../business/modifica-del-mazzo.md), decision in analysis) and adds them to the [library](#deckpresetlibrary); presets that already exist are kept as they are. The cards are found in the [CardDatabase](./shared.md#carddatabase) by grid slot ([TableShapes](./stats.md#tableshape-tableshapes-shapekind)`.Slot`).

- `Copie2`, `Copie4`, `Copie8`: n − 1 cards removed at random and the ace of Denari in n copies (40 cards).
- `Colonna` (the 4 aces), `Diagonale` (ace of Denari, 2 of Coppe, 3 of Spade, 4 of Bastoni), `Quadrato2`, `Quadrato3`, `Quadrato4` (from the ace of Denari): each **Secca** (only its cards removed, under 40 cards) and **Piena** (as many random duplicates as cards removed, 40 cards).
- `SemeSecca` (Denari 1–9), `PariSecca` (2, 4, 6, 8 of every suit: 16 cards, fante = 8).

## DeckPresetBuilderWindow

`Assets/Scripts/Shared/Editor/DeckPresetBuilderWindow.cs`. Editor only, an Odin `OdinEditorWindow` ([Odin docs](https://odininspector.com/documentation)). Menu `Tools/Preset Builder`: creates a custom [DeckPreset](#deckpreset) from a **card grid** like the table (rows = suits, columns = values 1–9, kings left out; IMGUI drawn in an `[OnInspectorGUI]` method): in each cell −1 removes the card (red), n adds n copies (green), 0 keeps it; "Clear grid" resets it. The preset gets only chosen cards (`SetChosen`), no counts; it is added to the [library](#deckpresetlibrary), so the options panel and the simulation use it next to the others.

- Shows the non-king cards, the deck size and the asset path. Checks the limits (at most `MaxDuplicates` copies added, a valid file name): on an error the button is disabled and an error box says why. The grid lives in the window (serialized `cells`), not in an asset.
- Empty name: `Custom<deck size>`, with a number when taken (`AssetDatabase.GenerateUniqueAssetPath`). A typed name that already exists is an error, never an overwrite. After creating, the name is cleared and an info box shows the asset created.
- A preset deleted by hand leaves an empty entry in the library: harmless, removed at the next create or by the generator menus.
