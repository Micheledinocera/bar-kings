using UnityEngine;
using UnityEngine.UI; // Se usi UI

public class CardDisplay3D : MonoBehaviour
{
    public CardData cardData;
    public GameObject fronte;
    public GameObject retro;
    private MeshRenderer meshRenderer;
    private bool isFlipped = false;

    void Awake()
    {
        // Otteniamo il riferimento al componente che disegna la grafica
        meshRenderer = fronte.GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture=cardData.immagineFronte.texture;
    }

    public void Setup(CardData data)
    {
        cardData = data;
        // Qui potresti impostare il materiale del retro
    }

    public void Flip()
    {
        if (isFlipped) return;

        transform.Rotate(0, 0, 180);
        
        isFlipped = true;
    }
}