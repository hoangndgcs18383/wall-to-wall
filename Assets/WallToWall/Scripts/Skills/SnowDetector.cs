using System;
using DG.Tweening;
using UnityEngine;

public class SnowDetector : MonoBehaviour
{
    public void StartAnimation()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(15, 1f).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(LayerKeys.TRIANGLE))
        {
            Triangle triangle = col.GetComponentInParent<Triangle>();
            
            if (triangle != null)
            {
                triangle.SnowEffect();
            }
        }
    }
}