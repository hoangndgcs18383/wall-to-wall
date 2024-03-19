#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
using System.Collections;
using Google.Play.Review;
using UnityEngine;
#endif

public class AppRatingManager : Singleton<AppRatingManager>
{
#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    private Coroutine _coroutine;
#endif

    public void Start()
    {
#if UNITY_ANDROID
        _coroutine = StartCoroutine(InitReview());
#endif
    }

    public void RateAndReview()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#elif UNITY_ANDROID
        StartCoroutine(LaunchReview());
#endif
    }

#if UNITY_ANDROID
    private IEnumerator InitReview(bool force = false)
    {
        if (_reviewManager == null) _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            if (force) DirectlyOpen();
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
    }

    public bool IsReviewAvailable()
    {
        return _playReviewInfo != null;
    }


    public IEnumerator LaunchReview()
    {
        if (_playReviewInfo == null)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            yield return StartCoroutine(InitReview(true));
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null;
        SaveSystem.Instance.SetInt(PrefKeys.CanTriggeredRatingPopup, 1);

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            DirectlyOpen();
            yield break;
        }
    }
#endif

    private void DirectlyOpen()
    {
        Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}");
    }
}