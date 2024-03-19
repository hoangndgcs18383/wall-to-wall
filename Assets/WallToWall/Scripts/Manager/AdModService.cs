using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdModService : IAdsService
{
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9371736562739737~9064752988";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
  private string _adUnitId = "unused";
#endif
    
    private InterstitialAd _interstitialAd;
    
    public event Action OnInitializationCompleteEvent = delegate { };
    public event Action<string> OnAdsAdLoadedEvent = delegate { };
    public event Action<string, ShowAdResult> OnAdsShowCompleteEvent = delegate { };

    public void Initialize()
    {
        MobileAds.Initialize(initStatus =>
        {
            OnInitializationCompleteEvent.Invoke();
        });
    }

    public void LoadInterstitial()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
        
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    public void ShowInterstitial()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }
}
