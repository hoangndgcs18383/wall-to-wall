using System;
using UnityEngine;

public enum ShowAdResult
{
    Finish,
    Skip,
    Failed,
    Success
}

public interface IAdsService
{
    event Action OnInitializationCompleteEvent;
    event Action<string> OnAdsAdLoadedEvent;
    event Action<string, ShowAdResult> OnAdsShowCompleteEvent;
    void Initialize();
    void LoadInterstitial();
    void ShowInterstitial();
}