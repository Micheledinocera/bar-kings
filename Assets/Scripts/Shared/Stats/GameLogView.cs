using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Scrollable on-screen game log of the table scenes: entries are appended at the bottom and the view follows them.
public class GameLogView : MonoBehaviour
{
    [SerializeField, Required]
    private TMP_Text text;

    [SerializeField, Required]
    private ScrollRect scrollRect;

    readonly StringBuilder entries = new();

    // Called by the Log button of the table scenes; entries keep being added while the panel is hidden.
    public void ToggleVisible() => gameObject.SetActive(!gameObject.activeSelf);

    public void Clear()
    {
        entries.Clear();
        text.text = string.Empty;
    }

    public void Append(string entry)
    {
        if (entries.Length > 0)
        {
            entries.Append("\n\n");
        }
        entries.Append(entry);
        text.text = entries.ToString();
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
