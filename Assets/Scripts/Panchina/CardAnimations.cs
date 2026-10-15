using DG.Tweening;
using UnityEngine;

public static class CardAnimationsPanchina
{
    public static Sequence MoveAnimation(
        GameObject card,
        Vector3 targetPosition,
        float duration = 1f
    )
    {
        Sequence sequence = DOTween.Sequence();

        float alzata = 1f;

        sequence.Insert(
            0,
            card.transform.DOMoveY(card.transform.position.y + alzata, duration / 3)
                .SetEase(Ease.OutQuad)
        );
        sequence.Insert(
            duration / 3,
            card.transform.DOMove(targetPosition, duration).SetEase(Ease.InOutBack)
        );

        sequence.SetLink(card);
        return sequence;
    }

    // One move of a path, as in the first POC: the card (lifted) flips face up, the card it displaces is lifted,
    // then the card moves onto the target. The first move also lifts the clicked card.
    public static Sequence PathMoveAnimation(
        CardDisplay3D card,
        CardDisplay3D displaced,
        Vector3 targetPosition,
        bool liftCard,
        float duration = 0.5f
    )
    {
        const float lift = 1f;
        Sequence sequence = DOTween.Sequence();

        if (liftCard)
        {
            sequence.Append(
                card.transform.DOMoveY(card.transform.position.y + lift, duration / 3)
                    .SetEase(Ease.OutQuad)
            );
        }
        sequence.AppendInterval(duration / 3);
        sequence.Append(
            card.transform.DORotate(new Vector3(0, 0, 180), duration, RotateMode.WorldAxisAdd)
                .SetEase(Ease.InOutBack)
        );
        if (displaced != null)
        {
            sequence.Append(
                displaced
                    .transform.DOMoveY(displaced.transform.position.y + lift, duration / 3)
                    .SetEase(Ease.InQuad)
            );
        }
        sequence.Append(card.transform.DOMove(targetPosition, duration).SetEase(Ease.InOutBack));

        sequence.SetLink(card.gameObject);
        return sequence;
    }
}
