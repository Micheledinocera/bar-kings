using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// TEMP Bar-Michele (feature events-impl): asks the player, when a king is turned and an escape is left, whether to
// escape (no fight, one escape used) or to fight (HorseEscape). Over the whole screen like FightPopup: the path
// waits for the answer (BarRpgView). Inactive while hidden.
public class EscapeChoicePopup : MonoBehaviour
{
    [SerializeField, Required]
    private TMP_Text title;

    [SerializeField, Required]
    private TMP_Text body;

    [SerializeField, Required]
    private Button escapeButton;

    [SerializeField, Required]
    private Button fightButton;

    // Shows the popup and completes with true when the player escapes, false when they fight.
    public async UniTask<bool> Ask(
        string titleText,
        string bodyText,
        CancellationToken cancellationToken
    )
    {
        title.text = titleText;
        body.text = bodyText;
        gameObject.SetActive(true);
        UniTaskCompletionSource<bool> answered = new();
        UnityAction onEscape = () => answered.TrySetResult(true);
        UnityAction onFight = () => answered.TrySetResult(false);
        escapeButton.onClick.AddListener(onEscape);
        fightButton.onClick.AddListener(onFight);
        try
        {
            return await answered.Task.AttachExternalCancellation(cancellationToken);
        }
        finally
        {
            escapeButton.onClick.RemoveListener(onEscape);
            fightButton.onClick.RemoveListener(onFight);
            if (this != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
