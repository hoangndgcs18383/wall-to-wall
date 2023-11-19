using System;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

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
    private float endValue = 1f;
    
    [SerializeField] private Image background;

    public static LoadingManager Instance;

    public struct userAttributes
    {
        public int TriangleCountUpScore;
    }
    public struct appAttributes {}

    //private static readonly int PinchUvAmount = Shader.PropertyToID("_PinchUvAmount");
    //private static readonly int FadeAmount = Shader.PropertyToID("_FadeAmount");

    private void Awake()
    {
        Instance = this;
    }
    
    async void Start()
    {
        Application.quitting += RestTransition;
        
        /*AddressablesManager.TryLoadAssetSync(BackgroundAddress.GetAddress("BG_FIRST"), out Sprite backgroundSprite);
        background.sprite = backgroundSprite;*/
        /*if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }
        
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());*/
    }

    private void ApplyRemoteSettings(ConfigResponse obj)
    {
        Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config);
    }

    async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();

        // remote config requires authentication for managing environment information
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
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