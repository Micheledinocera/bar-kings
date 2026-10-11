using System.Collections.Generic;
using UnityEngine;

// The 40 cards of the deck, in table order: a card's position in the list is its place on the table (CardDataInGame.index).
// Created and filled by the editor menu Tools/Genera Carte Napoletane.
[CreateAssetMenu(fileName = "CardDatabase", menuName = "Carte/Card Database")]
public class CardDatabase : ScriptableObject
{
    [SerializeField]
    private List<CardData> cards = new();

    public IReadOnlyList<CardData> Cards => cards;

    public List<CardDataInGame> CreateCardsInGame()
    {
        List<CardDataInGame> cardsInGame = new(cards.Count);
        for (int i = 0; i < cards.Count; i++)
        {
            cardsInGame.Add(new(cards[i], i));
        }
        return cardsInGame;
    }

#if UNITY_EDITOR
    public void SetCards(List<CardData> newCards)
    {
        cards = new(newCards);
    }
#endif
}
