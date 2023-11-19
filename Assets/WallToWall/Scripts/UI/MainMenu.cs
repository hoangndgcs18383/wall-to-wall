using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [BoxGroup("GUI")] [SerializeField] private CanvasGroup canvasGroup;
    [BoxGroup("GUI")] [SerializeField] private Image background;
    [BoxGroup("GUI")] [SerializeField] private Image hole;

    [BoxGroup("Config")] [SerializeField] private PlayerConfig playerConfig;

    [SerializeField] private Image currentPlayerSprite;
    [SerializeField] private Image currentUnlockStarImage;
    [SerializeField] private Sprite unlockSkinSprite;
    [SerializeField] private Sprite lockStarSprite;
    [SerializeField] private Sprite unlockStarSprite;
    [SerializeField] private ButtonW2W nextSkinButton;
    [SerializeField] private ButtonW2W previousButton;
    [SerializeField] private TMP_Text currentSkinText;

    private int _currentSkinIndex = 0;
    private bool _isTransitioning = false;
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
        SkinManager.Instance.AddListenerSkinUnlocked(OnUnlockSkin);
#if UNITY_ANDROID || UNITY_IOS
        AdsManager.Instance.Initialize();
#endif
        
        nextSkinButton.onClick.AddListener(NextSkin);
        previousButton.onClick.AddListener(PreviousSkin);

        rateButton.onClick.AddListener(ShowRatePanel);
        settingButton.onClick.AddListener(ShowSettingPanel);
        playButton.onClick.AddListener(ShowInGamePanel);
        rankButton.onClick.AddListener(ShowRankPanel);
        Transition();
    }

    private void Transition()
    {
        OnLoadSkin();
        hole.rectTransform.DOKill();
        hole.rectTransform.localRotation = Quaternion.identity;
        hole.rectTransform.DORotate(new Vector3(0, 0, 360), 10f, rotateMode).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        currentPlayer.DOKill();
        currentPlayer.localPosition = Vector3.zero;
        currentPlayer.localScale = Vector3.one;
        currentPlayer.localRotation = Quaternion.identity;

        currentPlayer.DOLocalMoveY(jumpHeight, jumpDuration).SetEase(jumpEase).SetLoops(-1, LoopType.Yoyo);
        currentPlayer.DOScale(scaleWeight, scaleDuration).SetEase(scaleEase).SetLoops(-1, LoopType.Yoyo);
        currentPlayer.DORotate(rotateAngle, rotateDuration, rotateMode).SetEase(rotateEase).SetLoops(-1, LoopType.Yoyo);
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        _allCanvasGroup.alpha = 0;
        _allCanvasGroup.DOFade(1, 2f);
        background.gameObject.SetActive(true);
        ShowOrHideCanvasGroup(true);

        Timing.CallDelayed(3f, () =>
        {
            for (int i = 0; i < playerConfig.skins.Count; i++)
            {
                SkinManager.Instance.CheckValidSkinUnlock(PlayerPrefs.GetInt("BestScore", 0), i);
            }
        });
    }

    private void OnUnlockSkin(SkinData skinData)
    {
        UIManager.Instance.ShowUnlockSkinScreen(skinData);
    }

    public override void Hide()
    {
        _allCanvasGroup.DOFade(0, 2f).OnComplete(base.Hide);
    }

    private void NextSkin()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        currentPlayerSprite.DOFade(0, 0.5f).OnComplete(() =>
        {
            _isTransitioning = false;
            NextSkinIndex();
            currentPlayerSprite.DOFade(1, 0.5f);
        });
    }

    private void NextSkinIndex()
    {
        _currentSkinIndex++;
        if (_currentSkinIndex >= playerConfig.skins.Count)
        {
            _currentSkinIndex = 0;
        }

        OnLoadSkin();
    }

    private void PreviousSkin()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        currentPlayerSprite.DOFade(0, 0.5f).OnComplete(() =>
        {
            _isTransitioning = false;
            PreviousSkinIndex();
            currentPlayerSprite.DOFade(1, 0.5f);
        });
    }

    private void PreviousSkinIndex()
    {
        _currentSkinIndex--;
        if (_currentSkinIndex < 0)
        {
            _currentSkinIndex = playerConfig.skins.Count - 1;
        }

        OnLoadSkin();
    }

    private void OnLoadSkin()
    {
        PlayerPrefs.SetInt("CurrentSkinIndex", _currentSkinIndex);

        if (PlayerPrefs.GetInt("BestScore", 0) < 10 && _currentSkinIndex > 0)
        {
            currentPlayerSprite.sprite = unlockSkinSprite;
            currentUnlockStarImage.sprite = lockStarSprite;
            currentSkinText.SetText(playerConfig.skins[_currentSkinIndex].key);
            return;
        }

        currentPlayerSprite.sprite = playerConfig.skins[_currentSkinIndex].sprite;
        currentUnlockStarImage.sprite = unlockStarSprite;
        currentSkinText.SetText(playerConfig.skins[_currentSkinIndex].key);
    }

    #region Button Events

    private void ShowOrHideCanvasGroup(bool isShow)
    {
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
        //InGameManager.Instance.Reload();

        LoadingManager.Instance.Transition(TransitionType.Fade, hole);
        LoadingManager.Instance.Transition(TransitionType.Fade, background, () =>
        {
            ShowOrHideCanvasGroup(false);
            AudioManager.Instance.StopBGM();
        }, () =>
        {
            hole.rectTransform.DOKill();
            hole.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
            UIManager.Instance.ShowInGameScreen();
            AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
        });
    }

    private void ShowRankPanel()
    {
        UIManager.Instance.ShowRankScreen();
    }

    #endregion
}