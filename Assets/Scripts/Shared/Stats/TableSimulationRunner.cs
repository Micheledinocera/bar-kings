using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

// Simulation button of the table scenes: plays many games of every preset of the library with the rules of the open
// scene (bench or discard, from TableDeckManager), with each autoplay strategy chosen, each game up to the 4th king, and
// saves three CSV files: the paths of the first games, as a snapshot, one row per game (holes and shapes at the end)
// and the averages of every preset. The scene name in the file names says the mode. The game on the table is not
// touched.
public class TableSimulationRunner : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    // Optional, the Console always gets the messages.
    [SerializeField]
    private GameLogView gameLog;

    [SerializeField, Required]
    private Button button;

    [SerializeField, Required]
    private DeckPresetLibrary presetLibrary; // Assets/Instances/DeckPresetLibrary.asset

    // Games per preset written path by path in the runs file.
    [SerializeField, MinValue(0)]
    private int snapshotGames = 20;

    // Games per preset in the averages, the snapshot games included.
    [SerializeField, MinValue(1)]
    private int games = 300;

    // Seed of the simulation: 0 draws a new one at every run. The same seed gives the same files.
    [SerializeField]
    private int seed;

    // Every preset is played with each strategy, on the same deals.
    [SerializeField]
    private List<AutoplayStrategy> strategies = new()
    {
        AutoplayStrategy.Random,
        AutoplayStrategy.Targeted,
    };

    const int GamesPerFrame = 50; // games simulated before yielding a frame, so the scene stays responsive
    const string OutputFolder = "Simulations"; // in the project root (git-ignored); persistentDataPath in a build

    public void Run()
    {
        RunAsync(destroyCancellationToken).Forget();
    }

    async UniTaskVoid RunAsync(CancellationToken cancellationToken)
    {
        button.interactable = false;
        try
        {
            int simulationSeed = seed != 0 ? seed : Random.Range(1, int.MaxValue);
            System.Random random = new(simulationSeed);
            List<CardDataInGame> baseCards = tableDeckManager.DeckDatabase.CreateCardsInGame();
            IReadOnlyList<DeckPreset> presets = presetLibrary.Presets;
            Log(
                $"Simulation | {tableDeckManager.Excess} | {presets.Count} presets x {strategies.Count} strategies "
                    + $"x {games} games, to the 4th king | seed {simulationSeed}"
            );

            // Rows written result by result, so the games of a result can be dropped.
            StringBuilder runs = new();
            StringBuilder gameRows = new();
            StringBuilder averages = new();
            bool withHeader = true;
            int played = 0;
            foreach (DeckPreset preset in presets)
            {
                List<int> gameSeeds = new(games);
                for (int i = 0; i < games; i++)
                {
                    gameSeeds.Add(random.Next(1, int.MaxValue));
                }
                foreach (AutoplayStrategy strategy in strategies)
                {
                    List<SimulatedGame> presetGames = new(games);
                    foreach (int gameSeed in gameSeeds)
                    {
                        presetGames.Add(
                            GameSimulator.Play(
                                baseCards,
                                preset,
                                tableDeckManager.Excess,
                                gameSeed,
                                random,
                                strategy
                            )
                        );
                        if (++played % GamesPerFrame == 0)
                        {
                            await UniTask.Yield(cancellationToken);
                        }
                    }
                    SimulationResult[] result = { new(preset, strategy, presetGames) };
                    runs.Append(SimulationCsv.Runs(result, snapshotGames, withHeader));
                    gameRows.Append(SimulationCsv.Games(result, withHeader));
                    averages.Append(SimulationCsv.Averages(result, simulationSeed, withHeader));
                    withHeader = false;
                }
            }

            string folder = Path.Combine(
                Application.isEditor
                    ? Path.GetDirectoryName(Application.dataPath)
                    : Application.persistentDataPath,
                OutputFolder
            );
            Directory.CreateDirectory(folder);
            string prefix = Path.Combine(
                folder,
                $"{System.DateTime.Now:yyyyMMdd-HHmmss}-{gameObject.scene.name}"
            );
            File.WriteAllText($"{prefix}-runs.csv", runs.ToString());
            File.WriteAllText($"{prefix}-games.csv", gameRows.ToString());
            File.WriteAllText($"{prefix}-averages.csv", averages.ToString());
            Log($"Simulation saved: {prefix}-runs.csv, -games.csv, -averages.csv");
        }
        finally
        {
            if (button != null) // destroyed when the scene is unloaded mid-run
            {
                button.interactable = true;
            }
        }
    }

    void Log(string entry)
    {
        Debug.Log(entry);
        if (gameLog != null)
        {
            gameLog.Append(entry);
        }
    }
}
