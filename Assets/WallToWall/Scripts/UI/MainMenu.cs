using System;
using System.Collections.Generic;
using DG.Tweening;
using Hzeff.Events;
using MEC;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct MainMenuEvent : IEvent
{
    public Action OnCloseComplete;
}

public class MainMenu : BaseScreen
{
    [BoxGroup("Animation")] [SerializeField]
    private float jumpDuration = 5f;

    [BoxGroup("Animation")] [SerializeField]
    private Ease jumpEase = Ease.OutBack;

    [BoxGroup("Animation")] [SerializeField]
    private float jumpHeight = 0.5f;

    [BoxGroup("Animation")] [SerializeField]
    private float scaleDuration = 2f;

    [BoxGroup("Animation")] [SerializeField]
    private Ease scaleEase = Ease.OutBack;

    [BoxGroup("Animation")] [SerializeField]
    private float scaleWeight = 1.05f;

    [BoxGroup("Animation")] [SerializeField]
    private Vector3 rotateAngle = new Vector3(0, 0, 2);

    [BoxGroup("Animation")] [SerializeField]
    private float rotateDuration = 0.5f;

    [BoxGroup("Animation")] [SerializeField]
    private Ease rotateEase = Ease.OutBack;

    [BoxGroup("Animation")] [SerializeField]
    private RotateMode rotateMode = RotateMode.FastBeyond360;

    [BoxGroup("References")] [SerializeField]
    private RectTransform currentPlayer;

    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W rateButton;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W playButton;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W rankButton;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W settingButton;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W inventoryButton;
    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W removeAdsButton;

    [BoxGroup("GUI")] [SerializeField] private CanvasGroup canvasGroup;
    [BoxGroup("GUI")] [SerializeField] private Image background;
    [BoxGroup("GUI")] [SerializeField] private Image blurBackground;

    [BoxGroup("GUI")] [SerializeField] private SkeletonGraphic skeletonGraphic;
    //[BoxGroup("GUI")] [SerializeField] private Image mainBackground;
    //[BoxGroup("GUI")] [SerializeField] private Image hole;

    [BoxGroup("Config")] [SerializeField] private PlayerConfig playerConfig;

    [SerializeField] private Image currentPlayerSprite;
    [BoxGroup("GUI")] [SerializeField] private SkeletonGraphic skinSkeleton;

    [SerializeField] private Image currentUnlockStarImage;

    //[SerializeField] private Sprite unlockSkinSprite;
    //[SerializeField] private Material unlockSkinMaterial;
    [SerializeField] private Sprite lockStarSprite;
    [SerializeField] private Sprite unlockStarSprite;
    [SerializeField] private ButtonW2W nextSkinButton;
    [SerializeField] private ButtonW2W previousButton;
    [SerializeField] private TMP_Text currentSkinText;

    private int _currentSkinIndex = 0;
    private bool _isTransitioning = false;
    private EventBinding<NameDisplayChangedEvent> _nameDisplay;
    private CanvasGroup _allCanvasGroup;

    public override void Initialize()
    {
        base.Initialize();

        _allCanvasGroup = GetComponent<CanvasGroup>();
        Dictionary<string, SkinData> sprites = new Dictionary<string, SkinData>();
        for (int i = 0; i < playerConfig.skins.Count; i++)
        {
            sprites.Add(playerConfig.skins[i].key, playerConfig.skins[i]);
        }

        SkinManager.Instance.Initialize(sprites);
        TutorialManager.Instance.Initialize();
        //SkinManager.Instance.AddListenerSkinUnlocked(OnUnlockSkin);
#if UNITY_ANDROID || UNITY_IOS
        //AdsManager.Instance.Initialize();
#endif

        //nextSkinButton.onClick.AddListener(NextSkin);
        //previousButton.onClick.AddListener(PreviousSkin);

        rateButton.onClick.AddListener(ShowRatePanel);
        settingButton.onClick.AddListener(ShowSettingPanel);
        if (TutorialManager.Instance.HadReleasedTutorial)
        {
            playButton.onClick.AddListener(ShowInGamePanel);
        }
        else
        {
            playButton.onClick.AddListener(ShowInTutorialPanel);
        }

        rankButton.onClick.AddListener(ShowRankPanel);
        inventoryButton.onClick.AddListener(ShowInventoryPanel);
        removeAdsButton.onClick.AddListener(ShowRemoveAdsScreen);

        OnNameDisplayChanged(new NameDisplayChangedEvent
            { DisplayName = SaveSystem.Instance.GetString(PrefKeys.UserName) });
        EventDispatcher<NameDisplayChangedEvent>.Register(
            new EventBinding<NameDisplayChangedEvent>(OnNameDisplayChanged));
        EventDispatcher<MainMenuEvent>.Register(new EventBinding<MainMenuEvent>(OnUnlockAllSkinEvent));
        Transition();
    }

    private void OnNameDisplayChanged(NameDisplayChangedEvent obj)
    {
        currentSkinText.SetText(obj.DisplayName);
    }

    private void Transition()
    {
        OnLoadSkin();
        currentPlayer.DOKill();
        currentPlayer.localPosition = Vector3.zero;
        currentPlayer.localScale = Vector3.one;
        currentPlayer.localRotation = Quaternion.identity;

        currentPlayer.DOLocalMoveY(jumpHeight, jumpDuration).SetEase(jumpEase).SetLoops(-1, LoopType.Yoyo);
        currentPlayer.DOScaleY(0.85f, jumpDuration).SetEase(jumpEase).SetLoops(-1, LoopType.Yoyo);

        //currentPlayer.DOScale(scaleWeight, scaleDuration).SetEase(scaleEase).SetLoops(-1, LoopType.Yoyo);
        //currentPlayer.DORotate(rotateAngle, rotateDuration, rotateMode).SetEase(rotateEase).SetLoops(-1, LoopType.Yoyo);
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);

        background.gameObject.SetActive(true);
        blurBackground.gameObject.SetActive(true);
        _allCanvasGroup.alpha = 0;
        _allCanvasGroup.interactable = false;
        _allCanvasGroup.blocksRaycasts = false;
        _allCanvasGroup.DOFade(1, 2f);
        ShowOrHideCanvasGroup(true);
        skeletonGraphic.allowMultipleCanvasRenderers = true;

        Timing.CallDelayed(3f, () =>
        {
            CheckForUpdateSkin();
        });
    }

    private void OnUnlockAllSkinEvent(MainMenuEvent obj)
    {
        CheckForUpdateSkin();
    }

    private void CheckForUpdateSkin()
    {
        SkinManager.Instance.ClearCurrentSkinList();
        for (int i = 0; i < playerConfig.skins.Count; i++)
        {
            SkinManager.Instance.CheckValidSkinUnlock(PlayerPrefs.GetInt("BestScore", 0), i);
        }

        if (SkinManager.Instance.GetCurrentSkinList() != null &&
            SkinManager.Instance.GetCurrentSkinList().Count > 0)
        {
            UIManager.Instance.ShowUnlockSkinScreen(SkinManager.Instance.GetCurrentSkinList());
        }

        _allCanvasGroup.interactable = true;
        _allCanvasGroup.blocksRaycasts = true;
    }

    public override void Hide()
    {
        _allCanvasGroup.DOFade(0, 2f).OnComplete(base.Hide);
        skeletonGraphic.allowMultipleCanvasRenderers = false;
    }

    public RectTransform GetInventoryButton()
    {
        return inventoryButton.transform as RectTransform;
    }

    public void OnLoadSkin()
    {
        PlayerPrefs.SetInt("CurrentSkinIndex", _currentSkinIndex);

        /*if (PlayerPrefs.GetInt("BestScore", 0) < playerConfig.skins[_currentSkinIndex].unlockPoint &&
            _currentSkinIndex > 0)
        {
            currentPlayerSprite.sprite = playerConfig.skins[_currentSkinIndex].lockSprite;
            currentUnlockStarImage.sprite = lockStarSprite;
            currentSkinText.SetText(playerConfig.skins[_currentSkinIndex].key);
            mainBackground.sprite = playerConfig.skins[_currentSkinIndex].backgroundMainSprite;
            //background.sprite = playerConfig.skins[_currentSkinIndex].backgroundAllSprite;
            return;
        }*/

        var skinData = SkinManager.Instance.GetCurrentSkin();
        //unlockSkinMaterial.SetTexture("_MainTex", skinData.unlockSprite.texture);
        currentPlayerSprite.sprite = skinData.unlockSprite;
        //currentUnlockStarImage.sprite = unlockStarSprite;
        //skinSkeleton.initialSkinName = skinData.hash;
        skinSkeleton.skeletonDataAsset = skinData.skeletonDataAsset;
        skinSkeleton.Initialize(true);
        //currentSkinText.SetText(skinData.nameDisplay);
        //mainBackground.sprite = skinData.backgroundMainSprite;
        //background.sprite = playerConfig.skins[_currentSkinIndex].backgroundAllSprite;
        PlayerPrefs.SetInt("LastSkinIndex", _currentSkinIndex);
    }

    #region Button Events

    private void ShowOrHideCanvasGroup(bool isShow)
    {
        skeletonGraphic.gameObject.SetActive(isShow);
        canvasGroup.alpha = isShow ? 1 : 0;
        canvasGroup.interactable = isShow;
        canvasGroup.blocksRaycasts = isShow;
    }

    private void ShowRatePanel()
    {
        UIManager.Instance.ShowRateScreen();
    }

    private void ShowSettingPanel()
    {
        UIManager.Instance.ShowSettingScreen();
    }

    private void ShowInGamePanel()
    {
        SceneManager.LoadSceneAsync("InGame");
        AdsManager.Instance.LoadAndShowInterstitial();
        //InGameManager.Instance.Reload();
        LoadingManager.Instance.Transition(TransitionType.Fade, background, () =>
        {
            ShowOrHideCanvasGroup(false);
            AudioManager.Instance.StopBGM();
        }, () =>
        {
            background.gameObject.SetActive(false);
            blurBackground.gameObject.SetActive(false);
            UIManager.Instance.ShowInGameScreen();
            AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
        });
    }

    private void ShowInTutorialPanel()
    {
        UIManager.Instance.ShowTutorialScreen(() =>
        {
            ShowInGamePanel();
            playButton.onClick.RemoveListener(ShowInTutorialPanel);
            playButton.onClick.AddListener(ShowInGamePanel);
            TutorialManager.Instance.HadReleasedTutorial = true;
        });

        //LoadingManager.Instance.Transition(TransitionType.Fade, hole);
        LoadingManager.Instance.Transition(TransitionType.Fade, background, () =>
        {
            ShowOrHideCanvasGroup(false);
            AudioManager.Instance.StopBGM();
        }, () =>
        {
            blurBackground.gameObject.SetActive(false);
            //UIManager.Instance.ShowInGameScreen();
            AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
        });
    }

    private void ShowRankPanel()
    {
        UIManager.Instance.ShowRankScreen();
    }

    private void ShowInventoryPanel()
    {
        UIManager.Instance.ShowInventoryScreen(OnLoadSkin);
    }

    private void ShowRemoveAdsScreen()
    {
        UIManager.Instance.ShowAdsPopup();
    }

    #endregion
}