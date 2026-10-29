---
tipo: info
aggiornato: 2026-10-29
verificato: 2026-10-29
consolidato: 2026-10-19
fonti:
  - raw/compiled/preset-buchi/2026-10-23-implementazione.md
  - raw/compiled/preset-buchi/2026-10-23-risultati.md
  - raw/compiled/preset-buchi/2026-10-23-risposte.md
  - raw/compiled/preset-builder/2026-10-19-obiettivo.md
  - raw/compiled/demo-1/2026-10-16-obiettivo.md
  - raw/compiled/index40/2026-10-16-csv-fixes.md
  - raw/compiled/index40/2026-10-16-simulazione-tre-modi.md
  - raw/compiled/stats/2026-10-15-obiettivo.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/stats/2026-10-16-sessione.md
  - raw/compiled/stats/2026-10-16-simulazione-preset.md
---

# Stats scripts: game log and simulation

Scripts in `Assets/Scripts/Shared/Stats/`: the **game log** of the table scenes (Console and an on-screen panel) and the **simulation** (autoplay of many games, saved as CSV) used to compare the [deck presets](./deck.md#deckpreset): how many holes are left at the end of a game and which **shapes** they make. Logic in plain C#; only `GameLogView` and `TableSimulationRunner` are MonoBehaviours. Results: `docs/raw/compiled/preset-buchi/2026-10-23-risultati.md` (tribuna, holes and shapes, end at the 4th king); older ones, with the end at no face-down card left: `docs/raw/compiled/index40/2026-10-16-simulazione-tre-modi.md`. Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [GameLogFormatter](#gamelogformatter) | `Assets/Scripts/Shared/Stats/GameLogFormatter.cs` | static class |
| [GameLogView](#gamelogview) | `Assets/Scripts/Shared/Stats/GameLogView.cs` | MonoBehaviour |
| [AutoplayStrategy](#autoplaystrategy) | `Assets/Scripts/Shared/Stats/AutoplayStrategy.cs` | enum |
| [GameSimulator](#gamesimulator) | `Assets/Scripts/Shared/Stats/GameSimulator.cs` | static class |
| [SimulatedGame, SimulatedPath](#simulatedgame-simulatedpath) | `Assets/Scripts/Shared/Stats/SimulatedGame.cs`, `Assets/Scripts/Shared/Stats/SimulatedPath.cs` | plain C# classes |
| [TableShape, TableShapes, ShapeKind](#tableshape-tableshapes-shapekind) | `Assets/Scripts/Shared/Stats/TableShape.cs`, `Assets/Scripts/Shared/Stats/TableShapes.cs`, `Assets/Scripts/Shared/Stats/ShapeKind.cs` | plain C# class, static class, enum |
| [SimulationResult](#simulationresult) | `Assets/Scripts/Shared/Stats/SimulationResult.cs` | plain C# class |
| [SimulationCsv](#simulationcsv) | `Assets/Scripts/Shared/Stats/SimulationCsv.cs` | static class |
| [TableSimulationRunner](#tablesimulationrunner) | `Assets/Scripts/Shared/Stats/TableSimulationRunner.cs` | MonoBehaviour |

## GameLogFormatter

`Assets/Scripts/Shared/Stats/GameLogFormatter.cs`. Text of the game log:

- `FormatDeal`: preset (`DeckPreset.NameOf`: "Normale" for the normal deck), seed, deck size, table cards, holes, then by [ExcessCards](./deck.md#excesscards) mode the bench count, the discarded count or the number of slots (index>40), removed cards and duplicates;
- `FormatPath`: path number, starting slot, length (cards moved; a bench card filling the starting slot is listed apart), `PathEnd`, kings face up after the path (`kings 2/4`), and the cards with their target slot;
- `FormatAllKings`: `All 4 kings face up at path N`, once per game;
- `CardName`: `<valore> <seme>`, also used in the CSV.
- **ASCII only** (`|`, `->`, `-`): TextMeshPro adds characters missing from the font atlas to the dynamic fallback font asset (`Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`), which would then change at every Play.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [SimulationCsv](#simulationcsv).

## GameLogView

`Assets/Scripts/Shared/Stats/GameLogView.cs`. Scrollable on-screen log: `Append(entry)` adds at the bottom and scrolls to it, `Clear()`, `ToggleVisible()` (shows or hides the panel; entries keep coming while hidden). Serialized: `text` (TMP), `scrollRect`.

- **Prefab**: `Assets/Prefab/GameLog.prefab` (panel on the left of the Canvas, see [scenes](./scenes.md#prefabs)).
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager) (`gameLog`, cleared at every setup), [TableSimulationRunner](#tablesimulationrunner) (`gameLog`), the `LogToggleButton` of the table scenes (`ToggleVisible`, persistent `OnClick`).

## AutoplayStrategy

`Assets/Scripts/Shared/Stats/AutoplayStrategy.cs`. How the autoplay chooses the face-down slot each path starts from, a trial strategy for the statistics, not a game rule: `Random` (any face-down slot) or `Targeted` (first the face-down slots of the removed cards, at random among them: a player who knows the deck and aims at the shape of the missing cards; then any).

- **Used by**: [GameSimulator](#gamesimulator), [SimulationResult](#simulationresult), [TableSimulationRunner](#tablesimulationrunner) (`strategies`), [SimulationCsv](#simulationcsv).

## GameSimulator

`Assets/Scripts/Shared/Stats/GameSimulator.cs`. `Play(baseCards, preset, excess, gameSeed, choices, strategy = Random)`: deals with `DeckBuilder.Build(excess, …)` ([DeckBuilder](./deck.md#deckbuilder)) and `new Random(gameSeed)` (the same deal as the scene with that mode and `seed`; bench from `DeckDeal.BenchCards`), then plays [TableState](./table.md#tablestate) paths from the slots the [AutoplayStrategy](#autoplaystrategy) chooses with `choices`. Returns a [SimulatedGame](#simulatedgame-simulatedpath).

- **The game ends right after the path that turns the 4th king face up** (rule being tried for the tribuna, [Modifica del mazzo](../business/modifica-del-mazzo.md); the scenes go on until no face-down card is left). It always ends: every path turns at least one card face up, and the kings are always on the table.
- After each path it records, for each slot, the path after which it became a hole for the last time (`holeSince`: 0 for a hole since the deal, −1 for a slot that is not a hole). Only holes at the end count. With the path rule of today a path makes **at most one hole** (its starting slot), so a shape of k slots needs at least k paths.
- In index>40 every path closes: every card has its own slot, so it never meets a face-up duplicate, and no card has the index of a hole.
- **Used by**: [TableSimulationRunner](#tablesimulationrunner).

## SimulatedGame, SimulatedPath

`Assets/Scripts/Shared/Stats/SimulatedGame.cs`: `Seed`, `Deal` ([DeckDeal](./deck.md#deckdeal)), `Paths`, `AllKingsPath` (path after which all 4 kings are face up: the last path), `HoleSince` (see [GameSimulator](#gamesimulator)), `FaceDownAtEnd`, `HoleSlots` (the holes at the end), `Shapes` (the shapes of holes at the end with the path after which each was complete, from [TableShapes](#tableshape-tableshapes-shapekind)`.Complete`). `Assets/Scripts/Shared/Stats/SimulatedPath.cs`: `Number`, `Cards` (moved by the path), `Length`, `End`, `FilledFromBench`, `FaceUpKings`, `Holes` (holes on the table after the path).

- **Used by**: [GameSimulator](#gamesimulator), [SimulationResult](#simulationresult), [SimulationCsv](#simulationcsv), [TableSimulationRunner](#tablesimulationrunner).

## TableShape, TableShapes, ShapeKind

Shapes of holes on the base table grid, measured for the tribuna bonus being studied ([Modifica del mazzo](../business/modifica-del-mazzo.md), decision in analysis). They are **drawings on the grid** (rule given by the user): they depend on the order of the suit rows (Denari, Coppe, Spade, Bastoni).

- `Assets/Scripts/Shared/Stats/ShapeKind.cs`: `Column` (the 4 suits of one value), `Diagonal` (one cell per row on consecutive values, going down or up), `Square2`, `Square3`, `Square4`.
- `Assets/Scripts/Shared/Stats/TableShape.cs`: one shape at one place: `Kind`, `Name` (e.g. `column 3`, `diagonal 1 down`, `square2 Coppe 4`, the top-left cell), `Slots`.
- `Assets/Scripts/Shared/Stats/TableShapes.cs`: `All`, every shape on the 4×10 grid, built once (74: 10 columns, 14 diagonals, 27 squares 2×2, 16 3×3, 7 4×4; every instance counts, a 3×3 also contains four 2×2); `Slot(row, column)`, `ValueOf(slot)` (1 = ace … 10 = king); `Complete(holeSince)`: the shapes whose slots are all holes, each with the highest `holeSince` of its slots.
- **Used by**: [SimulatedGame](#simulatedgame-simulatedpath), [SimulationCsv](#simulationcsv), [ShapePresetGenerator](./deck.md#shapepresetgenerator) (`Slot`, `ValueOf`), [TombolaCard](./hud.md#tombolacard) (the tombola card of the Bar scene: `Complete`, `Kind`).

## SimulationResult

`Assets/Scripts/Shared/Stats/SimulationResult.cs`: the games of one [preset](./deck.md#deckpreset) played with one [AutoplayStrategy](#autoplaystrategy): `Preset`, `Strategy`, `Games`.

- **Used by**: [TableSimulationRunner](#tablesimulationrunner), [SimulationCsv](#simulationcsv).

## SimulationCsv

`Assets/Scripts/Shared/Stats/SimulationCsv.cs`. The three CSV texts of a simulation, in the **Italian Excel** format, so that a double click opens them right: `;` as separator, `,` as decimal mark. Points read as thousands separators by Italian Excel made the averages look like sums. Each method writes the rows of some [SimulationResult](#simulationresult)s, after the header when `withHeader`, so the runner can drop the games of a result once written. Kept short on purpose (request of the user): holes at the end and shapes, nothing that follows from the preset or from the file name (the mode, in the scene name).

- `Runs`: one row per path of the first `snapshotGames` games of each preset and strategy: `preset; strategy; game; seed; table_duplicates; path; length; end; bench_fill; kings_face_up; holes; cards`;
- `Games`: one row per game: `preset; strategy; game; seed; table_duplicates; paths; face_down_at_end; holes; column; diagonal; square2; square3; square4; shapes`: the number of shapes of each kind at the end, and `shapes` lists every shape as `name @path`, the path after which it was complete;
- `Averages`: one row per preset and strategy over all its games: `preset; strategy; simulation_seed; games; table_duplicates; paths_per_game` (= path of the 4th king)`; avg_path_length` (total cards moved / total paths)`; first_path_length; longest_path; face_down_at_end; holes`, then the shapes of each kind per game.
- `table_duplicates`: table cards that are copies of a card already on the table, i.e. how many of the preset's duplicates reached the table (the others went to the bench or were discarded; always 0 in index>40). `bench_fill`: 1 when a bench card went face down into the starting slot after the path.
- **Used by**: [TableSimulationRunner](#tablesimulationrunner).

## TableSimulationRunner

`Assets/Scripts/Shared/Stats/TableSimulationRunner.cs`. The **Simula** button of the table scenes. `Run()` (persistent `OnClick` of its own `Button`) starts an `async UniTaskVoid`: for each preset of `presetLibrary` ([DeckPresetLibrary](./deck.md#deckpresetlibrary)), draws `games` game seeds and plays them with each strategy of `strategies` (the same deals for every strategy) through [GameSimulator](#gamesimulator), with the rules of the open scene (`TableDeckManager.Excess`: panchina, tribuna or index>40) and its `DeckDatabase`; then writes `<yyyyMMdd-HHmmss>-<scene>-runs.csv`, `-games.csv` and `-averages.csv` with [SimulationCsv](#simulationcsv) and logs where. The CSV rows are written after each preset and strategy, and its games dropped. The button is not interactable while it runs; the table being played is not touched. An empty library simulates nothing.

- **Serialized fields**: `tableDeckManager`, `gameLog` (optional; scene overrides of the prefab instance), `button`, `presetLibrary` (`Assets/Instances/DeckPresetLibrary.asset`, set on the prefab), `snapshotGames` (20), `games` (300 on the prefab, snapshot games included), `seed` (0 = a new one at every run, written in the averages), `strategies` ([AutoplayStrategy](#autoplaystrategy) list, `Random` and `Targeted`).
- **Output**: `Simulations/` in the project root (git-ignored, `.gitignore`); `Application.persistentDataPath/Simulations` in a build.
- Yields a frame every `GamesPerFrame` (50) games. With the Editor in the background the frames stop (`Application.runInBackground` off), and so does the simulation: run it with the Editor focused.
- **Prefab**: `Assets/Prefab/SimulationButton.prefab`, in the table scenes ([scenes](./scenes.md#panchina-tribuna-and-index40)).
