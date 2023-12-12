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

    private IEnumerator<float> ShowAndLoadScreen<T>(string screenName, CanvasType parent, IUIData data = null,
        Action<T> callback = null)
        where T : class, IBaseScreen
    {
        BaseScreen baseScreen = null;
        if (!screens.ContainsKey(screenName))
        {
            var screen = screenReferences.Find(x => x.key == screenName);
            if (screen.screen == null)
            {
                Debug.LogError($"Screen {screenName} not found");
                yield break;
            }

            baseScreen = Instantiate(screen.screen, GetCanvas(parent), true);
            baseScreen.RectTransform.anchoredPosition = Vector2.zero;
            baseScreen.RectTransform.sizeDelta = Vector2.zero;
            baseScreen.RectTransform.localScale = Vector3.one;
            baseScreen.RectTransform.SetAsLastSibling();
            yield return Timing.WaitForOneFrame;
            screens.Add(screenName, baseScreen);
            screens[screenName].Initialize();
        }
        else
        {
            baseScreen = (BaseScreen)screens[screenName];
        }

        yield return Timing.WaitForOneFrame;
        baseScreen.Show(data);

        yield return Timing.WaitForOneFrame;
        callback?.Invoke(baseScreen as T);
    }

    public T GetScreen<T>() where T : IBaseScreen
    {
        if (screens.ContainsKey(typeof(T).Name))
        {
            return (T)screens[typeof(T).Name];
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

    public void ShowInventoryScreen(Action onClose = null)
    {
        Timing.RunCoroutine(ShowAndLoadScreen<InventoryPanel>("InventoryPanel", CanvasType.Main, null,
            panel => { panel.RegisterOnClose(onClose); }));
    }

    public void ShowUnlockSkinScreen(Dictionary<string, SkinData> skinData)
    {
        Timing.RunCoroutine(ShowAndLoadScreen<UnlockSkinPanel>("UnlockSkinPanel", CanvasType.Main, null,
            screen => { screen.SetData(skinData); }));
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

    public void ShowTutorialScreen(Action onComplete = null)
    {
        Timing.RunCoroutine(ShowAndLoadScreen<TutorialPanel>("TutorialPanel", CanvasType.Main, null,
            screen => { screen.RegisterCompleteCallback(onComplete); }));
    }

    [Button]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}