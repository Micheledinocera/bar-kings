using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

// End of the encounter in the Bar scene: when the 4th king is face up (TableDeckManager.EncounterEnded) a panel
// fades in over the table with the paths played and the shapes of holes made; hidden again at the next game.
public class EncounterEndView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private CanvasGroup group;

    [SerializeField, Required]
    private TMP_Text body;

    [SerializeField]
    private float fadeDuration = 0.4f;

    [SerializeField]
    private Ease fadeEase = Ease.OutQuad;

    void OnEnable()
    {
        tableDeckManager.GameStarted += Hide;
        tableDeckManager.EncounterEnded += Show;
        Hide();
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= Hide;
        tableDeckManager.EncounterEnded -= Show;
    }

    void Hide()
    {
        group.DOKill();
        group.alpha = 0f;
        group.blocksRaycasts = false;
    }

    void Show(int pathsPlayed)
    {
        TombolaCard card = new(tableDeckManager.Table);
        string paths = pathsPlayed == 1 ? "1 path giocato" : $"{pathsPlayed} path giocati";
        string shapes =
            card.CompleteShapes.Count == 0 ? "nessuna forma" : $"forme: {card.Summary()}";
        body.text = $"L'incontro finisce qui.\n{paths}\n{shapes}";
        group.blocksRaycasts = true;
        group.DOFade(1f, fadeDuration).SetEase(fadeEase).SetLink(gameObject);
    }
}
