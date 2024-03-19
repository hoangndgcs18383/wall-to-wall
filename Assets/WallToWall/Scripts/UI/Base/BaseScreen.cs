using UnityEngine;

public delegate void OnHideScreen();

public class BaseScreen : MonoBehaviour, IBaseScreen
{
    private bool isInitialized = false;

    private RectTransform _rectTransform;

    public OnHideScreen OnHideScreen;

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    public virtual void Initialize()
    {
        if (isInitialized) return;
        isInitialized = true;
    }

    public virtual void Show(IUIData data = null)
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnHideScreen?.Invoke();
        OnHideScreen = null;
    }

    public virtual void BackToScreen()
    {
        Hide();
    }
}