using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Summary of a fight in the TribunaRpg scene, over the whole screen: it blocks the table and the buttons until
// it is closed, and the resolution of the path waits for it (RpgStatsView). Inactive while hidden.
public class FightPopup : MonoBehaviour
{
    [SerializeField, Required]
    private TMP_Text title;

    [SerializeField, Required]
    private TMP_Text body;

    [SerializeField, Required]
    private Button closeButton;

    // Shows the popup and completes when the close button is clicked.
    public async UniTask Show(
        string titleText,
        string bodyText,
        CancellationToken cancellationToken
    )
    {
        title.text = titleText;
        body.text = bodyText;
        gameObject.SetActive(true);
        UniTaskCompletionSource closed = new();
        UnityAction onClose = () => closed.TrySetResult();
        closeButton.onClick.AddListener(onClose);
        try
        {
            await closed.Task.AttachExternalCancellation(cancellationToken);
        }
        finally
        {
            closeButton.onClick.RemoveListener(onClose);
            if (this != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
