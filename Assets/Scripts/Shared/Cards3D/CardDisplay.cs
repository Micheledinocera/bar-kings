using UnityEngine;

public class CardDisplay3D : MonoBehaviour
{
    static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");

    public CardDataInGame cardData;
    public GameObject fronte;
    public GameObject retro;
    private MeshRenderer meshRenderer;
    public bool isFlipped = false;

    public void Setup(CardDataInGame data)
    {
        cardData = data;

        // A property block instead of renderer.material: no material instance per card to destroy.
        meshRenderer = fronte.GetComponent<MeshRenderer>();
        MaterialPropertyBlock properties = new();
        meshRenderer.GetPropertyBlock(properties);
        properties.SetTexture(BaseMapId, cardData.cardData.immagineFronte.texture);
        meshRenderer.SetPropertyBlock(properties);
    }
}
