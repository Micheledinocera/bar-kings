using System;
using UnityEngine;

// Durations of the card animations for one AnimationSpeed level, in seconds.
[Serializable]
public class AnimationTiming
{
    [Range(0.01f, 1f)]
    public float dealMoveDuration; // per card when dealing

    [Range(0.01f, 2f)]
    public float pathMoveDuration; // per flip / move in a path

    public AnimationTiming(float deal, float path)
    {
        dealMoveDuration = deal;
        pathMoveDuration = path;
    }
}
