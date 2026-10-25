using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A stop of the road of the Bar scene: a card of the path, dimmed until the walker reaches it.
public class RoadStopView : MonoBehaviour
{
    [SerializeField, Required]
    private Image card;

    [SerializeField, Required]
    private TMP_Text label;

    [SerializeField, Required]
    private CanvasGroup group;

    [SerializeField, Range(0f, 1f)]
    private float notReachedAlpha = 0.35f;

    public void Show(CardDataInGame stop)
    {
        card.sprite = stop.cardData.immagineFronte;
        label.text = PathReceipt.CardLabel(stop);
        SetReached(false);
    }

    public void SetReached(bool reached) => group.alpha = reached ? 1f : notReachedAlpha;
}
