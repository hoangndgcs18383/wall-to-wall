using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager
{
    private static SkinManager _instance;
    
    public static SkinManager Instance
    {
        get { return _instance ??= new SkinManager(); }
    }
    
    private Dictionary<int, Color> _skinColorList = new Dictionary<int, Color>();
    private event Action<Color> OnSkinColorChanged;  

    public void Initialize()
    {
        _skinColorList = new Dictionary<int, Color>
        {
            {5, new Color(0.8f, 0.8f, 0.8f)},
            {10, new Color(0.6f, 0.8f, 0.8f)},
            {15, new Color(0.4f, 0.4f, 0.8f)},
            {20, new Color(0.3f, 0.1f, 0.8f)},
            {25, new Color(1f, 0.2f, 0.8f)},
        };
    }
    
    public void SetSkinColor(int score)
    {
        foreach (var skinColor in _skinColorList)
        {
            if (score >= skinColor.Key)
            {
                OnSkinColorChanged?.Invoke(skinColor.Value);
            }
        }
    }
    
    public void AddListenerSkinColorChanged(Action<Color> action)
    {
        OnSkinColorChanged += action;
    }
    
    public void RemoveListenerSkinColorChanged(Action<Color> action)
    {
        OnSkinColorChanged -= action;
    }
}
