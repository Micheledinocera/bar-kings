using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class CardDisplay3D : MonoBehaviour
{
    public CardData cardData;
    public GameObject fronte;
    public GameObject retro;
    private MeshRenderer meshRenderer;
    private bool isFlipped = false;
    private bool isAnimating = false;

    public void Setup(CardData data)
    {
        cardData = data;
        
        meshRenderer = fronte.GetComponent<MeshRenderer>();
        meshRenderer.material.SetTexture("_BaseMap", cardData.immagineFronte.texture);
    }

    public async Task Flip()
    {
        if (isAnimating) return;
        // if (isFlipped) return;
        isAnimating = true;
        isFlipped = !isFlipped;
        await CardAnimations.FlipAnimation(this, isFlipped).ToUniTask(TweenCancelBehaviour.Kill, this.GetCancellationTokenOnDestroy());
        isAnimating = false;

    }
}