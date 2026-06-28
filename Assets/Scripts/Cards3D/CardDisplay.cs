using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class CardDisplay3D : MonoBehaviour
{
    public CardData cardData;
    public GameObject fronte;
    public GameObject retro;
    private MeshRenderer meshRenderer;
    public bool isFlipped = false;

    public void Setup(CardData data)
    {
        cardData = data;
        
        meshRenderer = fronte.GetComponent<MeshRenderer>();
        meshRenderer.material.SetTexture("_BaseMap", cardData.immagineFronte.texture);
    }
}