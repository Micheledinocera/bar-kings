using UnityEngine;

public class CardControllerPanchina : MonoBehaviour
{
    async void OnMouseDown()
    {
        if (GameManager.isClickBlocked)
            return;

        CardDisplay3D cardDisplay = GetComponentInParent<CardDisplay3D>();
        GameManager.isClickBlocked = true;
        try
        {
            await DeckManagerPanchina.instance.PlayPath(cardDisplay);
        }
        finally
        {
            GameManager.isClickBlocked = false;
        }
    }
}
