using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static class CardAnimations
{
    public static Sequence FlipAnimation(CardDisplay3D card, bool front = true, float duration = 1f)
    {
        Sequence sequence = DOTween.Sequence();

        float alzata = 1f;

        sequence.Insert(0, card.transform.DOMoveY(card.transform.position.y + alzata, duration / 3).SetEase(Ease.OutQuad));
        sequence.Insert(duration / 3, card.transform.DORotate(new Vector3(0, 0, front ? 180 : -180), duration, RotateMode.WorldAxisAdd).SetEase(Ease.InOutBack));
        sequence.Append(card.transform.DOMoveY(card.transform.position.y, duration / 3).SetEase(Ease.InQuad));

        sequence.SetLink(card.gameObject);
        return sequence;
    }
    public static Sequence MoveAnimation(GameObject card, Vector3 targetPosition, float duration = 1f)
    {
        Sequence sequence = DOTween.Sequence();

        float alzata = 1f;

        sequence.Insert(0, card.transform.DOMoveY(card.transform.position.y + alzata, duration / 3).SetEase(Ease.OutQuad));
        sequence.Insert(duration / 3, card.transform.DOMove(targetPosition, duration).SetEase(Ease.InOutBack));

        sequence.SetLink(card);
        return sequence;
    }
    public static async Task MoveCards(GameObject startingCard, GameObject endingCard, float duration = 0.5f)
    {
        float alzata = 1f;
        int firstCardIndex = startingCard.GetComponent<CardDisplay3D>().cardData.index;
        startingCard.GetComponent<CardDisplay3D>().isFlipped = true;
        GameObject tempStartingCard = startingCard;
        Vector3 startingPosition = startingCard.transform.position;
        GameObject tempEndingCard = endingCard;
        int tempEndingCardIndex = tempEndingCard.GetComponent<CardDisplay3D>().cardData.index;
        await startingCard.transform.DOMoveY(startingCard.transform.position.y + alzata, duration / 3).SetEase(Ease.OutQuad).ToUniTask();
        while (tempEndingCardIndex != firstCardIndex)
        {
            await MoveCardsAtom(tempStartingCard, tempEndingCard);
            tempStartingCard = tempEndingCard;
            tempEndingCard = DeckManager3D.deckCards[tempEndingCardIndex];
            tempEndingCardIndex = tempEndingCard.GetComponent<CardDisplay3D>().cardData.index;
        }
        await tempStartingCard.transform.DORotate(new Vector3(0, 0, 180), duration, RotateMode.WorldAxisAdd).SetEase(Ease.InOutBack).ToUniTask();
        await tempStartingCard.transform.DOMove(startingPosition, duration).SetEase(Ease.InOutBack).ToUniTask();
    }

    public static Sequence MoveCardsAtom(GameObject startingCard, GameObject endingCard, float duration = 0.5f)
    {
        float alzata = 1f;
        Sequence sequence = DOTween.Sequence();
        endingCard.GetComponent<CardDisplay3D>().isFlipped = true;
        sequence.Insert(duration / 3, startingCard.transform.DORotate(new Vector3(0, 0, 180), duration, RotateMode.WorldAxisAdd).SetEase(Ease.InOutBack));
        sequence.Append(endingCard.transform.DOMoveY(endingCard.transform.position.y + alzata, duration / 3).SetEase(Ease.InQuad));
        sequence.Append(startingCard.transform.DOMove(endingCard.transform.position, duration).SetEase(Ease.InOutBack));

        return sequence;
    }
}