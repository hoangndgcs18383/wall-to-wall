using System;
using DG.Tweening;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOKill();
        transform.DOScale(3f, 0.5f);
        spriteRenderer.SetAlpha(0.5f);
        spriteRenderer.DOKill();
        spriteRenderer.DOFade(0, 0.5f);
    }
}

public static class Extension
{
    public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
    {
        var color = spriteRenderer.color;
        color = new Color(color.r, color.g, color.b, alpha);
        spriteRenderer.color = color;
    }
}