using DG.Tweening;
using UnityEngine;

public static class StaticAnimations
{
    // public static Sequence FlipAnimation(CardDisplay3D Card,float duration=1f)
    // {
    //     Sequence animationSequence=DOTween.Sequence();

    //     animationSequence.Append(Card.transform.DORotate(new Vector3(0, 0, 180),duration));
    //     // animationSequence.Append(Card.transform.DORotateQuaternion(new Quaternion(0,0,0,1),duration));

    //     animationSequence.SetLink(Card.gameObject);
    //     return animationSequence;
    // }
    public static Tween FlipAnimation(CardDisplay3D card, bool front=true ,float duration = 1f)
    {
        // Usiamo un Tween diretto invece di una Sequence se l'animazione è semplice
        // .SetEase(Ease.InOutBack) rende il movimento naturale, molto più "pro"
        return card.transform.DORotate(new Vector3(0, 0, front?180:-180), duration, RotateMode.WorldAxisAdd)
            .SetEase(Ease.InOutBack)
            .SetLink(card.gameObject);
    }
}