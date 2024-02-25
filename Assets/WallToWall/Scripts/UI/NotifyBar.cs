using DG.Tweening;
using TMPro;
using UnityEngine;

public class NotifyBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text txtMessage;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Show(string message, float height = 0, float duration = 0.5f)
    {
        _rectTransform.anchoredPosition = new Vector2(0, height == 0 ? -_rectTransform.rect.height : height);
        _rectTransform.DOAnchorPosY(0, duration).SetEase(Ease.OutBack);

        canvasGroup.DOFade(1f, duration).OnComplete(() =>
        {
            _rectTransform.DOAnchorPosY(-_rectTransform.rect.height, duration).SetDelay(2f).SetEase(Ease.OutBounce)
                .OnComplete(
                    () => { canvasGroup.DOFade(0f, duration).OnComplete(() => { }); });
        });
        txtMessage.SetText(message);
    }
}