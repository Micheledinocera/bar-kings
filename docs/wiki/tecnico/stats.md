---
tipo: info
aggiornato: 2026-10-16
verificato: 2026-10-16
fonti:
  - raw/compiled/index40/2026-10-16-csv-fixes.md
  - raw/compiled/index40/2026-10-16-simulazione-tre-modi.md
  - raw/compiled/stats/2026-10-15-obiettivo.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/stats/2026-10-16-sessione.md
  - raw/compiled/stats/2026-10-16-simulazione-preset.md
---

# Stats scripts: game log and simulation

Scripts in `Assets/Scripts/Shared/Stats/`: the **game log** of the table scenes (Console and an on-screen panel) and the **simulation** (autoplay of many games, saved as CSV) used to compare the [deck presets](./deck.md#deckpreset). Logic in plain C#; only `GameLogView` and `TableSimulationRunner` are MonoBehaviours. Results: `docs/raw/compiled/index40/2026-10-16-simulazione-tre-modi.md` (the three modes; replaces, for preset 71 and the holes of preset 32, the first table in `docs/raw/compiled/stats/2026-10-16-simulazione-preset.md`). Issues: [known-issues](./known-issues.md).

| Class | Path | Kind |
|---|---|---|
| [GameLogFormatter](#gamelogformatter) | `Assets/Scripts/Shared/Stats/GameLogFormatter.cs` | static class |
| [GameLogView](#gamelogview) | `Assets/Scripts/Shared/Stats/GameLogView.cs` | MonoBehaviour |
| [GameSimulator](#gamesimulator) | `Assets/Scripts/Shared/Stats/GameSimulator.cs` | static class |
| [SimulatedGame, SimulatedPath](#simulatedgame-simulatedpath) | `Assets/Scripts/Shared/Stats/SimulatedGame.cs`, `Assets/Scripts/Shared/Stats/SimulatedPath.cs` | plain C# classes |
| [SimulationCsv](#simulationcsv) | `Assets/Scripts/Shared/Stats/SimulationCsv.cs` | static class |
| [TableSimulationRunner](#tablesimulationrunner) | `Assets/Scripts/Shared/Stats/TableSimulationRunner.cs` | MonoBehaviour |

## GameLogFormatter

`Assets/Scripts/Shared/Stats/GameLogFormatter.cs`. Text of the game log:

- `FormatDeal`: preset, seed, deck size, table cards, holes, then by [ExcessCards](./deck.md#excesscards) mode the bench count, the discarded count or the number of slots (index>40), removed cards and duplicates;
- `FormatPath`: path number, starting slot, length (cards moved; a bench card filling the starting slot is listed apart), `PathEnd`, kings face up after the path (`kings 2/4`), and the cards with their target slot;
- `FormatAllKings`: `All 4 kings face up at path N`, once per game;
- `CardName`: `<valore> <seme>`, also used in the CSV.
- **ASCII only** (`|`, `->`, `-`): TextMeshPro adds characters missing from the font atlas to the dynamic fallback font asset (`Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`), which would then change at every Play.
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager), [SimulationCsv](#simulationcsv).

## GameLogView

`Assets/Scripts/Shared/Stats/GameLogView.cs`. Scrollable on-screen log: `Append(entry)` adds at the bottom and scrolls to it, `Clear()`. Serialized: `text` (TMP), `scrollRect`.

- **Prefab**: `Assets/Prefab/GameLog.prefab` (panel on the left of the Canvas, see [scenes](./scenes.md#prefabs)).
- **Used by**: [TableDeckManager](./table.md#tabledeckmanager) (`gameLog`, cleared at every setup), [TableSimulationRunner](#tablesimulationrunner) (`gameLog`).

## GameSimulator

`Assets/Scripts/Shared/Stats/GameSimulator.cs`. `Play(baseCards, preset, excess, gameSeed, choices)`: deals with `DeckBuilder.Build(excess, …)` ([DeckBuilder](./deck.md#deckbuilder)) and `new Random(gameSeed)` (the same deal as the scene with that mode and `seed`; bench from `DeckDeal.BenchCards`), then plays [TableState](./table.md#tablestate) paths until no face-down card is left. Returns a [SimulatedGame](#simulatedgame-simulatedpath).

- **Autoplay strategy being tried**: every path starts from a face-down slot chosen at random with `choices`. Not a game rule.
- The game always ends: every path turns at least one card face up and the bench is finite.
- In index>40 every path closes: every card has its own slot, so it never meets a face-up duplicate, and no card has the index of a hole.
- **Used by**: [TableSimulationRunner](#tablesimulationrunner).

## SimulatedGame, SimulatedPath

`Assets/Scripts/Shared/Stats/SimulatedGame.cs`: `Seed`, `Deal` ([DeckDeal](./deck.md#deckdeal)), `Paths`, `AllKingsPath` (path after which all 4 kings are face up, 0 if never). `Assets/Scripts/Shared/Stats/SimulatedPath.cs`: `Number`, `Cards` (moved by the path), `Length`, `End`, `FilledFromBench`, `FaceUpKings`.

- **Used by**: [GameSimulator](#gamesimulator), [SimulationCsv](#simulationcsv), [TableSimulationRunner](#tablesimulationrunner).

## SimulationCsv

`Assets/Scripts/Shared/Stats/SimulationCsv.cs`. The two CSV texts of a simulation, in the **Italian Excel** format, so that a double click opens them right: `;` as separator, `,` as decimal mark. Points read as thousands separators by Italian Excel made the averages look like sums. What follows from the preset (deck size, holes at the start, starting slot) is left out.

- `Runs`: one row per path of the first `snapshotGames` games of each preset: `preset; mode; game; seed; table_duplicates; path; length; end; bench_fill; kings_face_up; cards`;
- `Averages`: one row per preset over all its games: `preset; mode; simulation_seed; games; table_duplicates; paths_per_game; avg_path_length` (total cards moved / total paths)`; first_path_length; longest_path; closed_paths_pct; all_kings_games_pct; all_kings_at_path` (over the games where it happens).
- `table_duplicates`: table cards that are copies of a card already on the table, i.e. how many of the preset's duplicates reached the table (the others went to the bench or were discarded; always 0 in index>40). `bench_fill`: 1 when a bench card went face down into the starting slot after the path.
- **Used by**: [TableSimulationRunner](#tablesimulationrunner).

## TableSimulationRunner

`Assets/Scripts/Shared/Stats/TableSimulationRunner.cs`. The **Simula** button of the table scenes. `Run()` (persistent `OnClick` of its own `Button`) starts an `async UniTaskVoid`: for each preset of `presets`, `games` games with [GameSimulator](#gamesimulator), the rules of the open scene (`TableDeckManager.Excess`: panchina, tribuna or index>40) and its `DeckDatabase`; then writes `<yyyyMMdd-HHmmss>-<scene>-runs.csv` and `-averages.csv` with [SimulationCsv](#simulationcsv) and logs where. The button is not interactable while it runs; the table being played is not touched.

- **Serialized fields**: `tableDeckManager`, `gameLog` (optional; scene overrides of the prefab instance), `button`, `presets`, `snapshotGames` (20), `games` (1000, snapshot games included), `seed` (0 = a new one at every run, written in the averages).
- **Output**: `Simulations/` in the project root (git-ignored, `.gitignore`); `Application.persistentDataPath/Simulations` in a build.
- Yields a frame every `GamesPerFrame` (50) games. With the Editor in the background the frames stop (`Application.runInBackground` off), and so does the simulation: run it with the Editor focused.
- **Prefab**: `Assets/Prefab/SimulationButton.prefab`, in the three table scenes ([scenes](./scenes.md#panchina-scenatribuna-and-scenaindex40)).
