using UnityEngine;

public class CardControllerTest : MonoBehaviour
{
    async void OnMouseDown()
    {
        CardDisplay3D cardDisplay = GetComponentInParent<CardDisplay3D>();

        if (GameManager.isClickBlocked || cardDisplay.isFlipped) return;
        
        GameManager.isClickBlocked=true;
        
        await CardAnimationsTest.MoveCards(cardDisplay.gameObject,DeckManager3D.deckCards[cardDisplay.cardData.index]);
        
        GameManager.isClickBlocked=false;
    }
}