using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

// TEMP Bar-Michele (feature events-impl): copy of RpgStatsView for the Bar-Michele test scene, where the events
// are tried without touching TribunaRpg. To merge back or delete when one idea is chosen. Differences from
// RpgStatsView: the stats are one line, for the header of the Bar HUD; each card is resolved as soon as it is
// turned (TableDeckManager.MoveResolver), not at the end of the path.
//
// RPG part of the scene: resolves the cards of a path in order while they are turned (CardEffects); a king
// starts a fight (Fight) with the stats reached so far, its summary opens in a popup (FightPopup) at once, and
// the path goes on when the popup is closed; the next cards are applied only if the king is beaten. A defeat ends the encounter. Logs
// everything in the Console and shows the stats on screen. Listens to TableDeckManager only; a new game (Reset)
// starts from the stats of the Inspector. Owns the player's santini and amari (PlayerInventory, editable in the
// Inspector during Play mode): the santini change the card effects, an amaro is drunk from the InventoryView
// between two paths (not while a path is resolved, not after the encounter is over).
public class BarRpgView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private TMP_Text statsText;

    [SerializeField, Required]
    private TMP_Text eventsText;

    [SerializeField, Required]
    private FightPopup fightPopup;

    [SerializeField, Required]
    private InventoryView inventoryView;

    [SerializeField]
    private PlayerConfig player = new();

    [SerializeField]
    private KingConfig king = new();

    [SerializeField]
    private PlayerInventory inventory = new();

    private PlayerStats stats;
    private bool resolving; // a path is being resolved: no amaro can be drunk
    private bool encounterOver;

    // The path being played: its number, the cards resolved so far, whether a defeat stopped it, its fights and
    // the Console lines not written yet.
    private int currentPath;
    private int pathCards;
    private bool pathStopped;
    private readonly List<string> pathFights = new();
    private readonly List<string> pathLog = new();

    // What the InventoryView shows, to redraw it when the inventory changes (also from the Inspector).
    private readonly List<object> shownItems = new();
    private bool shownCanDrink;

    private bool CanDrink => !resolving && !encounterOver;

    void OnEnable()
    {
        tableDeckManager.GameStarted += ResetStats;
        tableDeckManager.PathStarted += OnPathStarted;
        tableDeckManager.MoveResolver = ResolveMove;
        tableDeckManager.PathPlayed += OnPathPlayed;
        tableDeckManager.EncounterEnded += OnEncounterEnded;
        inventoryView.DrinkRequested += DrinkAmaro;
        ResetStats();
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= ResetStats;
        tableDeckManager.PathStarted -= OnPathStarted;
        tableDeckManager.MoveResolver = null;
        tableDeckManager.PathPlayed -= OnPathPlayed;
        tableDeckManager.EncounterEnded -= OnEncounterEnded;
        inventoryView.DrinkRequested -= DrinkAmaro;
    }

    // Picks up the changes made to the inventory in the Inspector during Play mode.
    void Update() => RefreshInventory(force: false);

    void ResetStats()
    {
        stats = player.CreateStats();
        encounterOver = false;
        eventsText.text = "nessun path ancora";
        RefreshStats();
        RefreshInventory(force: true);
    }

    void OnEncounterEnded(int paths)
    {
        encounterOver = true;
        RefreshInventory(force: true);
    }

    void DrinkAmaro(int index)
    {
        if (
            !CanDrink
            || index < 0
            || index >= inventory.amari.Count
            || inventory.amari[index] == null
        )
        {
            return;
        }
        Amaro amaro = inventory.amari[index];
        inventory.amari.RemoveAt(index);
        string line = amaro.Drink(stats);
        Debug.Log($"RPG - amaro: {line}\n  {StatsLine()}");
        eventsText.text = line;
        RefreshStats();
        RefreshInventory(force: true);
    }

    void RefreshInventory(bool force)
    {
        List<object> items = inventory.santini.Cast<object>().Concat(inventory.amari).ToList();
        if (!force && shownCanDrink == CanDrink && shownItems.SequenceEqual(items))
        {
            return;
        }
        shownItems.Clear();
        shownItems.AddRange(items);
        shownCanDrink = CanDrink;
        inventoryView.Show(inventory.santini, inventory.amari, CanDrink);
    }

    // A path starts: its cards are resolved one by one while they are turned (ResolveMove).
    void OnPathStarted(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        currentPath = pathNumber;
        pathCards = 0;
        pathStopped = false;
        pathFights.Clear();
        pathLog.Clear();
        pathLog.Add($"RPG - path n. {pathNumber}");
        resolving = true;
        eventsText.text = $"path n. {pathNumber}: in corso";
        RefreshInventory(force: true);
    }

    // Awaited by TableDeckManager after each turned card, before the next one: a card is applied at once (the
    // stats change while the path goes on), a king opens its fight popup mid-path and the path goes on when it is
    // closed. After a defeat the remaining cards still move on the table but are not applied.
    async UniTask ResolveMove(int index, PathMove move, CancellationToken cancellationToken)
    {
        // The bench move only fills a hole face down: it is not a card of the path.
        if (pathStopped || move.End == PathEnd.FilledFromBench)
        {
            return;
        }
        pathCards++;
        CardDataInGame card = move.Card;
        if (!CardEffects.IsKing(card))
        {
            pathLog.Add($"  {CardEffects.Apply(card, stats, player, inventory.santini)}");
            RefreshStats();
            return;
        }

        string kingName = PathReceipt.CardLabel(card);
        KingFighter fighter = king.CreateFighter(kingName);
        pathLog.Add(
            $"  {kingName}: fight! (vita {fighter.Health}, armatura {fighter.Armor}, "
                + $"danno {fighter.Damage})"
        );
        FightResult result = Fight.Run(stats, fighter);
        pathLog.AddRange(result.Turns.Select(turn => $"    {turn}"));
        pathFights.Add(FightLine(kingName, result));
        pathLog.Add($"  {pathFights[^1]}");
        Debug.Log(string.Join("\n", pathLog));
        pathLog.Clear();
        if (result.Outcome == FightOutcome.KingWon)
        {
            // The cards after the king are not applied: the encounter is over.
            pathStopped = true;
            tableDeckManager.EndEncounter();
        }

        RefreshStats();
        eventsText.text = $"path n. {currentPath}: fight con {kingName}";
        tableDeckManager.TablePaused = true;
        try
        {
            await fightPopup.Show(
                FightTitle(kingName, result.Outcome),
                FightSummary(fighter, result),
                cancellationToken
            );
        }
        finally
        {
            tableDeckManager.TablePaused = false;
        }
    }

    void OnPathPlayed(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        pathLog.Add($"  {StatsLine()}");
        Debug.Log(string.Join("\n", pathLog));
        pathLog.Clear();
        eventsText.text =
            pathFights.Count == 0
                ? $"path n. {pathNumber}: {pathCards} carte, nessun re"
                : $"path n. {pathNumber}: " + string.Join("\n", pathFights);
        resolving = false;
        RefreshStats();
        RefreshInventory(force: true);
    }

    static string FightLine(string kingName, FightResult result) =>
        result.Outcome switch
        {
            FightOutcome.PlayerWon => $"{kingName} battuto in {result.Turns.Count} turni",
            FightOutcome.KingWon =>
                $"SCONFITTO da {kingName} in {result.Turns.Count} turni - Reset per rigiocare",
            _ => $"{kingName}: pareggio, nessuno passa l'armatura",
        };

    static string FightTitle(string kingName, FightOutcome outcome) =>
        outcome switch
        {
            FightOutcome.PlayerWon => $"{kingName} battuto!",
            FightOutcome.KingWon => $"Sconfitto da {kingName}",
            _ => $"{kingName}: pareggio",
        };

    string FightSummary(KingFighter fighter, FightResult result)
    {
        string summary =
            $"Turni: {result.Turns.Count}\n"
            + $"Danni inflitti al re: {result.DamageToKing} "
            + $"(vita re {fighter.Health}/{fighter.MaxHealth})\n"
            + $"Danni subiti: {result.DamageToPlayer} (vita {stats.Health}/{stats.MaxHealth})\n"
            + $"Il tuo colpo: {stats.Damage} - {fighter.Armor} armatura = "
            + $"{Fight.HitDamage(stats, fighter)}\n"
            + $"Colpo del re: {fighter.Damage} - {stats.Armor} armatura = "
            + $"{Fight.HitDamage(fighter, stats)}";
        return result.Outcome switch
        {
            FightOutcome.KingWon => summary + "\n\nIncontro finito: Reset per rigiocare.",
            FightOutcome.Stalemate => summary + "\n\nNessuno passa l'armatura dell'altro.",
            _ => summary,
        };
    }

    void RefreshStats()
    {
        // One line, as the StatistichePlaceholder of the Bar header it replaces.
        statsText.text =
            $"L. {stats.Money}    vita {stats.Health}/{stats.MaxHealth}    "
            + $"armatura {stats.Armor}    danno {stats.Damage}";
    }

    string StatsLine() =>
        $"danno {stats.Damage} | vita {stats.Health}/{stats.MaxHealth} | "
        + $"armatura {stats.Armor} | denari {stats.Money} Lire";
}
