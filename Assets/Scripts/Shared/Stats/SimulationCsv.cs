using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

// CSV files of a simulation (TableSimulationRunner): the paths of the first games of each preset, as a snapshot,
// and one row of averages per preset over all its games. Comma separated, dot as decimal mark.
public static class SimulationCsv
{
    public static string Runs(
        IEnumerable<(DeckPreset preset, IReadOnlyList<SimulatedGame> games)> results,
        string mode,
        int snapshotGames
    )
    {
        StringBuilder csv = new();
        csv.AppendLine(
            "preset,mode,game,seed,deck_size,table_duplicates,start_holes,path,start_slot,length,end,bench_fill,kings_face_up,cards"
        );
        foreach ((DeckPreset preset, IReadOnlyList<SimulatedGame> games) in results)
        {
            for (int i = 0; i < snapshotGames && i < games.Count; i++)
            {
                SimulatedGame game = games[i];
                foreach (SimulatedPath path in game.Paths)
                {
                    string cards = string.Join(" > ", path.Cards.Select(GameLogFormatter.CardName));
                    csv.AppendLine(
                        $"{preset.name},{mode},{i + 1},{game.Seed},{game.Deal.DeckSize},{game.Deal.TableDuplicates},"
                            + $"{game.Deal.Holes},{path.Number},{path.StartSlot},{path.Length},{path.End},"
                            + $"{(path.FilledFromBench ? 1 : 0)},{path.FaceUpKings},\"{cards}\""
                    );
                }
            }
        }
        return csv.ToString();
    }

    public static string Averages(
        IEnumerable<(DeckPreset preset, IReadOnlyList<SimulatedGame> games)> results,
        string mode,
        int simulationSeed
    )
    {
        StringBuilder csv = new();
        csv.AppendLine(
            "preset,mode,simulation_seed,games,deck_size,table_duplicates,start_holes,paths_per_game,avg_path_length,"
                + "first_path_length,longest_path,closed_paths_pct,all_kings_games_pct,all_kings_at_path"
        );
        foreach ((DeckPreset preset, IReadOnlyList<SimulatedGame> games) in results)
        {
            List<SimulatedPath> paths = games.SelectMany(game => game.Paths).ToList();
            List<SimulatedGame> allKings = games.Where(game => game.AllKingsPath > 0).ToList();
            csv.AppendLine(
                string.Join(
                    ",",
                    preset.name,
                    mode,
                    simulationSeed,
                    games.Count,
                    Number(games.Average(game => game.Deal.DeckSize)),
                    Number(games.Average(game => game.Deal.TableDuplicates)),
                    Number(games.Average(game => game.Deal.Holes)),
                    Number(games.Average(game => game.Paths.Count)),
                    Number(paths.Average(path => path.Length)),
                    Number(games.Average(game => game.Paths[0].Length)),
                    Number(games.Average(game => game.Paths.Max(path => path.Length))),
                    Number(100.0 * paths.Count(path => path.End == PathEnd.Closed) / paths.Count),
                    Number(100.0 * allKings.Count / games.Count),
                    allKings.Count > 0 ? Number(allKings.Average(game => game.AllKingsPath)) : ""
                )
            );
        }
        return csv.ToString();
    }

    static string Number(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
