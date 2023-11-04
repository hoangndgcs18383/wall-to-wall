using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private bool isHideInAwake;
    
    [BoxGroup("Settings")] [SerializeField] private SettingItem soundItem;
    [BoxGroup("Settings")] [SerializeField] private SettingItem musicItem;

    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W btnClose;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W btnFacebook;

    private void Awake()
    {
        if (isHideInAwake && gameObject.activeSelf)
        {
            Hide();
        }
    
    }

    public void Show()
    {
        gameObject.SetActive(true);
        soundItem.Initialize(true, OnSound);
        musicItem.Initialize(true, OnMusic);
        
        btnClose.onClick.AddListener(Hide);
        btnFacebook.onClick.AddListener(OnClickFacebook);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
        btnClose.onClick.RemoveListener(Hide);
        btnFacebook.onClick.RemoveListener(OnClickFacebook);
    }
    
    private void OnMusic(bool obj)
    {
    }

    private void OnSound(bool obj)
    {
    }
    
    private void OnClickFacebook()
    {
        Debug.Log("Facebook");
    }
}