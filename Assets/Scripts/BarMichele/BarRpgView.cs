using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// TEMP Bar-Michele (feature events-impl): copy of RpgStatsView for the Bar-Michele test scene, where the events
// are tried without touching TribunaRpg. To merge back or delete when one idea is chosen. Differences from
// RpgStatsView: the stats are in a panel of the left column (Il giocatore); each card is resolved as soon as it is
// turned (TableDeckManager.MoveResolver), not at the end of the path; the 8 and 9 are events instead of stats
// (BarEvents): a horse (9) gives escapes, a fante (8) asks shop or skip, and every king asks escape or fight
// (ChoicePopup).
//
// RPG part of the scene: resolves the cards of a path in order while they are turned (CardEffects); a king
// starts a fight (Fight) with the stats reached so far, its summary opens in a popup (FightPopup) at once, and
// the path goes on when the popup is closed; the next cards are applied only if the king is beaten. A defeat
// ends the encounter. Logs everything in the Console and shows the stats on screen. Listens to
// TableDeckManager only; a new game (Reset) starts from the stats of the Inspector. Owns the player's santini and amari (PlayerInventory, editable in the
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

    [SerializeField, Required, FormerlySerializedAs("escapeChoicePopup")]
    private ChoicePopup choicePopup;

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
    private int escapes; // fights with a king that can still be skipped for free in this encounter (BarEvents)

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
        escapes = 0;
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
        if (BarEvents.IsHorse(card))
        {
            int gained = BarEvents.EscapesFrom(card, inventory.santini);
            escapes += gained;
            pathLog.Add($"  {PathReceipt.CardLabel(card)}: +{gained} fuga (fughe {escapes})");
            RefreshStats();
            return;
        }
        if (BarEvents.IsFante(card))
        {
            await ResolveFante(card, cancellationToken);
            return;
        }
        if (!CardEffects.IsKing(card))
        {
            pathLog.Add($"  {CardEffects.Apply(card, stats, player, inventory.santini)}");
            RefreshStats();
            return;
        }

        string kingName = PathReceipt.CardLabel(card);
        KingFighter fighter = king.CreateFighter(kingName);
        if (await AskEscape(kingName, fighter, cancellationToken))
        {
            Escape(kingName);
            return;
        }

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
        await ShowPopup(
            FightTitle(kingName, result.Outcome),
            FightSummary(fighter, result),
            cancellationToken
        );
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

    // Asks whether to escape from the king just turned (true) or fight it, with what the fight would be and what
    // the escape costs.
    UniTask<bool> AskEscape(
        string kingName,
        KingFighter fighter,
        CancellationToken cancellationToken
    )
    {
        string cost =
            escapes > 0
                ? $"Fughe: {escapes}. Scappare usa una fuga."
                : $"Nessuna fuga: scappare costa il {BarEvents.EscapePenaltyLirePercent}% delle Lire "
                    + $"({BarEvents.EscapePenalty(stats.Money)} Lire).";
        string body =
            $"{kingName}: vita {fighter.Health}, armatura {fighter.Armor}, danno {fighter.Damage}\n"
            + $"Il tuo colpo: {Fight.HitDamage(stats, fighter)} - "
            + $"colpo del re: {Fight.HitDamage(fighter, stats)}\n"
            + $"La tua vita: {stats.Health}/{stats.MaxHealth}\n\n"
            + $"{cost}\nScappando non combatti e il re e' perso.";
        string escapeLabel =
            escapes > 0
                ? "Scappa (1 fuga)"
                : $"Scappa (-{BarEvents.EscapePenalty(stats.Money)} L.)";
        return Ask($"{kingName}!", body, escapeLabel, "Combatti", cancellationToken);
    }

    // Escapes from a king: an escape if one is left, otherwise the Lire malus. No fight, the king is lost.
    void Escape(string kingName)
    {
        string paid;
        if (escapes > 0)
        {
            escapes--;
            paid = $"1 fuga (fughe {escapes})";
        }
        else
        {
            int lost = BarEvents.EscapePenalty(stats.Money);
            stats.AddMoney(-lost);
            paid = $"-{lost} Lire";
        }
        pathFights.Add($"scappato da {kingName} ({paid})");
        pathLog.Add($"  {kingName}: scappato, nessun combattimento, {paid}");
        eventsText.text = $"path n. {currentPath}: scappato da {kingName}";
        RefreshStats();
    }

    // A fante opens the shop (nothing to buy yet) or is skipped for a share of the Lire of a game.
    async UniTask ResolveFante(CardDataInGame card, CancellationToken cancellationToken)
    {
        string label = PathReceipt.CardLabel(card);
        int skipLire = BarEvents.SkipShopLire;
        bool enter = await Ask(
            $"{label}: lo shop",
            $"Il {label} ti apre lo shop.\n\nSe tiri dritto prendi {skipLire} Lire.",
            "Entra nello shop",
            $"Salta (+{skipLire} L.)",
            cancellationToken
        );
        if (enter)
        {
            pathLog.Add($"  {label}: shop (vuoto per ora)");
            eventsText.text = $"path n. {currentPath}: shop del {label}";
            await ShowPopup(
                "Lo shop",
                "Lo shop e' in arrivo: qui si compreranno santini e amari.",
                cancellationToken
            );
            return;
        }
        stats.AddMoney(skipLire);
        pathLog.Add($"  {label}: shop saltato, +{skipLire} Lire");
        eventsText.text = $"path n. {currentPath}: shop saltato, +{skipLire} Lire";
        RefreshStats();
    }

    // The choice popup, with the table paused meanwhile.
    async UniTask<bool> Ask(
        string title,
        string body,
        string firstLabel,
        string secondLabel,
        CancellationToken cancellationToken
    )
    {
        tableDeckManager.TablePaused = true;
        try
        {
            return await choicePopup.Ask(title, body, firstLabel, secondLabel, cancellationToken);
        }
        finally
        {
            tableDeckManager.TablePaused = false;
        }
    }

    // The message popup (FightPopup), with the table paused meanwhile.
    async UniTask ShowPopup(string title, string body, CancellationToken cancellationToken)
    {
        tableDeckManager.TablePaused = true;
        try
        {
            await fightPopup.Show(title, body, cancellationToken);
        }
        finally
        {
            tableDeckManager.TablePaused = false;
        }
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
        // Two columns, for the "Il giocatore" panel of the left column: the stats used by the fights first.
        statsText.text =
            $"Vita {stats.Health}/{stats.MaxHealth}<pos=55%>Danno {stats.Damage}\n"
            + $"Armatura {stats.Armor}<pos=55%>Fughe {escapes}\n"
            + $"Lire {stats.Money}";
    }

    string StatsLine() =>
        $"danno {stats.Damage} | vita {stats.Health}/{stats.MaxHealth} | "
        + $"armatura {stats.Armor} | denari {stats.Money} Lire";
}
