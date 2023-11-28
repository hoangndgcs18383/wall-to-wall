using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager
{
    private static TutorialManager _instance;

    public static TutorialManager Instance
    {
        get { return _instance ??= new TutorialManager(); }
    }

    private List<TutorialConfig> _tutorialConfigs;

    public void Initialize(List<TutorialConfig> tutorialConfigs)
    {
        _tutorialConfigs = tutorialConfigs;
    }

    public void NextTutorial()
    {
        if (_tutorialConfigs == null || _tutorialConfigs.Count == 0) return;

        _tutorialConfigs.RemoveAt(0);
    }

    public bool CheckValidTutorial(int index)
    {
        if (_tutorialConfigs == null || _tutorialConfigs.Count == 0) return false;

        if (_tutorialConfigs[index].tutorialData.tutorialText != null)
        {
            return true;
        }

        return false;
    }

    public TutorialConfig GetTutorialConfig(int index)
    {
        return _tutorialConfigs[index];
    }

    public void ClearCurrentTutorialList()
    {
        _tutorialConfigs.Clear();
    }

    public List<TutorialConfig> GetCurrentTutorialList() => _tutorialConfigs;
}

[Serializable]
public class TutorialData
{
    public string tutorialText;
}

[Serializable]
public struct TutorialConfig
{
    public int tutorialIndex;
    public string tutorialKey;
    public TutorialData tutorialData;
}