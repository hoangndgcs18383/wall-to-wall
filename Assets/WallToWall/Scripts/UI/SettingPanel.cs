using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SettingPanel : BaseScreen
{
    [SerializeField] private bool isHideInAwake;

    [BoxGroup("Settings")] [SerializeField]
    private SettingItem soundItem;

    [BoxGroup("Settings")] [SerializeField]
    private SettingItem musicItem;

    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W btnClose;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W btnFacebook;

    [SerializeField] private TMP_InputField inputFieldDisplayName;

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

    public override void Initialize()
    {
        base.Initialize();

        btnClose.onClick.AddListener(Hide);
        btnFacebook.onClick.AddListener(OnClickFacebook);
        inputFieldDisplayName.onEndEdit.AddListener(OnEndEditDisplayName);
    }


    public override void Show(IUIData data = null)
    {
        base.Show(data);

        inputFieldDisplayName.text = SaveSystem.Instance.GetString(PrefKeys.UserName);

        bool isSfxOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        bool isMsbOn = PlayerPrefs.GetInt("Music", 1) == 1;
        soundItem.Initialize(isSfxOn, OnSound);
        musicItem.Initialize(isMsbOn, OnMusic);
    }

    private void OnEndEditDisplayName(string displayName)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            SaveSystem.Instance.SetString(PrefKeys.UserName, displayName);
        }
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
        Application.OpenURL("https://www.facebook.com/ndhoang.2607/");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}