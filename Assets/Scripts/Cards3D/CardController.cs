using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class CardController : MonoBehaviour
{
    // async void OnMouseDown()
    // {
    //     CardDisplay3D padre = GetComponentInParent<CardDisplay3D>();

    //     if (padre != null)
    //     {
    //         await padre.Flip();
    //     }
    // }

    async void OnMouseDown()
    {
        CardDisplay3D cardDisplay = GetComponentInParent<CardDisplay3D>();

        if (GameManager.isClickBlocked || cardDisplay.isFlipped) return;
        
        GameManager.isClickBlocked=true;
        
        await CardAnimations.MoveCards(cardDisplay.gameObject,DeckManager3D.deckCards[cardDisplay.cardData.index]);
        
        GameManager.isClickBlocked=false;
    }
}