using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Ci serve per il metodo Shuffle

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab; // Trascina qui il Prefab "Carta"
    public Transform deckPosition; // Posizione dove creare il mazzo
    public Transform spawnPoint; // Punto dove apparirà la carta pescata
    private Stack<CardData> mazzo = new Stack<CardData>();
    // Lista che conterrà i 40 asset
    public List<CardData> cardDatabase = new List<CardData>();

    void Start()
    {
        ShuffleDeck();
        foreach (var carta in cardDatabase)
        {
            mazzo.Push(carta);
        }
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
    public void PescaCarta()
    {
        if (mazzo.Count > 0)
        {
            CardData cartaEstratta = mazzo.Pop();

            // Istanziamo il Prefab nella scena
            GameObject nuovaCarta = Instantiate(cardPrefab, spawnPoint.position, Quaternion.identity);

            // Passiamo i dati alla carta
            nuovaCarta.GetComponent<CardDisplay>().Setup(cartaEstratta);

            Debug.Log("Hai pescato: " + cartaEstratta.nome);
        }
        else
        {
            Debug.Log("Il mazzo è vuoto!");
        }
    }
}