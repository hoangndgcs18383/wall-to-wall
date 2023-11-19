using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : IUnityAdsInitializationListener
{
    private static AdsManager _instance;

    public static AdsManager Instance
    {
        get { return _instance ??= new AdsManager(); }
    }

    private const string _androidGameId = "5479261";
    private const string _iOSGameId = "5479260";

    private const string _adUnitId = "Banner_Android";

    private string _gameId;
    private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    public void Initialize()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId;
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, false, this);
        }
#endif
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");

        Advertisement.Banner.SetPosition(_bannerPosition);
        LoadBanner();
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
}