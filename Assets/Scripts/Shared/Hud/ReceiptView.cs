using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

// The receipt of the Bar scene (PathReceipt): a new receipt when a path starts, a line for every card as it is
// turned, the end of the path when it is over. Cleared at every new game.
public class ReceiptView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private TMP_Text text;

    readonly StringBuilder receipt = new();

    void OnEnable()
    {
        tableDeckManager.GameStarted += ShowEmpty;
        tableDeckManager.PathStarted += StartReceipt;
        tableDeckManager.MovePlayed += AddLine;
        tableDeckManager.PathPlayed += EndReceipt;
        ShowEmpty();
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= ShowEmpty;
        tableDeckManager.PathStarted -= StartReceipt;
        tableDeckManager.MovePlayed -= AddLine;
        tableDeckManager.PathPlayed -= EndReceipt;
    }

    void ShowEmpty()
    {
        receipt.Clear();
        text.text = PathReceipt.Empty;
    }

    void StartReceipt(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        receipt.Clear();
        receipt.Append(PathReceipt.Header(pathNumber));
        text.text = receipt.ToString();
    }

    // A bench card filling the starting slot is not a card of the path: it is in the footer.
    void AddLine(int moveIndex, PathMove move)
    {
        if (move.End == PathEnd.FilledFromBench)
        {
            return;
        }
        receipt.Append('\n').Append(PathReceipt.Line(moveIndex + 1, move.Card));
        text.text = receipt.ToString();
    }

    void EndReceipt(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        receipt.Append('\n').Append(PathReceipt.Footer(moves));
        text.text = receipt.ToString();
    }
}
