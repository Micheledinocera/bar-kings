using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class CardDisplay3D : MonoBehaviour
{
    public CardDataInGame cardData;
    public GameObject fronte;
    public GameObject retro;
    private MeshRenderer meshRenderer;
    public bool isFlipped = false;

    public void Setup(CardDataInGame data)
    {
        cardData = data;
        
        meshRenderer = fronte.GetComponent<MeshRenderer>();
        meshRenderer.material.SetTexture("_BaseMap", cardData.cardData.immagineFronte.texture);
    }
}