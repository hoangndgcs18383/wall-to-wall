using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum TransitionType
{
    Pinch,
    Fade
}

public class LoadingManager : MonoBehaviour
{
    [TabGroup("Transition Materials")] [SerializeField]
    private Material backgroundMaterial;

    [TabGroup("Transition")] [SerializeField]
    private float transitionDuration = 1f;

    [TabGroup("Transition")] [SerializeField]
    private float startValue = 0f;

    [TabGroup("Transition")] [SerializeField]
    private float endValue = 0.8f;

    [SerializeField] private Image background;

    [SerializeField] private TMP_Text startText;
    [SerializeField] private ButtonW2W startButton;

    public static LoadingManager Instance;
    private int _percentage;
    private int _targetPercentage;
    private CoroutineHandle _loadingCoroutine;

    public struct userAttributes
    {
        public int TriangleCountUpScore;
    }

    public struct appAttributes
    {
    }

    //private static readonly int PinchUvAmount = Shader.PropertyToID("_PinchUvAmount");
    //private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Application.quitting += RestTransition;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        QualitySettings.SetQualityLevel(0);

#if UNITY_ANDROID || UNITY_IOS
        AdsManager.Instance.Initialize();
#endif

        InitializeUnityServicesAsync();

        startButton.targetGraphic.raycastTarget = false;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            startButton.targetGraphic.raycastTarget = true;
            startText.SetText("Press to start");
        }
        else
        {
            RemoteManager.Instance.InitializeAsync((percent) =>
            {
                _targetPercentage += percent;
                Timing.KillCoroutines(_loadingCoroutine);
                _loadingCoroutine = Timing.RunCoroutine(IETextPercentage());
            });

            //SpreadsheetManager.Write(new GSTU_Search(animal.associatedSheet, animal.associatedWorksheet, "G10"), new ValueRange(list), null);
        }

        /*AddressablesManager.TryLoadAssetSync(BackgroundAddress.GetAddress("BG_FIRST"), out Sprite backgroundSprite);
        background.sprite = backgroundSprite;*/
        /*if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }
        
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());*/
    }

    public IEnumerator<float> IETextPercentage()
    {
        while (_percentage < _targetPercentage)
        {
            _percentage = Mathf.Clamp(_percentage + 1, 0, _targetPercentage);
            startText.SetText($"{_percentage}%");
            if (_percentage >= 100)
            {
                startButton.targetGraphic.raycastTarget = true;
                startText.SetText("Press to start");
            }

            yield return Timing.WaitForOneFrame;
        }
    }

    private void ApplyRemoteSettings(ConfigResponse obj)
    {
        Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config);
    }

    async void InitializeUnityServicesAsync()
    {
        await UnityServices.InitializeAsync();

        // remote config requires authentication for managing environment information
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (!SaveSystem.Instance.HasKey(PrefKeys.UserId))
            {
                SaveSystem.Instance.SetString(PrefKeys.UserId, AuthenticationService.Instance.PlayerId);


                string randomDisplayName = "User" + Random.Range(0, 1000);
                SaveSystem.Instance.SetString(PrefKeys.UserName, randomDisplayName);
            }
        }

        IAPManager.Instance.InitializedIAP();
    }

    public void Transition(TransitionType transitionType, Image image, Action onStart = null, Action onComplete = null)
    {
        switch (transitionType)
        {
            case TransitionType.Fade:
                FadeTransition(image, onStart, onComplete);
                break;
            case TransitionType.Pinch:
                FadeTransition(image, onStart, onComplete);
                break;
        }
    }

    private void FadeTransition(Image image, Action onStart, Action onComplete)
    {
        onStart?.Invoke();
        image.material.EnableKeyword("FADE_ON");
        image.material.SetFloat("_FadeAmount", startValue);
        DOVirtual.Float(startValue, endValue, transitionDuration,
            value => { image.material.SetFloat("_FadeAmount", value); }).OnComplete(() =>
        {
            image.material.DisableKeyword("FADE_ON");
            onComplete?.Invoke();
        });
    }

    private void RestTransition()
    {
        backgroundMaterial.EnableKeyword("FADE_ON");
        backgroundMaterial.SetFloat("_FadeAmount", startValue);
    }
}