using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Linq;

public class DeckManagerPanchina : MonoBehaviour
{
    public static DeckManagerPanchina instance;
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo
    public static List<CardData> cardDatabase = new();
    public static List<CardDataInGame> cardDatabaseInGame = new();
    public static List<CardDataInGame> shuffledCardDatabaseInGame = new();
    public static List<GameObject> deckCards = new();
    List<CardDataInGame> iQuattroRe = new();
    List<CardDataInGame> altreCarte = new();
    int DECK_LIMIT = 40;
    async void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        GameManager.isClickBlocked = true;
        CardGeneratorPanchina.GenerateCards();
        iQuattroRe = cardDatabaseInGame.Where(c => c.cardData.valore == 10).ToList();
        altreCarte = cardDatabaseInGame.Where(c => c.cardData.valore != 10).ToList();
        shuffledCardDatabaseInGame.AddRange(altreCarte);
        // AddDoppioni();
        // ShuffleDeck();
        AddRe();
        ShuffleDeck();
        RenderDeck();
        await MoveCards();
        GameManager.isClickBlocked = false;
    }

    public Transform GetSlotByIndex(int index)
    {
        return deckPosition.GetChild(0).GetChild(index);
    }
    private void AddRe()
    {
        cardDatabaseInGame = shuffledCardDatabaseInGame.Where((c, i) => i < DECK_LIMIT - iQuattroRe.Count()).ToList();
        cardDatabaseInGame.AddRange(iQuattroRe);
    }

    private void AddDoppioni()
    {
        Debug.Log(cardDatabaseInGame.Count());
        for (int i = 0; i < 5; i++)
        {
            shuffledCardDatabaseInGame.Add(cardDatabaseInGame[i]);
            Debug.Log($"Aggiunto doppione {cardDatabaseInGame[i].cardData.nome} - {cardDatabaseInGame[i].cardData.seme}");
        }
    }

    void ShuffleDeck()
    {
        shuffledCardDatabaseInGame = cardDatabaseInGame;
        for (int i = 0; i < shuffledCardDatabaseInGame.Count; i++)
        {
            CardDataInGame temp = shuffledCardDatabaseInGame[i];
            int randomIndex = Random.Range(i, shuffledCardDatabaseInGame.Count);
            shuffledCardDatabaseInGame[i] = shuffledCardDatabaseInGame[randomIndex];
            shuffledCardDatabaseInGame[randomIndex] = temp;
        }

        Debug.Log("Mazzo mescolato!");
    }

    void RenderDeck()
    {
        for (int i = shuffledCardDatabaseInGame.Count - 1; i >= 0; i--)
        {
            GameObject nuovaCarta = Instantiate(cardPrefab, deckPosition.position + new Vector3(0, 0.02f, 0.02f) * i, Quaternion.identity, deckPosition);
            nuovaCarta.GetComponent<CardDisplay3D>().Setup(shuffledCardDatabaseInGame[i]);
            nuovaCarta.name = nuovaCarta.GetComponent<CardDisplay3D>().cardData.cardData.nome;
            deckCards.Add(nuovaCarta);
        }
    }

    async Task MoveCards()
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            Vector3 cardPosition = new(-5 + (1.2f * (i % 10)), 0.25f, 4.5f - (1.5f * (i / 10)));
            await CardAnimationsPanchina.MoveAnimation(deckCards[i], cardPosition, 0.1f).ToUniTask(TweenCancelBehaviour.Kill, this.GetCancellationTokenOnDestroy());
            deckCards[i].transform.SetParent(GetSlotByIndex(i));
        }
    }
}