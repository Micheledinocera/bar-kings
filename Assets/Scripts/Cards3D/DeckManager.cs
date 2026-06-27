using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Ci serve per il metodo Shuffle

public class DeckManager3D : MonoBehaviour
{
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo
    public Transform cardsContainer;
    public List<CardData> cardDatabase = new();
    private List<GameObject> deckCards = new();

    void Start()
    {
        ShuffleDeck();
        RenderDeck();
        moveCards();
    }

    void ShuffleDeck()
    {
        // Algoritmo di Fisher-Yates per mescolare la lista
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
        for (int i = 0; i < cardDatabase.Count; i++)
        {
            GameObject nuovaCarta = Instantiate(cardPrefab, deckPosition.position + new Vector3(0, 0.02f, 0.02f) * i, Quaternion.identity, deckPosition);
            nuovaCarta.GetComponent<CardDisplay3D>().Setup(cardDatabase[i]);
            deckCards.Add(nuovaCarta);
        }
    }

    void moveCards()
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            deckCards[i].transform.SetParent(cardsContainer.transform,false);
            deckCards[i].transform.position=new Vector3(-7f+(1.2f*(i% 10)),0.25f,4.5f-(1.5f*(i/ 10)));
        }
    }
}