using DG.Tweening;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public float duration = 0.5f;
    
    private float offsetX = 2f;

    private void OnEnable()
    {
        transform.localPosition = new Vector3(-offsetX, transform.localPosition.y, 0);
        transform.DOLocalMoveX(offsetX, duration).SetEase(Ease.Linear);
    }
    
    public void TurnOff()
    {
        transform.DOLocalMoveX(-offsetX, duration / 2).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        });
    }
}