using UnityEngine;

public class CardControllerPanchina : MonoBehaviour
{
    async void OnMouseDown()
    {
        CardDisplay3D cardDisplay = GetComponentInParent<CardDisplay3D>();

        if (GameManager.isClickBlocked || cardDisplay.isFlipped) return;
        
        GameManager.isClickBlocked=true;
        
        await CardAnimationsPanchina.MoveCards(cardDisplay.gameObject,DeckManagerPanchina.deckCards[cardDisplay.cardData.index]);
        
        GameManager.isClickBlocked=false;
    }
}