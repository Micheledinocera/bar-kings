using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

// Setup and view of the table scenes (Panchina, Tribuna, Index40): builds the deck from a preset, deals it, mirrors
// TableState, animates paths and writes the game log.
public class TableDeckManager : MonoBehaviour
{
    public static TableDeckManager instance;
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo

    [SerializeField]
    private CardDatabase deckDatabase; // Assets/Instances/CardDatabase.asset

    // Preset of the next game: the options panel changes it, the game in progress keeps its own until Reset. Empty:
    // the first preset of the library, or the normal deck when the library is empty.
    [SerializeField]
    private DeckPreset deckPreset; // Assets/Instances/DeckPresets

    [SerializeField, Required]
    private DeckPresetLibrary presetLibrary; // Assets/Instances/DeckPresetLibrary.asset

    // Seed of the deck: 0 draws a new one at every game. The seed is logged, so a game can be replayed.
    [SerializeField]
    private int seed;

    [SerializeField]
    private ExcessCards excessCards = ExcessCards.Bench;

    // The encounter ends when the 4th king is face up: no path can be played until Reset. Rule being tried for the
    // tribuna (wiki business/modifica-del-mazzo.md, decision in analysis); off: the game goes on while face-down
    // cards are left.
    [SerializeField]
    private bool endAtFourthKing;

    // On-screen log; optional, the Console always gets the log.
    [SerializeField]
    private GameLogView gameLog;

    // Animation durations per speed level; the options panel changes the level, the next animation uses it.
    [SerializeField]
    private AnimationSpeed animationSpeed = AnimationSpeed.Normal;

    [SerializeField]
    private AnimationTiming slowTiming = new(0.2f, 0.5f);

    [SerializeField]
    private AnimationTiming normalTiming = new(0.1f, 0.2f);

    [SerializeField]
    private AnimationTiming fastTiming = new(0.03f, 0.05f);

    const float StackOffset = 0.02f; // height of a card on top of a duplicate

    // Card GameObjects of the game, table cards first, then the bench pile.
    readonly List<GameObject> deckCards = new();

    // Logical table state, built after dealing; cards on screen mirror it.
    TableState table;
    readonly Dictionary<CardDataInGame, CardDisplay3D> cardViews = new();
    DeckPreset currentPreset; // preset of the game in progress; null for the normal deck
    int pathCount;
    bool allKingsLogged;
    bool encounterOver; // endAtFourthKing and the 4th king is face up

    public CardDatabase DeckDatabase => deckDatabase;

    public ExcessCards Excess => excessCards;

    public DeckPreset CurrentPreset => currentPreset;

    public DeckPreset NextPreset => deckPreset;

    public AnimationSpeed Speed => animationSpeed;

    // Table of the game in progress; null while the first deal runs. Read by the HUD, never changed from outside.
    public TableState Table => table;

    // Raised when a deal is over and Table is the new game's (first game and every Reset).
    public event Action GameStarted;

    // Raised when a path starts, before its animation: the path number in the game and all its moves, in order.
    public event Action<int, IReadOnlyList<PathMove>> PathStarted;

    // Raised when the animation of one move of the path is over: its position in the moves and the move.
    public event Action<int, PathMove> MovePlayed;

    // Raised when the animation of a path is over: the path number in the game and its moves, in order.
    public event Action<int, IReadOnlyList<PathMove>> PathPlayed;

    // Raised after PathPlayed when the path turned the 4th king and endAtFourthKing is on: the paths played.
    public event Action<int> EncounterEnded;

    // The preset of the next game, from the next Reset.
    public void SetNextPreset(DeckPreset preset) => deckPreset = preset;

    public void SetSpeed(AnimationSpeed speed) => animationSpeed = speed;

    AnimationTiming Timing =>
        animationSpeed switch
        {
            AnimationSpeed.Slow => slowTiming,
            AnimationSpeed.Fast => fastTiming,
            _ => normalTiming,
        };

    void Start() => SetupTableAsync(destroyCancellationToken).Forget();

    async UniTaskVoid SetupTableAsync(CancellationToken cancellationToken)
    {
        if (instance == null)
        {
            instance = this;
        }

        GameManager.isClickBlocked = true;
        int gameSeed = seed != 0 ? seed : UnityEngine.Random.Range(1, int.MaxValue);
        if (deckPreset == null)
        {
            deckPreset = presetLibrary.Presets.FirstOrDefault();
        }
        currentPreset = deckPreset;
        DeckDeal deal = DeckBuilder.Build(
            excessCards,
            deckDatabase.CreateCardsInGame(),
            currentPreset,
            new System.Random(gameSeed)
        );
        List<CardDataInGame> benchCards = deal.BenchCards(excessCards).ToList();

        pathCount = 0;
        allKingsLogged = false;
        encounterOver = false;
        if (gameLog != null)
        {
            gameLog.Clear();
        }
        Log(
            GameLogFormatter.FormatDeal(
                DeckPreset.NameOf(currentPreset),
                gameSeed,
                deal,
                excessCards
            )
        );

        ShowSlots(deal.TableSlots.Count);
        RenderDeck(deal.TableSlots, benchCards);
        await MoveCards(deal.TableSlots, cancellationToken);
        table = new TableState(deal.TableSlots, benchCards);
        GameManager.isClickBlocked = false;
        GameStarted?.Invoke();
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

    // Grid position of a slot: rows of 10, 4 for the base table, more in index>40.
    public static Vector3 SlotPosition(int slot) =>
        new(
            -5 + (1.2f * (slot % DeckRules.SlotsPerRow)),
            0.25f,
            4.5f - (1.5f * (slot / DeckRules.SlotsPerRow))
        );

    // Plays the path starting from a clicked card: the table computes the moves, the scene animates them.
    public async UniTask PlayPath(CardDisplay3D clickedCard)
    {
        int startSlot = table.SlotOf(clickedCard.cardData);
        if (encounterOver || startSlot < 0 || table.IsFaceUp(startSlot))
        {
            return;
        }

        List<PathMove> moves = table.PlayPath(startSlot);
        pathCount++;
        int faceUpKings = table.FaceUpKings;
        Log(GameLogFormatter.FormatPath(pathCount, startSlot, moves, faceUpKings));
        if (!allKingsLogged && faceUpKings == DeckRules.KingCount)
        {
            allKingsLogged = true;
            encounterOver = endAtFourthKing;
            Log(GameLogFormatter.FormatAllKings(pathCount));
        }

        PathStarted?.Invoke(pathCount, moves);
        CancellationToken cancellationToken = destroyCancellationToken;
        for (int i = 0; i < moves.Count; i++)
        {
            CardDisplay3D card = cardViews[moves[i].Card];
            Vector3 target =
                SlotPosition(moves[i].ToSlot) + Vector3.up * (StackOffset * moves[i].StackIndex);
            if (moves[i].End == PathEnd.FilledFromBench)
            {
                // From the panchina pile to the hole, face down.
                await TableCardAnimations
                    .MoveAnimation(card.gameObject, target, Timing.pathMoveDuration)
                    .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
            }
            else
            {
                // Only a move that goes on displaces the card of the next move.
                CardDisplay3D displaced =
                    moves[i].End == PathEnd.None ? cardViews[moves[i + 1].Card] : null;
                card.isFlipped = true;
                await TableCardAnimations
                    .PathMoveAnimation(card, displaced, target, i == 0, Timing.pathMoveDuration)
                    .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
            }
            card.transform.SetParent(GetSlotByIndex(moves[i].ToSlot), true);
            MovePlayed?.Invoke(i, moves[i]);
        }
        PathPlayed?.Invoke(pathCount, moves);
        if (encounterOver)
        {
            EncounterEnded?.Invoke(pathCount);
        }
    }

    public Transform GetSlotByIndex(int index)
    {
        return deckPosition.GetChild(0).GetChild(index);
    }

    // Index40 has a slot for every index up to DeckRules.MaxTableSlots: the slots beyond the deal are hidden.
    void ShowSlots(int slotCount)
    {
        Transform slots = deckPosition.GetChild(0);
        for (int i = 0; i < slots.childCount; i++)
        {
            slots.GetChild(i).gameObject.SetActive(i < slotCount);
        }
    }

    void Log(string entry)
    {
        Debug.Log(entry);
        if (gameLog != null)
        {
            gameLog.Append(entry);
        }
    }

    // One pile: the table cards on top, in slot order, the panchina below them.
    void RenderDeck(IReadOnlyList<CardDataInGame> tableSlots, List<CardDataInGame> benchCards)
    {
        deckCards.Clear();
        cardViews.Clear();
        List<CardDataInGame> pile = tableSlots
            .Where(card => card != null)
            .Concat(benchCards)
            .ToList();
        for (int i = 0; i < pile.Count; i++)
        {
            GameObject nuovaCarta = Instantiate(
                cardPrefab,
                deckPosition.position + new Vector3(0, 0.02f, 0.02f) * (pile.Count - 1 - i),
                Quaternion.identity,
                deckPosition
            );
            CardDisplay3D view = nuovaCarta.GetComponent<CardDisplay3D>();
            view.Setup(pile[i]);
            nuovaCarta.name = pile[i].cardData.nome;
            deckCards.Add(nuovaCarta);
            cardViews.Add(pile[i], view);
        }
    }

    // Deals the table cards to their slots; holes get no card.
    async UniTask MoveCards(
        IReadOnlyList<CardDataInGame> tableSlots,
        CancellationToken cancellationToken
    )
    {
        for (int slot = 0; slot < tableSlots.Count; slot++)
        {
            if (tableSlots[slot] == null)
            {
                continue;
            }
            GameObject card = cardViews[tableSlots[slot]].gameObject;
            await TableCardAnimations
                .MoveAnimation(card, SlotPosition(slot), Timing.dealMoveDuration)
                .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
            card.transform.SetParent(GetSlotByIndex(slot));
        }
    }
}
