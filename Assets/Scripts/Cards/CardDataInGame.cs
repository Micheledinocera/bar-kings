using UnityEngine;

public class CardDataInGame : ScriptableObject
{
    public CardData cardData;
    public int index;
    public CardDataInGame(CardData cardData, int index)
    {
        this.cardData = cardData;
        this.index = index;
    }
}