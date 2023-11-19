using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct ScreenReference
{
    public string key;
    public BaseScreen screen;
}

public enum CanvasType
{
    Main,
    InGame
}

public class UIManager : Singleton<UIManager>
{
    [TableList] [SerializeField] private List<ScreenReference> screenReferences = new List<ScreenReference>();

    [TabGroup("Canvas")] [SerializeField] private RectTransform mainCanvas;
    [TabGroup("Canvas")] [SerializeField] private RectTransform inGameCanvas;

    private Dictionary<string, IBaseScreen> screens;
    
    [SerializeField] private Sprite testSprite;

    private void Awake()
    {
        screens = new Dictionary<string, IBaseScreen>();
    }

    private RectTransform GetCanvas(CanvasType canvasType)
    {
        return canvasType switch
        {
            CanvasType.Main => mainCanvas,
            CanvasType.InGame => inGameCanvas,
            _ => null
        };
    }

    private IEnumerator<float> ShowAndLoadScreen<T>(string screenName, CanvasType parent, IUIData data = null)
        where T : IBaseScreen
    {
        if (!screens.ContainsKey(screenName))
        {
            var screen = screenReferences.Find(x => x.key == screenName);
            if (screen.screen == null)
            {
                Debug.LogError($"Screen {screenName} not found");
                yield break;
            }

            BaseScreen baseScreen = Instantiate(screen.screen, GetCanvas(parent), true);
            baseScreen.RectTransform.anchoredPosition = Vector2.zero;
            baseScreen.RectTransform.sizeDelta = Vector2.zero;
            baseScreen.RectTransform.localScale = Vector3.one;

            yield return Timing.WaitForOneFrame;
            screens.Add(screenName, baseScreen);
            screens[screenName].Initialize();
        }

        yield return Timing.WaitForOneFrame;
        screens[screenName].Show(data);

        yield return Timing.WaitForOneFrame;
    }
    
    public T GetScreen<T>() where T : IBaseScreen
    {
        if (screens.ContainsKey(typeof(T).Name))
        {
            return (T) screens[typeof(T).Name];
        }

        return default;
    }

    public void ShowPauseScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreen<PausePanel>("PausePanel", CanvasType.InGame));
    }

    public void ShowInGameScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreen<InGamePanel>("InGamePanel", CanvasType.InGame));
    }

    public void ShowRateScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreen<RatePanel>("RatePanel", CanvasType.Main));
    }

    public void ShowSettingScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreen<SettingPanel>("SettingPanel", CanvasType.Main));
    }
    
    [Button]
    public void ShowTestUnlockSkinScreen()
    {
        ShowUnlockSkinScreen(new SkinData
        {
            sprite = testSprite
        });
    }

    public void ShowUnlockSkinScreen(SkinData skinData)
    {
        Timing.RunCoroutine(ShowAndLoadScreen<UnlockSkinPanel>("UnlockSkinPanel", CanvasType.Main, skinData));
    }

    public void ShowMainMenuScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreen<MainMenu>("MainMenu", CanvasType.Main));
    }

    public void ShowGameOverScreen(TotalScoreUIData data)
    {
        Timing.RunCoroutine(ShowAndLoadScreen<GameOverPanel>("GameOverPanel", CanvasType.InGame, data));
    }
    
    public void ShowRankScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreen<RankPanel>("RankPanel", CanvasType.Main));
    }
}