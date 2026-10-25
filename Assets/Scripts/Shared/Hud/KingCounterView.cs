using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Kings face up in the Bar scene: "Re scoperti N di 4" and one crown per king, lit when the king is face up.
public class KingCounterView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private TMP_Text label;

    [SerializeField, Required]
    private Image[] crowns = new Image[DeckRules.KingCount];

    [SerializeField]
    private Color faceUpColor = new(0.788f, 0.635f, 0.153f); // Nicotina #C9A227

    [SerializeField]
    private Color faceDownColor = new(0.361f, 0.275f, 0.188f); // #5C4630

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
        int kings = tableDeckManager.Table?.FaceUpKings ?? 0;
        label.text = $"Re scoperti {kings} di {DeckRules.KingCount}";
        for (int i = 0; i < crowns.Length; i++)
        {
            crowns[i].color = i < kings ? faceUpColor : faceDownColor;
        }
    }
}
