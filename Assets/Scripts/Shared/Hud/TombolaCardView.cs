using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

// The tombola card of the Bar scene (TombolaCard): one cell per slot of the base grid, in slot order, refreshed at
// every new game and after every path.
public class TombolaCardView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    // DeckRules.TableSlots cells, in slot order (rows = suits, columns = values).
    [SerializeField, Required]
    private TombolaCellView[] cells = new TombolaCellView[DeckRules.TableSlots];

    [SerializeField, Required]
    private TMP_Text summary;

    void OnEnable()
    {
        tableDeckManager.GameStarted += Refresh;
        tableDeckManager.PathPlayed += OnPathPlayed;
        Refresh();
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= Refresh;
        tableDeckManager.PathPlayed -= OnPathPlayed;
    }

    void OnPathPlayed(int pathNumber, IReadOnlyList<PathMove> moves) => Refresh();

    void Refresh()
    {
        if (tableDeckManager.Table == null)
        {
            foreach (TombolaCellView cell in cells)
            {
                cell.Show(TombolaCell.Empty);
            }
            summary.text = string.Empty;
            return;
        }
        TombolaCard card = new(tableDeckManager.Table);
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Show(card.Cells[i]);
        }
        summary.text = card.Summary();
    }
}
