using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonW2W : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public bool isInteractable = true;
    public Image targetGraphic;
    public UnityEvent onClick = new UnityEvent();

    private bool _isInit = false;
    private RectTransform _rectTransform;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void OnValidate()
    {
        targetGraphic ??= GetComponent<Image>();
    }

    private void OnDisable()
    {
        _rectTransform.DOKill();
        _rectTransform.localScale = Vector3.one;
    }

    private void Initialize()
    {
        if(_isInit) return;
        _isInit = true;
        
        _rectTransform = GetComponent<RectTransform>();
        targetGraphic ??= GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _rectTransform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
        onClick?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rectTransform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack);
    }
}
