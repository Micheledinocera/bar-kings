using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DeckManagerPanchina : MonoBehaviour
{
    public static DeckManagerPanchina instance;
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo

    [SerializeField]
    private CardDatabase deckDatabase; // Assets/Instances/CardDatabase.asset
    public static List<CardData> cardDatabase = new();
    public static List<CardDataInGame> cardDatabaseInGame = new();
    public static List<CardDataInGame> shuffledCardDatabaseInGame = new();
    public static List<GameObject> deckCards = new();
    List<CardDataInGame> iQuattroRe = new();
    List<CardDataInGame> altreCarte = new();
    const int DECK_LIMIT = 40; // cards on the table; the others wait in the panchina
    const int KingValue = 10;
    const int KingCount = 4;

    // Cards beyond the table, face down in the pile at deckPosition: they fill the holes left by paths.
    readonly List<CardDataInGame> benchCards = new();

    [SerializeField, Range(0, 36)]
    private int duplicateCount = 9; // duplicates of the first non-king cards (ace, two, ... of Denari)

    [SerializeField, Range(0.02f, 1f)]
    private float dealMoveDuration = 0.1f; // seconds per card when dealing

    [SerializeField, Range(0.05f, 2f)]
    private float pathMoveDuration = 0.5f; // seconds per flip / move in a path

    const float StackOffset = 0.02f; // height of a card on top of a duplicate

    // Logical table state, built after dealing; cards on screen mirror it.
    PanchinaTable table;
    readonly Dictionary<CardDataInGame, CardDisplay3D> cardViews = new();

    void Start() => SetupTableAsync(destroyCancellationToken).Forget();

    async UniTaskVoid SetupTableAsync(CancellationToken cancellationToken)
    {
        if (instance == null)
        {
            instance = this;
        }

        GameManager.isClickBlocked = true;
        cardDatabase = deckDatabase.Cards.ToList();
        cardDatabaseInGame = deckDatabase.CreateCardsInGame();
        iQuattroRe = cardDatabaseInGame.Where(c => c.cardData.valore == KingValue).ToList();
        altreCarte = cardDatabaseInGame.Where(c => c.cardData.valore != KingValue).ToList();
        if (iQuattroRe.Count != KingCount)
        {
            throw new System.InvalidOperationException(
                $"{deckDatabase.name} has {iQuattroRe.Count} kings, expected {KingCount}"
            );
        }
        shuffledCardDatabaseInGame.Clear();
        deckCards.Clear();
        benchCards.Clear();
        shuffledCardDatabaseInGame.AddRange(altreCarte);
        AddDoppioni();
        ShuffleDeck();
        AddRe();
        ShuffleDeck();
        RenderDeck();
        await MoveCards(cancellationToken);
        BuildTable();
        GameManager.isClickBlocked = false;
    }

    // Starts a new game: removes every card and deals again. Called by the Reset button; ignored while cards
    // are dealt or a path is played (clicks blocked), so nothing is animating when the cards are destroyed.
    public void ResetTable()
    {
        if (GameManager.isClickBlocked)
        {
            return;
        }
        foreach (GameObject card in deckCards)
        {
            Destroy(card);
        }
        SetupTableAsync(destroyCancellationToken).Forget();
    }

    // Grid position of a slot: 4 rows of 10.
    public static Vector3 SlotPosition(int slot) =>
        new(-5 + (1.2f * (slot % 10)), 0.25f, 4.5f - (1.5f * (slot / 10)));

    void BuildTable()
    {
        cardViews.Clear();
        foreach (GameObject card in deckCards)
        {
            CardDisplay3D view = card.GetComponent<CardDisplay3D>();
            cardViews.Add(view.cardData, view);
        }
        table = new PanchinaTable(shuffledCardDatabaseInGame, benchCards);
    }

    // Plays the path starting from a clicked card: the table computes the moves, the scene animates them.
    public async UniTask PlayPath(CardDisplay3D clickedCard)
    {
        int startSlot = table.SlotOf(clickedCard.cardData);
        if (startSlot < 0 || table.IsFaceUp(startSlot))
        {
            return;
        }

        List<PathMove> moves = table.PlayPath(startSlot);
        CancellationToken cancellationToken = destroyCancellationToken;
        for (int i = 0; i < moves.Count; i++)
        {
            CardDisplay3D card = cardViews[moves[i].Card];
            Vector3 target =
                SlotPosition(moves[i].ToSlot) + Vector3.up * (StackOffset * moves[i].StackIndex);
            if (moves[i].End == PathEnd.FilledFromBench)
            {
                // From the panchina pile to the hole, face down.
                await CardAnimationsPanchina
                    .MoveAnimation(card.gameObject, target, pathMoveDuration)
                    .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
            }
            else
            {
                // Only a move that goes on displaces the card of the next move.
                CardDisplay3D displaced =
                    moves[i].End == PathEnd.None ? cardViews[moves[i + 1].Card] : null;
                card.isFlipped = true;
                await CardAnimationsPanchina
                    .PathMoveAnimation(card, displaced, target, i == 0, pathMoveDuration)
                    .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
            }
            card.transform.SetParent(GetSlotByIndex(moves[i].ToSlot), true);
        }
    }

    public Transform GetSlotByIndex(int index)
    {
        return deckPosition.GetChild(0).GetChild(index);
    }

    // The table always has the 4 kings: they are added after the cut. The cards in excess (non-kings only, duplicates
    // included) go to the panchina.
    private void AddRe()
    {
        int tableCards = DECK_LIMIT - iQuattroRe.Count;
        benchCards.AddRange(shuffledCardDatabaseInGame.Skip(tableCards));
        shuffledCardDatabaseInGame = shuffledCardDatabaseInGame.Take(tableCards).ToList();
        shuffledCardDatabaseInGame.AddRange(iQuattroRe);
    }

    private void AddDoppioni()
    {
        for (int i = 0; i < duplicateCount; i++)
        {
            // Duplicates of non-king cards only. A new instance: on the table every card, duplicates included, is a
            // distinct object.
            shuffledCardDatabaseInGame.Add(
                new CardDataInGame(altreCarte[i].cardData, altreCarte[i].index)
            );
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < shuffledCardDatabaseInGame.Count; i++)
        {
            CardDataInGame temp = shuffledCardDatabaseInGame[i];
            int randomIndex = Random.Range(i, shuffledCardDatabaseInGame.Count);
            shuffledCardDatabaseInGame[i] = shuffledCardDatabaseInGame[randomIndex];
            shuffledCardDatabaseInGame[randomIndex] = temp;
        }
    }

    // One pile: the table cards on top (deckCards[i] goes to slot i), the panchina below them.
    void RenderDeck()
    {
        List<CardDataInGame> pile = shuffledCardDatabaseInGame.Concat(benchCards).ToList();
        for (int i = 0; i < pile.Count; i++)
        {
            GameObject nuovaCarta = Instantiate(
                cardPrefab,
                deckPosition.position + new Vector3(0, 0.02f, 0.02f) * (pile.Count - 1 - i),
                Quaternion.identity,
                deckPosition
            );
            nuovaCarta.GetComponent<CardDisplay3D>().Setup(pile[i]);
            nuovaCarta.name = nuovaCarta.GetComponent<CardDisplay3D>().cardData.cardData.nome;
            deckCards.Add(nuovaCarta);
        }
    }

    async UniTask MoveCards(CancellationToken cancellationToken)
    {
        for (int i = 0; i < DECK_LIMIT; i++)
        {
            Vector3 cardPosition = SlotPosition(i);
            await CardAnimationsPanchina
                .MoveAnimation(deckCards[i], cardPosition, dealMoveDuration)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
            deckCards[i].transform.SetParent(GetSlotByIndex(i));
        }
    }
}
