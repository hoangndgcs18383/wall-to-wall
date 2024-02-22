using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager
{
    private static SkinManager _instance;

    public static SkinManager Instance
    {
        get { return _instance ??= new SkinManager(); }
    }

    private Dictionary<string, SkinData> _skinList;


    private event Action<Color> OnSkinColorChanged;
    private event Action<SkinData> OnSkinUnlocked;

    private string _currentSkinKey = PlayerPrefs.GetString("CurrentSkinKey", "Skin_0");

    public void Initialize(Dictionary<string, SkinData> sprites)
    {
        _skinList = sprites;
        PlayerConfig playerConfig = Resources.Load<PlayerConfig>("PlayerConfig");
        for (int i = 0; i < playerConfig.skins.Count; i++)
        {
            string hash = playerConfig.skins[i].hash;
            playerConfig.skins[i].nameDisplay =
                SaveSystem.Instance.GetString(string.Concat(hash, "_", PrefKeys.NameDisplay));
            playerConfig.skins[i].unlockPoint =
                SaveSystem.Instance.GetInt(string.Concat(hash, "_", PrefKeys.UnlockPoint));
        }
    }

    public void SetSkinSprite()
    {
    }

    public bool CheckValidSkinUnlock(int point, int index)
    {
        if (_skinList == null || _skinList.Count == 0) return false;

        if (_skinList.ContainsKey($"Skin_{index}"))
        {
            if (_skinList[$"Skin_{index}"].isUnlock) return false;
            if (_skinList[$"Skin_{index}"].unlockPoint > point || IsSkinUnlocked(index)) return false;

            UnlockSkin(index);
            AddCurrentSkinList(_skinList[$"Skin_{index}"]);
            OnSkinUnlocked?.Invoke(_skinList[$"Skin_{index}"]);
            return true;
        }

        return false;
    }

    private Dictionary<string, SkinData> _currentSkinList = new Dictionary<string, SkinData>();

    public void AddCurrentSkinList(SkinData skinData)
    {
        _currentSkinList.Add(skinData.key, skinData);
    }

    public void ClearCurrentSkinList()
    {
        _currentSkinList.Clear();
    }

    public Dictionary<string, SkinData> GetCurrentSkinList() => _currentSkinList;

    public void UnlockSkin(int index)
    {
        if (_skinList == null || _skinList.Count == 0) return;
        if (index >= _skinList.Count) return;
        if (IsSkinUnlocked(index)) return;

        PlayerPrefs.SetInt($"SkinUnlocked_{index}", 1);
    }

    public bool IsSkinUnlocked(int index)
    {
        return PlayerPrefs.GetInt($"SkinUnlocked_{index}", 0) == 1;
    }

    public Sprite GetCurrentSkinSprite()
    {
        int currentSkinIndex = PlayerPrefs.GetInt("CurrentSkinIndex", 0);
        int lastIndex = PlayerPrefs.GetInt("LastSkinIndex", 0);

        if (_skinList == null || _skinList.Count == 0) return null;
        if (_skinList.ContainsKey($"Skin_{currentSkinIndex}"))
        {
            return IsSkinUnlocked(currentSkinIndex)
                ? _skinList[$"Skin_{currentSkinIndex}"].unlockSprite
                : _skinList[$"Skin_{lastIndex}"].unlockSprite;
        }

        return null;
    }

    public string GetCurrentKey()
    {
        int currentSkinIndex = PlayerPrefs.GetInt("CurrentSkinIndex", 0);
        int lastIndex = PlayerPrefs.GetInt("LastSkinIndex", 0);

        if (_skinList == null || _skinList.Count == 0) return null;
        if (_skinList.ContainsKey($"Skin_{currentSkinIndex}"))
        {
            return IsSkinUnlocked(currentSkinIndex)
                ? _skinList[$"Skin_{currentSkinIndex}"].key
                : _skinList[$"Skin_{lastIndex}"].key;
        }

        return null;
    }

    public void SelectSkin(string key)
    {
        _currentSkinKey = key;
        PlayerPrefs.SetString("CurrentSkinKey", key);
    }

    public SkinData GetCurrentSkin()
    {
        return _skinList[_currentSkinKey];
    }

    public int GetCurrentSkinIndex()
    {
        return int.Parse(_currentSkinKey.Split('_')[1]);
    }

    public void AddListenerSkinColorChanged(Action<Color> action)
    {
        OnSkinColorChanged += action;
    }

    public void RemoveListenerSkinColorChanged(Action<Color> action)
    {
        OnSkinColorChanged -= action;
    }

    public void AddListenerSkinUnlocked(Action<SkinData> action)
    {
        OnSkinUnlocked += action;
    }

    public void RemoveListenerSkinUnlocked(Action<SkinData> action)
    {
        OnSkinUnlocked -= action;
    }
}