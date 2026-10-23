using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

// CSV files of a simulation (TableSimulationRunner): the paths of the first games of each preset, as a snapshot,
// one row per game with its holes and shapes at the end, and one row of averages per preset and strategy. Each call
// writes the rows of some results, after the header when withHeader, so the runner can drop the games once written.
// Italian Excel format, so a double click opens them right: ';' as separator, ',' as decimal mark. What follows from
// the preset (deck size, holes at the start) or from the file name (the mode) is left out.
public static class SimulationCsv
{
    const string Separator = ";";

    static readonly NumberFormatInfo DecimalComma = new() { NumberDecimalSeparator = "," };

    static readonly ShapeKind[] Kinds = (ShapeKind[])Enum.GetValues(typeof(ShapeKind));

    public static string Runs(
        IEnumerable<SimulationResult> results,
        int snapshotGames,
        bool withHeader
    )
    {
        StringBuilder csv = new();
        if (withHeader)
        {
            csv.AppendLine(
                Row(
                    "preset",
                    "strategy",
                    "game",
                    "seed",
                    "table_duplicates",
                    "path",
                    "length",
                    "end",
                    "bench_fill",
                    "kings_face_up",
                    "holes",
                    "cards"
                )
            );
        }
        foreach (SimulationResult result in results)
        {
            for (int i = 0; i < snapshotGames && i < result.Games.Count; i++)
            {
                SimulatedGame game = result.Games[i];
                foreach (SimulatedPath path in game.Paths)
                {
                    string cards = string.Join(" > ", path.Cards.Select(GameLogFormatter.CardName));
                    csv.AppendLine(
                        Row(
                            result.Preset.name,
                            result.Strategy,
                            i + 1,
                            game.Seed,
                            game.Deal.TableDuplicates,
                            path.Number,
                            path.Length,
                            path.End,
                            path.FilledFromBench ? 1 : 0,
                            path.FaceUpKings,
                            path.Holes,
                            $"\"{cards}\""
                        )
                    );
                }
            }
        }
        return csv.ToString();
    }

    // One row per game: holes at the end, how many shapes of each kind they make, and every shape as "name @path",
    // the path after which it was complete.
    public static string Games(IEnumerable<SimulationResult> results, bool withHeader)
    {
        StringBuilder csv = new();
        if (withHeader)
        {
            csv.AppendLine(
                Row(
                    new object[]
                    {
                        "preset",
                        "strategy",
                        "game",
                        "seed",
                        "table_duplicates",
                        "paths",
                        "face_down_at_end",
                        "holes",
                    }
                        .Concat(Kinds.Select(ShapeColumn))
                        .Append("shapes")
                        .ToArray()
                )
            );
        }
        foreach (SimulationResult result in results)
        {
            for (int i = 0; i < result.Games.Count; i++)
            {
                SimulatedGame game = result.Games[i];
                string shapes = string.Join(
                    " | ",
                    game.Shapes.OrderBy(found => found.path)
                        .Select(found => $"{found.shape.Name} @{found.path}")
                );
                csv.AppendLine(
                    Row(
                        new object[]
                        {
                            result.Preset.name,
                            result.Strategy,
                            i + 1,
                            game.Seed,
                            game.Deal.TableDuplicates,
                            game.Paths.Count,
                            game.FaceDownAtEnd,
                            game.HoleSlots.Count(),
                        }
                            .Concat(Kinds.Select(kind => (object)ShapeCount(game, kind)))
                            .Append($"\"{shapes}\"")
                            .ToArray()
                    )
                );
            }
        }
        return csv.ToString();
    }

    // One row per preset and strategy: averages per game; for each shape kind, the shapes at the end per game.
    public static string Averages(
        IEnumerable<SimulationResult> results,
        int simulationSeed,
        bool withHeader
    )
    {
        StringBuilder csv = new();
        if (withHeader)
        {
            csv.AppendLine(
                Row(
                    new object[]
                    {
                        "preset",
                        "strategy",
                        "simulation_seed",
                        "games",
                        "table_duplicates",
                        "paths_per_game",
                        "avg_path_length",
                        "first_path_length",
                        "longest_path",
                        "face_down_at_end",
                        "holes",
                    }
                        .Concat(Kinds.Select(ShapeColumn))
                        .ToArray()
                )
            );
        }
        foreach (SimulationResult result in results)
        {
            IReadOnlyList<SimulatedGame> games = result.Games;
            List<SimulatedPath> paths = games.SelectMany(game => game.Paths).ToList();
            csv.AppendLine(
                Row(
                    new object[]
                    {
                        result.Preset.name,
                        result.Strategy,
                        simulationSeed,
                        games.Count,
                        Number(games.Average(game => game.Deal.TableDuplicates)),
                        Number(games.Average(game => game.Paths.Count)),
                        Number(paths.Average(path => path.Length)),
                        Number(games.Average(game => game.Paths[0].Length)),
                        Number(games.Average(game => game.Paths.Max(path => path.Length))),
                        Number(games.Average(game => game.FaceDownAtEnd)),
                        Number(games.Average(game => game.HoleSlots.Count())),
                    }
                        .Concat(
                            Kinds.Select(kind =>
                                (object)Number(games.Average(game => ShapeCount(game, kind)))
                            )
                        )
                        .ToArray()
                )
            );
        }
        return csv.ToString();
    }

    static int ShapeCount(SimulatedGame game, ShapeKind kind) =>
        game.Shapes.Count(found => found.shape.Kind == kind);

    static string ShapeColumn(ShapeKind kind) => kind.ToString().ToLowerInvariant();

    static string Row(params object[] cells) => string.Join(Separator, cells);

    static string Number(double value) => value.ToString("0.###", DecimalComma);
}
