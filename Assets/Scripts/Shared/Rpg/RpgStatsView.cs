using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

// RPG part of the TribunaRpg scene: at the end of each path resolves its cards in order (CardEffects); a king
// starts a fight (Fight) with the stats reached so far, its summary waits in a popup (FightPopup), and the cards
// after it are applied once the popup is closed, only if the king is beaten. A defeat ends the encounter. Logs
// everything in the Console and shows the stats on screen. Listens to TableDeckManager only; a new game (Reset)
// starts from the stats of the Inspector.
public class RpgStatsView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private TMP_Text statsText;

    [SerializeField, Required]
    private TMP_Text eventsText;

    [SerializeField, Required]
    private FightPopup fightPopup;

    [SerializeField]
    private PlayerConfig player = new();

    [SerializeField]
    private KingConfig king = new();

    private PlayerStats stats;

    void OnEnable()
    {
        tableDeckManager.GameStarted += ResetStats;
        tableDeckManager.PathPlayed += OnPathPlayed;
        ResetStats();
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= ResetStats;
        tableDeckManager.PathPlayed -= OnPathPlayed;
    }

    void ResetStats()
    {
        stats = player.CreateStats();
        eventsText.text = "nessun path ancora";
        RefreshStats();
    }

    void OnPathPlayed(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        // The bench move only fills a hole face down: it is not a card of the path.
        List<CardDataInGame> cards = moves
            .Where(move => move.End != PathEnd.FilledFromBench)
            .Select(move => move.Card)
            .ToList();
        ResolvePath(pathNumber, cards, destroyCancellationToken).Forget();
    }

    async UniTaskVoid ResolvePath(
        int pathNumber,
        List<CardDataInGame> cards,
        CancellationToken cancellationToken
    )
    {
        List<string> log = new() { $"RPG - path n. {pathNumber}" };
        List<string> fights = new();
        foreach (CardDataInGame card in cards)
        {
            if (!CardEffects.IsKing(card))
            {
                log.Add($"  {CardEffects.Apply(card, stats, player)}");
                continue;
            }

            string kingName = PathReceipt.CardLabel(card);
            KingFighter fighter = king.CreateFighter(kingName);
            log.Add(
                $"  {kingName}: fight! (vita {fighter.Health}, armatura {fighter.Armor}, "
                    + $"danno {fighter.Damage})"
            );
            FightResult result = Fight.Run(stats, fighter);
            log.AddRange(result.Turns.Select(turn => $"    {turn}"));
            fights.Add(FightLine(kingName, result));
            log.Add($"  {fights[^1]}");
            Debug.Log(string.Join("\n", log));
            log.Clear();
            if (result.Outcome == FightOutcome.KingWon)
            {
                // The cards after the king are not applied: the encounter is over.
                tableDeckManager.EndEncounter();
            }

            RefreshStats();
            eventsText.text = $"path n. {pathNumber}: fight con {kingName}";
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
            if (result.Outcome == FightOutcome.KingWon)
            {
                break;
            }
        }
        log.Add($"  {StatsLine()}");
        Debug.Log(string.Join("\n", log));

        eventsText.text =
            fights.Count == 0
                ? $"path n. {pathNumber}: {cards.Count} carte, nessun re"
                : $"path n. {pathNumber}: " + string.Join("\n", fights);
        RefreshStats();
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
        statsText.text =
            $"Danno {stats.Damage}\nVita {stats.Health}/{stats.MaxHealth}\n"
            + $"Armatura {stats.Armor}\nDenari {stats.Money} Lire";
    }

    string StatsLine() =>
        $"danno {stats.Damage} | vita {stats.Health}/{stats.MaxHealth} | "
        + $"armatura {stats.Armor} | denari {stats.Money} Lire";
}
