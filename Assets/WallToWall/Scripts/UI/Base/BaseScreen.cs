using UnityEngine;

public class BaseScreen : MonoBehaviour, IBaseScreen
{
    private bool isInitialized = false;

    private RectTransform _rectTransform;

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
    }

    public virtual void BackToScreen()
    {
        Hide();
    }
}