using UnityEngine;

public class CardController : MonoBehaviour
{
    

    void Awake()
    {
        // Otteniamo il riferimento al componente che disegna la grafica
    }

    async void OnMouseDown()
    {
        CardDisplay3D padre = GetComponentInParent<CardDisplay3D>();
        
        if (padre != null)
        {
            await padre.Flip();
        }
    }
}