using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MEC;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;

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

    [Button]
    public void BuildScreenAddress()
    {
#if UNITY_EDITOR
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("// This file is auto-generated. DO NOT EDIT");
        sb.AppendLine("public static class ScreenAddress");
        sb.AppendLine("{");
        sb.AppendLine("\tpublic static string GetAddress(string key)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tswitch (key)");
        sb.AppendLine("\t\t{");

        foreach (var screen in screenReferences)
        {
            if (screen.screen != null)
            {
                AssignAddress(screen.screen, "Screens", screen.key, "Screens");
                sb.AppendLine($"\t\t\tcase \"{screen.key}\":");
                sb.AppendLine($"\t\t\t\treturn \"{GetGuid(screen.screen)}\";");
            }
        }

        sb.AppendLine("\t\t}");
        sb.AppendLine("\t\treturn null;");
        sb.AppendLine("\t}");

        foreach (var screen in screenReferences)
        {
            if (screen.screen != null)
            {
                sb.AppendLine($"\tpublic const string {screen.key} = \"{screen.key}\";");
            }
        }

        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(Application.dataPath, "WallToWall/Scripts/Generated/ScreenAddress.cs"),
            sb.ToString());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    private void AssignAddress<T>(T data, string groupName, string address, string label) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
            group = settings.CreateGroup(groupName, false, false, true, new List<AddressableAssetGroupSchema>());
        AddressableAssetEntry entry =
            settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data)), group);
        entry.address = address;
        entry.labels.Add(label);
        //create schema content upload restriction
        AddressableAssetGroupSchema schema = group.GetSchema<AddressableAssetGroupSchema>();
        if (schema == null)
            schema = group.AddSchema<AddressableAssetGroupSchema>();
#endif
    }

#if UNITY_EDITOR
    private string GetGuid<T>(T data) where T : UnityEngine.Object
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetEntry entry =
            settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data)));
        return entry.guid;
    }
#endif

    private IEnumerator<float> ShowAndLoadScreenAsync<T>(string screenName, CanvasType parent, IUIData data = null,
        OnHideScreen onHide = null,
        Action<T> callback = null)
        where T : class, IBaseScreen
    {
        BaseScreen baseScreen = null;
        if (!screens.ContainsKey(screenName))
        {
            /*var screen = screenReferences.Find(x => x.key == screenName);
            if (screen.screen == null)
            {
                Debug.LogError($"Screen {screenName} not found");
                yield break;
            }*/

            //baseScreen = Instantiate(screen.screen, GetCanvas(parent), true);
            AddressablesManager.TryInstantiateSync(ScreenAddress.GetAddress(screenName), out GameObject go,
                GetCanvas(parent));
            baseScreen = go.GetComponent<BaseScreen>();
            baseScreen.RectTransform.anchoredPosition = Vector2.zero;
            baseScreen.RectTransform.sizeDelta = Vector2.zero;
            baseScreen.RectTransform.localScale = Vector3.one;
            baseScreen.RectTransform.SetAsLastSibling();
            //yield return Timing.WaitForOneFrame;
            screens.Add(screenName, baseScreen);
            screens[screenName].Initialize();
        }
        else
        {
            baseScreen = (BaseScreen)screens[screenName];
            baseScreen.OnHideScreen = onHide;
        }

        //yield return Timing.WaitForOneFrame;
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
        Timing.RunCoroutine(ShowAndLoadScreenAsync<PausePanel>("PausePanel", CanvasType.InGame));
    }

    public void ShowInGameScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<InGamePanel>("InGamePanel", CanvasType.InGame));
    }

    public void ShowRateScreen(OnHideScreen onHideScreen = null)
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<RatePanel>("RatePanel", CanvasType.Main, null, onHideScreen));
    }

    public void ShowSettingScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<SettingPanel>("SettingPanel", CanvasType.Main));
    }

    public void ShowInventoryScreen(Action onClose = null)
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<InventoryPanel>("InventoryPanel", CanvasType.Main, null,
            callback: panel => { panel.RegisterOnClose(onClose); }));
    }

    public void ShowUnlockSkinScreen(Dictionary<string, SkinData> skinData, Action onClose = null)
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<UnlockSkinPanel>("UnlockSkinPanel", CanvasType.Main, null,
            callback: screen =>
            {
                screen.SetData(skinData);
                screen.RegisterOnClose(onClose);
            }));
    }

    public void ShowMainMenuScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<MainMenu>("MainMenu", CanvasType.Main));
    }

    public void ShowGameOverScreen(TotalScoreUIData data)
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<GameOverPanel>("GameOverPanel", CanvasType.InGame, data));
    }

    public void ShowRankScreen()
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<RankPanel>("RankPanel", CanvasType.Main));
    }

    public void ShowTutorialScreen(Action onComplete = null)
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<TutorialPanel>("TutorialPanel", CanvasType.Main, null,
            callback: screen => { screen.RegisterCompleteCallback(onComplete); }));
    }

    public void ShowAdsPopup(OnHideScreen onHideScreen = null)
    {
        Timing.RunCoroutine(ShowAndLoadScreenAsync<AdsPopup>("AdsPopup", CanvasType.Main, null, onHideScreen));
    }

    [Button]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}