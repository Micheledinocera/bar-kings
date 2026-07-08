using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class DeckManager3D : MonoBehaviour
{
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo
    public static List<CardData> cardDatabase = new();
    public static List<CardDataInGame> cardDatabaseInGame = new();
    public static List<CardDataInGame> shuffledCardDatabaseInGame = new();
    public static List<GameObject> deckCards = new();

    async void Start()
    {
        GameManager.isClickBlocked=true;
        CardGenerator.GenerateCards();
        ShuffleDeck();
        RenderDeck();
        await MoveCards();
        GameManager.isClickBlocked=false;
    }

    void ShuffleDeck()
    {
        shuffledCardDatabaseInGame=cardDatabaseInGame;
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
        for (int i = shuffledCardDatabaseInGame.Count-1; i >=0 ; i--)
        {
            GameObject nuovaCarta = Instantiate(cardPrefab, deckPosition.position + new Vector3(0, 0.02f, 0.02f) * i, Quaternion.identity, deckPosition);
            nuovaCarta.GetComponent<CardDisplay3D>().Setup(shuffledCardDatabaseInGame[i]);
            deckCards.Add(nuovaCarta);
        }
    }

    async Task MoveCards()
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            Vector3 cardPosition=new(-5+(1.2f*(i% 10)),0.25f,4.5f-(1.5f*(i/ 10)));
            await CardAnimations.MoveAnimation(deckCards[i],cardPosition,0.1f).ToUniTask(TweenCancelBehaviour.Kill, this.GetCancellationTokenOnDestroy());
        }
    }
}