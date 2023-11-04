using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RateStar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private Image fillImage;
    
    private Image _image;
    private (int r, float p) _rate;
    private Action<(int, float)> _callback;
    private Action<(int, float)> _onClick;
    private Rect _rect;

    public void Initialize(Action<(int, float)> callback, (int r, float p) rate, Action<(int, float)> onClick)
    {
        _rate = rate;
        _image = GetComponent<Image>();
        _rect = _image.rectTransform.rect;
        _callback = callback;
        _onClick = onClick;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CalculateRate(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CalculateRate(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        CalculateRate(eventData);
    }

    private void CalculateRate(PointerEventData pointerEventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_image.rectTransform, pointerEventData.position,
            pointerEventData.pressEventCamera, out Vector2 localPoint);
        float x = localPoint.x + _rect.width / 2;
        float p = x / _rect.width;
        p = p switch
        {
            < 0 or < 0.5f => 0.5f,
            > 1 or > 0.5f => 1,
            _ => p
        };
        _rate.p = p;

        _callback?.Invoke((_rate.r, _rate.p));
    }
    
    public void SetFill(float p)
    {
        fillImage.fillAmount = p;
    }
    
    public void OnClick()
    {
        _onClick?.Invoke(_rate);
    }
}