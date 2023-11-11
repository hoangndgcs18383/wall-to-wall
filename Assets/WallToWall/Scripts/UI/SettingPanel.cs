using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private bool isHideInAwake;

    [BoxGroup("Settings")] [SerializeField]
    private SettingItem soundItem;

    [BoxGroup("Settings")] [SerializeField]
    private SettingItem musicItem;

    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W btnClose;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W btnFacebook;

    private void Awake()
    {
        bool isSfxOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        bool isMsbOn = PlayerPrefs.GetInt("Music", 1) == 1;
        soundItem.Initialize(isSfxOn, OnSound);
        musicItem.Initialize(isMsbOn, OnMusic);
        if (isHideInAwake && gameObject.activeSelf)
        {
            Hide();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        bool isSfxOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        bool isMsbOn = PlayerPrefs.GetInt("Music", 1) == 1;
        soundItem.Initialize(isSfxOn, OnSound);
        musicItem.Initialize(isMsbOn, OnMusic);

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
        PlayerPrefs.SetInt("Music", obj ? 1 : 0);
        AudioManager.Instance.SetOnOffBGM(obj);
    }

    private void OnSound(bool obj)
    {
        PlayerPrefs.SetInt("Sound", obj ? 1 : 0);
        AudioManager.Instance.SetOnOffSfx(obj);
    }

    private void OnClickFacebook()
    {
        Debug.Log("Facebook");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}