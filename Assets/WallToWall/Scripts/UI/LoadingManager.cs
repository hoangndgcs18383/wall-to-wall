using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum TransitionType
{
    Pinch,
    Fade
}

public class LoadingManager : MonoBehaviour
{
    [TabGroup("Transition Materials")] [SerializeField]
    private Material backgroundMaterial;

    [TabGroup("Transition")] [SerializeField]
    private float transitionDuration = 1f;

    [TabGroup("Transition")] [SerializeField]
    private float startValue = 0f;

    [TabGroup("Transition")] [SerializeField]
    private float endValue = 1f;

    public static LoadingManager Instance;

    //private static readonly int PinchUvAmount = Shader.PropertyToID("_PinchUvAmount");
    //private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");

    private void Awake()
    {
        Instance = this;
    }
    
    public void Transition(TransitionType transitionType, Image image, Action onStart = null, Action onComplete = null)
    {
        switch (transitionType)
        {
            case TransitionType.Fade:
                FadeTransition(image, onStart, onComplete);
                break;
            case TransitionType.Pinch:
                FadeTransition(image, onStart, onComplete);
                break;
        }
    }
    
    private void FadeTransition(Image image, Action onStart, Action onComplete)
    {
        onStart?.Invoke();
        image.material = new Material(backgroundMaterial);
        image.material.EnableKeyword("FADE_ON");
        image.material.SetFloat("_FadeAmount", startValue);
        DOVirtual.Float(startValue, endValue, transitionDuration,
            value => { image.material.SetFloat("_FadeAmount", value); }).OnComplete(() =>
        {
            image.material.DisableKeyword("FADE_ON");
            onComplete?.Invoke();
        });
    }

    private void RestTransition()
    {
        backgroundMaterial.SetFloat("_FadeAmount", startValue);
        backgroundMaterial.DisableKeyword("FADE_ON");
    }
}