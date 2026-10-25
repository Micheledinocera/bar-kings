using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

// The road of the Bar scene: when a path starts the road is cleared, then each card of the path is hung on the road
// as it is turned and the walker (the omino) walks to it, lighting it when it arrives. Only the cards: the events of
// the cards and the RPG part are not decided yet (wiki business/parte-rpg.md). The stops and the walker are children
// of road, anchored to its left edge.
public class PathRoadView : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private RectTransform road;

    [SerializeField, Required, AssetsOnly]
    private RoadStopView stopPrefab;

    [SerializeField, Required]
    private RectTransform walker;

    [SerializeField]
    private float margin = 110f; // from the edges of the road to the first and last stop

    [SerializeField]
    private float maxSpacing = 200f; // between two stops, for short paths

    [SerializeField]
    private float stepDuration = 0.45f; // walk from one stop to the next; a faster path cuts it short

    [SerializeField]
    private float hopHeight = 8f;

    [SerializeField]
    private Ease stepEase = Ease.InOutSine;

    // One stop per card of the path, in order; hidden until its card is turned.
    readonly List<RoadStopView> stops = new();
    readonly List<float> stopX = new();
    Sequence walk;
    float walkerY; // height of the walker on the road; a walk killed mid-hop goes back to it

    void Awake() => walkerY = walker.anchoredPosition.y;

    void OnEnable()
    {
        tableDeckManager.GameStarted += Clear;
        tableDeckManager.PathStarted += LayOut;
        tableDeckManager.MovePlayed += WalkTo;
    }

    void OnDisable()
    {
        tableDeckManager.GameStarted -= Clear;
        tableDeckManager.PathStarted -= LayOut;
        tableDeckManager.MovePlayed -= WalkTo;
    }

    void Start() => Clear();

    void Clear()
    {
        walk?.Kill();
        foreach (RoadStopView stop in stops)
        {
            Destroy(stop.gameObject);
        }
        stops.Clear();
        stopX.Clear();
        walker.anchoredPosition = new Vector2(margin / 2, walkerY);
    }

    // Places a hidden stop for every card of the path, so the road is spaced for the whole path.
    void LayOut(int pathNumber, IReadOnlyList<PathMove> moves)
    {
        Clear();
        List<CardDataInGame> cards = moves
            .Where(move => move.End != PathEnd.FilledFromBench)
            .Select(move => move.Card)
            .ToList();
        float spacing =
            cards.Count > 1
                ? Mathf.Min(maxSpacing, (road.rect.width - 2 * margin) / (cards.Count - 1))
                : 0f;
        for (int i = 0; i < cards.Count; i++)
        {
            RoadStopView stop = Instantiate(stopPrefab, road);
            RectTransform stopRect = (RectTransform)stop.transform;
            float x = margin + spacing * i;
            stopRect.anchoredPosition = new Vector2(x, stopRect.anchoredPosition.y);
            stop.Show(cards[i]);
            stop.gameObject.SetActive(false);
            stops.Add(stop);
            stopX.Add(x);
        }
        // The walker stays above its own stops, drawn last.
        walker.SetAsLastSibling();
    }

    // The card of move moveIndex was turned: its stop appears and the walker walks to it. A walk still going on is
    // cut short, and the stops it was heading to are lit.
    void WalkTo(int moveIndex, PathMove move)
    {
        if (move.End == PathEnd.FilledFromBench || moveIndex >= stops.Count)
        {
            return;
        }
        walk?.Kill();
        for (int i = 0; i < moveIndex; i++)
        {
            stops[i].SetReached(true);
        }
        RoadStopView stop = stops[moveIndex];
        stop.gameObject.SetActive(true);

        walker.anchoredPosition = new Vector2(walker.anchoredPosition.x, walkerY);
        walk = DOTween
            .Sequence()
            .SetLink(gameObject)
            .Append(walker.DOAnchorPosX(stopX[moveIndex], stepDuration).SetEase(stepEase))
            .Join(
                walker
                    .DOAnchorPosY(walkerY + hopHeight, stepDuration / 2)
                    .SetEase(Ease.OutQuad)
                    .SetLoops(2, LoopType.Yoyo)
            )
            .AppendCallback(() => stop.SetReached(true));
    }
}
