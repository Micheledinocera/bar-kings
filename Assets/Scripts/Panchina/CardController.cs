using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardControllerPanchina : MonoBehaviour
{
    void OnMouseDown() => PlayPathAsync().Forget();

    async UniTaskVoid PlayPathAsync()
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
