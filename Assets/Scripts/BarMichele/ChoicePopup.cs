using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// TEMP Bar-Michele (feature events-impl): a choice between two answers asked mid-path, for the card events of
// BarEvents (a king: escape or fight; a fante: shop or skip). Over the whole screen like FightPopup: the path
// waits for the answer (BarRpgView). The labels of the two buttons are set at every question. Inactive while
// hidden.
public class ChoicePopup : MonoBehaviour
{
    [SerializeField, Required]
    private TMP_Text title;

    [SerializeField, Required]
    private TMP_Text body;

    [SerializeField, Required, FormerlySerializedAs("escapeButton")]
    private Button firstButton;

    [SerializeField, Required, FormerlySerializedAs("fightButton")]
    private Button secondButton;

    // Shows the popup and completes with true when the first button is clicked, false for the second.
    public async UniTask<bool> Ask(
        string titleText,
        string bodyText,
        string firstLabel,
        string secondLabel,
        CancellationToken cancellationToken
    )
    {
        title.text = titleText;
        body.text = bodyText;
        firstButton.GetComponentInChildren<TMP_Text>().text = firstLabel;
        secondButton.GetComponentInChildren<TMP_Text>().text = secondLabel;
        gameObject.SetActive(true);
        UniTaskCompletionSource<bool> answered = new();
        UnityAction onFirst = () => answered.TrySetResult(true);
        UnityAction onSecond = () => answered.TrySetResult(false);
        firstButton.onClick.AddListener(onFirst);
        secondButton.onClick.AddListener(onSecond);
        try
        {
            return await answered.Task.AttachExternalCancellation(cancellationToken);
        }
        finally
        {
            firstButton.onClick.RemoveListener(onFirst);
            secondButton.onClick.RemoveListener(onSecond);
            if (this != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
