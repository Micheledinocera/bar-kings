using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TableCardController : MonoBehaviour
{
    void OnMouseDown()
    {
        // OnMouseDown ignores the UI: a click on an overlay (options panel) must not play the card below it.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        PlayPathAsync().Forget();
    }

    async UniTaskVoid PlayPathAsync()
    {
        if (GameManager.isClickBlocked)
            return;

        CardDisplay3D cardDisplay = GetComponentInParent<CardDisplay3D>();
        GameManager.isClickBlocked = true;
        try
        {
            await TableDeckManager.instance.PlayPath(cardDisplay);
        }
        finally
        {
            GameManager.isClickBlocked = false;
        }
    }
}
