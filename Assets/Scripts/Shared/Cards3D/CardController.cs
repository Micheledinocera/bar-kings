using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardController : MonoBehaviour
{
    void OnMouseDown() => MoveCardsAsync().Forget();

    async UniTaskVoid MoveCardsAsync()
    {
        CardDisplay3D cardDisplay = GetComponentInParent<CardDisplay3D>();

        if (GameManager.isClickBlocked || cardDisplay.isFlipped)
            return;

        GameManager.isClickBlocked = true;
        try
        {
            await CardAnimations.MoveCards(
                cardDisplay.gameObject,
                DeckManager3D.deckCards[cardDisplay.cardData.index],
                destroyCancellationToken
            );
        }
        finally
        {
            GameManager.isClickBlocked = false;
        }
    }
}
