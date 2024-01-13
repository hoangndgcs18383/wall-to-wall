using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private static AdsManager _instance;

    public static AdsManager Instance
    {
        get { return _instance ??= new AdsManager(); }
    }

    private const string _androidGameId = "5479261";
    private const string _iOSGameId = "5479260";

    private const string _adUnitId = "Banner_Android";

    private string _adUnitInterstitialAndroidId = "Interstitial_Android";
    private string _adUnitInterstitialIOSId = "Interstitial_iOS";

    string _adUnitInterstitialId;

    private string _gameId;
    private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

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
        _adUnitInterstitialId = _adUnitInterstitialAndrodiId;
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, false, this);
        }
#endif
    }

    private bool _isShowBanner = false;
    private bool _isShowInterstitial = false;

    public void Options(bool isShowBanner, bool isShowInterstitial)
    {
        _isShowBanner = isShowBanner;
        _isShowInterstitial = isShowInterstitial;
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");

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
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }

    private void OnBannerError(string message)
    {
        Debug.LogError("Banner Error: " + message);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");

        ShowBannerAd();
    }

    private void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(_adUnitId, options);
    }

    private void OnBannerShown()
    {
        Debug.Log("Banner shown");
    }

    private void OnBannerHidden()
    {
        Debug.Log("Banner hidden");
    }

    private void OnBannerClicked()
    {
        Debug.Log("Banner clicked");
    }

    #region Interstitial

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
    
    public void OnUnityAdsAdLoaded(string placementId)
    {
        ShowInterstitial();
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
        
    }
    

    #endregion

}