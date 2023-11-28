using DG.Tweening;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public float duration = 0.5f;
    
    private float offsetX = 1f;

    private void OnEnable()
    {
        transform.localPosition = new Vector3(-offsetX, transform.localPosition.y, transform.localPosition.z);
        transform.DOLocalMoveX(offsetX, duration).SetEase(Ease.Linear);
    }
    
    public void TurnOff()
    {
        transform.DOLocalMoveX(-offsetX, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}