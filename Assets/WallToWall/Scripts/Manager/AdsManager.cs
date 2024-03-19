using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager
{
    private static AdsManager _instance;

    public static AdsManager Instance
    {
        get { return _instance ??= new AdsManager(); }
    }

    private IAdsService _adsService;
    private int _currentShowAds = 0;

    public void Initialize()
    {
        _adsService = new AdModService();
        _adsService.Initialize();
        _adsService.OnInitializationCompleteEvent += OnInitializationComplete;
        _adsService.OnAdsAdLoadedEvent += OnAdsAdLoaded;
        _adsService.OnAdsShowCompleteEvent += OnAdsShowComplete;
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Service initialization complete.");
        _currentShowAds = SaveSystem.Instance.GetInt(PrefKeys.ShowAdsCount, 0);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    #region Interstitial

    public void LoadAndShowInterstitial()
    {
        if (_queueShowInterstitial.Count <= 0) LoadInterstitial();

        if (SaveSystem.Instance.GetInt(PrefKeys.DeathCount) >= GameConstant.AdsTriggerCount - 1 &&
            !IAPManager.Instance.IsRemoveAdsPurchased())
        {
#if UNITY_ANDROID || UNITY_IOS
            if (_queueShowInterstitial.Count > 0)
            {
                _queueShowInterstitial.Dequeue().Invoke();
                SaveSystem.Instance.SetInt(PrefKeys.DeathCount, 0);
            }
#endif
        }
    }

    private Queue<Action> _queueShowInterstitial = new Queue<Action>();


    private void LoadInterstitial()
    {
        _adsService.LoadInterstitial();
    }

    public void ShowInterstitial()
    {
        _adsService.ShowInterstitial();
    }

    private void OnAdsAdLoaded(string placementId)
    {
        _queueShowInterstitial.Enqueue(ShowInterstitial);
    }

    public void OnAdsShowComplete(string placementId, ShowAdResult showCompletionState)
    {
        Debug.Log("OnUnityAdsShowComplete " + placementId + " " + showCompletionState);
        _currentShowAds++;
        SaveSystem.Instance.SetInt(PrefKeys.ShowAdsCount, _currentShowAds);

        if (SaveSystem.Instance.GetInt(PrefKeys.ShowAdsCount) >= 3 &&
            SaveSystem.Instance.GetInt(PrefKeys.HasTriggeredAdsPopup, 0) == 0)
        {
            SaveSystem.Instance.SetInt(PrefKeys.HasTriggeredAdsPopup, 1);
            UIManager.Instance.ShowAdsPopup();
        }
    }

    #endregion
}