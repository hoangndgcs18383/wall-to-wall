using System;
using UnityEngine;


public class SettingItem : MonoBehaviour
{
    [SerializeField] private ButtonW2W btnOn;
    [SerializeField] private ButtonW2W btnOff;
    
    [SerializeField] private Sprite onIcon;
    [SerializeField] private Sprite offIcon;
    
    private bool _isOn;
    private Action<bool> _callback;

    private void OnEnable()
    {
        btnOn.onClick.AddListener(OnClickOn);
        btnOff.onClick.AddListener(OnClickOff);
    }

    private void OnDisable()
    {
        btnOn.onClick.RemoveListener(OnClickOn);
        btnOff.onClick.RemoveListener(OnClickOff);
    }

    public void Initialize(bool isOn, Action<bool> callback)
    {
        _isOn = isOn;
        _callback = callback;
        SetButtonInteractable(isOn);
    }

    public void OnClickOn()
    {
        SetButtonInteractable(true);
    }
    
    public void OnClickOff()
    {
        SetButtonInteractable(false);
    }
    
    private void SetButtonInteractable(bool isOn)
    {
        _isOn = isOn;
        btnOn.isInteractable = !isOn;
        btnOff.isInteractable = isOn;
        btnOn.targetGraphic.sprite = isOn ? onIcon : offIcon;
        btnOff.targetGraphic.sprite = isOn ? offIcon : onIcon;
        _callback?.Invoke(isOn);
    }
}
