using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TEMP Bar-Michele (feature events-impl): copy of ReceiptView for the Bar-Michele test scene, with the receipts of
// the paths already played kept and browsable, the lines in a scroll view (a long path keeps its font size) and
// the lines of BarPathReceipt. To merge back or delete when one idea is chosen.
//
// The receipt (BarPathReceipt): a new receipt when a path starts, a line for every card as it is turned, the end of
// the path when it is over. While a path is played the receipt follows it and the arrows are off; between two
// paths the arrows go back and forth through the receipts of the game, from the first to the last. Cleared at
// every new game.
public class BarReceiptView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private TMP_Text text;

    [SerializeField, Required]
    private Button previousButton;

    [SerializeField, Required]
    private Button nextButton;

    [SerializeField, Required]
    private TMP_Text pageText;

    // Scroll view of the lines: follows the last card while a path is played, starts from the top otherwise.
    [SerializeField, Required]
    private ScrollRect scroll;

    readonly List<string> receipts = new(); // paths over, in order
    readonly StringBuilder current = new(); // path being played
    bool playing;
    int shown; // index in receipts of the receipt shown between two paths

    void OnEnable()
    {
        tableDeckManager.GameStarted += ShowEmpty;
        tableDeckManager.PathStarted += StartReceipt;
        tableDeckManager.MovePlayed += AddLine;
        tableDeckManager.PathPlayed += EndReceipt;
        previousButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);
        ShowEmpty();
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= ShowEmpty;
        tableDeckManager.PathStarted -= StartReceipt;
        tableDeckManager.MovePlayed -= AddLine;
        tableDeckManager.PathPlayed -= EndReceipt;
        previousButton.onClick.RemoveListener(ShowPrevious);
        nextButton.onClick.RemoveListener(ShowNext);
    }

    void ShowEmpty()
    {
        receipts.Clear();
        current.Clear();
        playing = false;
        shown = 0;
        Refresh();
    }

    void StartReceipt(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        playing = true;
        current.Clear();
        current.Append(PathReceipt.Header(pathNumber));
        Refresh();
    }

    // A bench card filling the starting slot is not a card of the path: it is in the footer.
    void AddLine(int moveIndex, PathMove move)
    {
        if (move.End == PathEnd.FilledFromBench)
        {
            return;
        }
        current.Append('\n').Append(BarPathReceipt.Line(move.Card));
        Refresh();
    }

    void EndReceipt(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        current.Append('\n').Append(BarPathReceipt.Footer(moves));
        receipts.Add(current.ToString());
        playing = false;
        shown = receipts.Count - 1;
        Refresh();
    }

    void ShowPrevious()
    {
        if (!playing && shown > 0)
        {
            shown--;
            Refresh();
        }
    }

    void ShowNext()
    {
        if (!playing && shown < receipts.Count - 1)
        {
            shown++;
            Refresh();
        }
    }

    void Refresh()
    {
        if (playing)
        {
            text.text = current.ToString();
            pageText.text = $"path {receipts.Count + 1} in corso";
        }
        else if (receipts.Count == 0)
        {
            text.text = PathReceipt.Empty;
            pageText.text = "";
        }
        else
        {
            text.text = receipts[shown];
            pageText.text = $"{shown + 1} di {receipts.Count}";
        }
        // The size of the lines changes with the text: rebuild it before moving the scroll view.
        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = playing ? 0 : 1;
        previousButton.interactable = !playing && shown > 0;
        nextButton.interactable = !playing && shown < receipts.Count - 1;
    }
}
