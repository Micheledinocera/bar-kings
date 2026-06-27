using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Ci serve per il metodo Shuffle

public class DeckManager3D : MonoBehaviour
{
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo
    public List<CardData> cardDatabase = new();

    void Start()
    {
        ShuffleDeck();
        RenderDeck();
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
            GameObject nuovaCarta = Instantiate(cardPrefab, deckPosition.position+new Vector3(0,0.02f,0.02f)*i, Quaternion.identity,deckPosition);
            nuovaCarta.GetComponent<CardDisplay3D>().Setup(cardDatabase[i]);
        }
    }
}