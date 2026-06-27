using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class DeckManager3D : MonoBehaviour
{
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo
    public List<CardData> cardDatabase = new();
    public static List<GameObject> deckCards = new();

    async void Start()
    {
        GameManager.isClickBlocked=true;
        ShuffleDeck();
        RenderDeck();
        await MoveCards();
        GameManager.isClickBlocked=false;
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < cardDatabase.Count; i++)
        {
            CardData temp = cardDatabase[i];
            int randomIndex = Random.Range(i, cardDatabase.Count);
            cardDatabase[i] = cardDatabase[randomIndex];
            cardDatabase[randomIndex] = temp;
        }
        
        Debug.Log("Mazzo mescolato!");
    }

    void RenderDeck()
    {
        for (int i = cardDatabase.Count-1; i >=0 ; i--)
        {
            GameObject nuovaCarta = Instantiate(cardPrefab, deckPosition.position + new Vector3(0, 0.02f, 0.02f) * i, Quaternion.identity, deckPosition);
            nuovaCarta.GetComponent<CardDisplay3D>().Setup(cardDatabase[i]);
            deckCards.Add(nuovaCarta);
        }
    }

    async Task MoveCards()
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            // deckCards[i].transform.position=new Vector3(-7f+(1.2f*(i% 10)),0.25f,4.5f-(1.5f*(i/ 10)));
            Vector3 cardPosition=new(-5+(1.2f*(i% 10)),0.25f,4.5f-(1.5f*(i/ 10)));
            await CardAnimations.MoveAnimation(deckCards[i],cardPosition,0.1f).ToUniTask(TweenCancelBehaviour.Kill, this.GetCancellationTokenOnDestroy());
        }
    }
}