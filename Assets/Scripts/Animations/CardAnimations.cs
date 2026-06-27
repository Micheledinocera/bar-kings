using DG.Tweening;
using UnityEngine;

public static class CardAnimations
{
    public static Sequence FlipAnimation(CardDisplay3D card, bool front = true, float duration = 1f)
    {
        Sequence sequence = DOTween.Sequence();

        float alzata = 1f;
        
        sequence.Insert(0,card.transform.DOLocalMoveY(card.transform.position.y+ alzata, duration / 3).SetEase(Ease.OutQuad));
        sequence.Insert(duration/3,card.transform.DORotate(new Vector3(0, 0, front ? 180 : -180), duration, RotateMode.WorldAxisAdd).SetEase(Ease.InOutBack));
        sequence.Append(card.transform.DOLocalMoveY(card.transform.position.y, duration / 3).SetEase(Ease.InQuad));

        sequence.SetLink(card.gameObject);
        return sequence;
    }
}