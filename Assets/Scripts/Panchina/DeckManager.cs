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
    int DECK_LIMIT = 40;

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
        iQuattroRe = cardDatabaseInGame.Where(c => c.cardData.valore == 10).ToList();
        altreCarte = cardDatabaseInGame.Where(c => c.cardData.valore != 10).ToList();
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

    // Grid position of a slot: 4 rows of 10.
    public static Vector3 SlotPosition(int slot) =>
        new(-5 + (1.2f * (slot % 10)), 0.25f, 4.5f - (1.5f * (slot / 10)));

    void BuildTable()
    {
        cardViews.Clear();
        List<CardDataInGame> dealt = new(deckCards.Count);
        foreach (GameObject card in deckCards)
        {
            CardDisplay3D view = card.GetComponent<CardDisplay3D>();
            dealt.Add(view.cardData);
            cardViews.Add(view.cardData, view);
        }
        table = new PanchinaTable(dealt);
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
            CardDisplay3D displaced = i + 1 < moves.Count ? cardViews[moves[i + 1].Card] : null;
            Vector3 target =
                SlotPosition(moves[i].ToSlot) + Vector3.up * (StackOffset * moves[i].StackIndex);
            card.isFlipped = true;
            await CardAnimationsPanchina
                .PathMoveAnimation(card, displaced, target, i == 0, pathMoveDuration)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
            card.transform.SetParent(GetSlotByIndex(moves[i].ToSlot), true);
        }
    }

    public Transform GetSlotByIndex(int index)
    {
        return deckPosition.GetChild(0).GetChild(index);
    }

    private void AddRe()
    {
        shuffledCardDatabaseInGame = shuffledCardDatabaseInGame
            .Where((c, i) => i < DECK_LIMIT - iQuattroRe.Count())
            .ToList();
        shuffledCardDatabaseInGame.AddRange(iQuattroRe);
    }

    private void AddDoppioni()
    {
        for (int i = 0; i < 9; i++)
        {
            // A new instance: on the table every card, duplicates included, is a distinct object.
            shuffledCardDatabaseInGame.Add(
                new CardDataInGame(cardDatabaseInGame[i].cardData, cardDatabaseInGame[i].index)
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

    void RenderDeck()
    {
        for (int i = shuffledCardDatabaseInGame.Count - 1; i >= 0; i--)
        {
            GameObject nuovaCarta = Instantiate(
                cardPrefab,
                deckPosition.position + new Vector3(0, 0.02f, 0.02f) * i,
                Quaternion.identity,
                deckPosition
            );
            nuovaCarta.GetComponent<CardDisplay3D>().Setup(shuffledCardDatabaseInGame[i]);
            nuovaCarta.name = nuovaCarta.GetComponent<CardDisplay3D>().cardData.cardData.nome;
            deckCards.Add(nuovaCarta);
        }
    }

    async UniTask MoveCards(CancellationToken cancellationToken)
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            Vector3 cardPosition = SlotPosition(i);
            await CardAnimationsPanchina
                .MoveAnimation(deckCards[i], cardPosition, dealMoveDuration)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
            deckCards[i].transform.SetParent(GetSlotByIndex(i));
        }
    }
}
