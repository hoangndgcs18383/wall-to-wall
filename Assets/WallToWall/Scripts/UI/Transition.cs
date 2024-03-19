using System;
using UnityEngine;

public enum TransitionUIType
{
    None,
    Fade,
    Scale,
    Move
}

[Serializable]
public struct TransitionData
{
    public TransitionUIType transitionUIType;
    public float duration;
    public float startValue;
    public float endValue;
    public Vector3 startPosition;
    public Vector3 endPosition;
}

[CreateAssetMenu(fileName = "Transition", menuName = "FreakyBall/Transition")]
public class Transition : ScriptableObject
{
    public TransitionData transitionData;
}