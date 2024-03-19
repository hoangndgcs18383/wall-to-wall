using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsService : IAdsService, IUnityAdsInitializationListener, IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    private const string _androidGameId = "5479261";
    private const string _iOSGameId = "5479260";

#if UNITY_ANDROID
    private string _bannerId = "Banner_Android";
#elif UNITY_IPHONE
        private string _bannerId = "Banner_iOS";
#endif

    private string _adUnitInterstitialAndroidId = "Interstitial_Android";
    private string _adUnitInterstitialIOSId = "Interstitial_iOS";

    private string _adUnitInterstitialId;
    private string _gameId;

    private bool _isShowBanner = false;
    private bool _isShowInterstitial = false;
    private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    public event Action OnInitializationCompleteEvent = delegate { };
    public event Action<string> OnAdsAdLoadedEvent = delegate { };
    public event Action<string, ShowAdResult> OnAdsShowCompleteEvent = delegate { };

    public void Initialize()
    {
#if UNITY_IOS
        _gameId = _iOSGameId;
        _adUnitInterstitialId = _adUnitInterstitialIOSId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
        _adUnitInterstitialId = _adUnitInterstitialAndroidId;
#elif UNITY_EDITOR
        _gameId = _androidGameId;
        _adUnitInterstitialId = _adUnitInterstitialAndroidId;
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, false, this);
        }
#endif
    }

    public void LoadInterstitial()
    {
        Advertisement.Load(_adUnitInterstitialId, this);
    }

    public void ShowInterstitial()
    {
        if (Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Show(_adUnitInterstitialId, this);
        }
    }

    public void OnInitializationComplete()
    {
        OnInitializationCompleteEvent.Invoke();

        if (_isShowBanner)
        {
            Debug.Log("Unity Ads -- Show Banner");
            Advertisement.Banner.SetPosition(_bannerPosition);
            LoadBanner();
        }

        if (_isShowInterstitial)
        {
            Debug.Log("Unity Ads -- Show Interstitial");
            //LoadInterstitial();
        }

#if UNITY_ANDROID || UNITY_IOS
        LoadInterstitial();
#endif
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        OnAdsAdLoadedEvent.Invoke(placementId);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        OnAdsShowCompleteEvent.Invoke(placementId, showCompletionState == UnityAdsShowCompletionState.COMPLETED
            ? ShowAdResult.Finish
            : ShowAdResult.Skip);
    }

    #region Banner

    public void LoadBanner()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_bannerId, options);
#endif
    }

    private void OnBannerLoaded()
    {
    }

    private void OnBannerError(string message)
    {
    }

    #endregion
}