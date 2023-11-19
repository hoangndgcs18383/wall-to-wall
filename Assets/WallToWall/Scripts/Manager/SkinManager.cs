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

    public void Initialize(Dictionary<string, SkinData> sprites)
    {
        _skinList = sprites;
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
            OnSkinUnlocked?.Invoke(_skinList[$"Skin_{index}"]);
            return true;
        }

        return false;
    }

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