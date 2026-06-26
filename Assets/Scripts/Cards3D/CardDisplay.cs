using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class CardDisplay3D : MonoBehaviour
{
    public CardData cardData;
    public GameObject fronte;
    public GameObject retro;
    private MeshRenderer meshRenderer;
    private bool isFlipped = false;
    private bool isAnimating = false;

    void Awake()
    {
        // Otteniamo il riferimento al componente che disegna la grafica
        meshRenderer = fronte.GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = cardData.immagineFronte.texture;
    }

    public void Setup(CardData data)
    {
        cardData = data;
        // Qui potresti impostare il materiale del retro
    }

    public async Task Flip()
    {
        if (isAnimating) return;
        // if (isFlipped) return;
        isAnimating = true;
        isFlipped = !isFlipped;
        await StaticAnimations.FlipAnimation(this, isFlipped).ToUniTask(TweenCancelBehaviour.Kill, this.GetCancellationTokenOnDestroy());
        isAnimating = false;

    }
}