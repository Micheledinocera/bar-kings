using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pulse : MonoBehaviour
{
    public float scaleQuantity=0.5f;
    public float scaleTime=1;
    private Sequence sequence;
    void Start()
    {
        sequence = DOTween.Sequence()
           .Append( transform.DOBlendableScaleBy(new Vector3(1,1,1)*scaleQuantity,scaleTime));
        sequence.SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy() {
        sequence.Kill();
    }
}
