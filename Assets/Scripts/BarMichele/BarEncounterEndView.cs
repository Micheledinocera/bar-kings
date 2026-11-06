using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

// TEMP Bar-Michele (feature events-impl): copy of EncounterEndView for the Bar-Michele test scene, where the
// encounter can end in more ways. To merge back or delete when one idea is chosen.
//
// End of the encounter: a panel fades in over the table, titled by why it ended, first match wins:
// - the player was defeated by a king (BarRpgView.DefeatText): the path stopped there;
// - every card of the table is turned (BarEvents.AllCardsTurned), also checked after each path, so it is told
//   even when the encounter would go on;
// - the 4th king is face up.
// Then the paths played and the shapes of holes made. Hidden again at the next game.
public class BarEncounterEndView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private BarRpgView rpgView;

    [SerializeField, Required]
    private CanvasGroup group;

    [SerializeField, Required]
    private TMP_Text title;

    [SerializeField, Required]
    private TMP_Text body;

    [SerializeField]
    private float fadeDuration = 0.4f;

    [SerializeField]
    private Ease fadeEase = Ease.OutQuad;

    bool shown;

    void OnEnable()
    {
        tableDeckManager.GameStarted += Hide;
        tableDeckManager.PathPlayed += OnPathPlayed;
        tableDeckManager.EncounterEnded += Show;
        Hide();
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= Hide;
        tableDeckManager.PathPlayed -= OnPathPlayed;
        tableDeckManager.EncounterEnded -= Show;
    }

    void Hide()
    {
        shown = false;
        group.DOKill();
        group.alpha = 0f;
        group.blocksRaycasts = false;
    }

    // All the cards turned: told at once, even if the encounter did not end with this path.
    void OnPathPlayed(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        if (BarEvents.AllCardsTurned(tableDeckManager.Table))
        {
            Show(pathNumber);
        }
    }

    void Show(int pathsPlayed)
    {
        if (shown)
        {
            return;
        }
        shown = true;
        TombolaCard card = new(tableDeckManager.Table);
        string paths = pathsPlayed == 1 ? "1 path giocato" : $"{pathsPlayed} path giocati";
        string shapes =
            card.CompleteShapes.Count == 0 ? "nessuna forma" : $"forme: {card.Summary()}";
        string reason;
        if (rpgView.DefeatText != null)
        {
            title.text = "Sconfitto!";
            reason = rpgView.DefeatText;
        }
        else if (BarEvents.AllCardsTurned(tableDeckManager.Table))
        {
            title.text = "Tutte le carte girate!";
            reason = "Hai girato tutte le carte del tavolo.";
        }
        else
        {
            title.text = "Quarto re!";
            reason = "L'incontro finisce qui.";
        }
        body.text = $"{reason}\n{paths}\n{shapes}";
        group.blocksRaycasts = true;
        group.DOFade(1f, fadeDuration).SetEase(fadeEase).SetLink(gameObject);
    }
}
