using DG.Tweening;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public float duration = 0.5f;
    
    private float offsetX = 2f;

    private void OnEnable()
    {
        transform.DOLocalMoveX(offsetX, duration).SetEase(Ease.Linear);
    }
    
    public void TurnOff()
    {
        transform.DOLocalMoveX(-offsetX, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}