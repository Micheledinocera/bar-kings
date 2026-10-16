using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

// Simulation button of the table scenes: plays many games of the selected presets with the rules of the open scene
// (bench or discard, from TableDeckManager) and saves two CSV files: the paths of the first games, as a snapshot,
// and the averages of every preset. The game on the table is not touched.
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
    private List<DeckPreset> presets = new();

    // Games per preset written path by path in the runs file.
    [SerializeField, MinValue(0)]
    private int snapshotGames = 20;

    // Games per preset in the averages, the snapshot games included.
    [SerializeField, MinValue(1)]
    private int games = 1000;

    // Seed of the simulation: 0 draws a new one at every run. The same seed gives the same files.
    [SerializeField]
    private int seed;

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
            string mode = tableDeckManager.Excess.ToString();
            List<CardDataInGame> baseCards = tableDeckManager.DeckDatabase.CreateCardsInGame();
            Log(
                $"Simulation | {mode} | {presets.Count} presets x {games} games | seed {simulationSeed}"
            );

            List<(DeckPreset, IReadOnlyList<SimulatedGame>)> results = new();
            foreach (DeckPreset preset in presets)
            {
                List<SimulatedGame> played = new(games);
                for (int i = 0; i < games; i++)
                {
                    int gameSeed = random.Next(1, int.MaxValue);
                    played.Add(
                        GameSimulator.Play(
                            baseCards,
                            preset,
                            tableDeckManager.Excess,
                            gameSeed,
                            random
                        )
                    );
                    if ((i + 1) % GamesPerFrame == 0)
                    {
                        await UniTask.Yield(cancellationToken);
                    }
                }
                results.Add((preset, played));
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
            File.WriteAllText(
                $"{prefix}-runs.csv",
                SimulationCsv.Runs(results, mode, snapshotGames)
            );
            File.WriteAllText(
                $"{prefix}-averages.csv",
                SimulationCsv.Averages(results, mode, simulationSeed)
            );
            Log($"Simulation saved: {prefix}-runs.csv, -averages.csv");
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
