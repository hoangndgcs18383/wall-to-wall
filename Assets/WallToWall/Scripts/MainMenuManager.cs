using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zeff.Framework.Extensions;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [BoxGroup("Animation")] [SerializeField]
    private float jumpDuration = 0.5f;

    [BoxGroup("Animation")] [SerializeField]
    private Ease jumpEase = Ease.OutBack;

    [BoxGroup("Animation")] [SerializeField]
    private float jumpHeight = 0.5f;

    [BoxGroup("Animation")] [SerializeField]
    private float scaleDuration = 0.5f;

    [BoxGroup("Animation")] [SerializeField]
    private Ease scaleEase = Ease.OutBack;

    [BoxGroup("Animation")] [SerializeField]
    private float scaleWeight = 1.1f;

    [BoxGroup("Animation")] [SerializeField]
    private Vector3 rotateAngle = new Vector3(0, 0, 2);

    [BoxGroup("Animation")] [SerializeField]
    private float rotateDuration = 0.5f;

    [BoxGroup("Animation")] [SerializeField]
    private Ease rotateEase = Ease.OutBack;

    [BoxGroup("Animation")] [SerializeField]
    private RotateMode rotateMode = RotateMode.FastBeyond360;

    [BoxGroup("References")] [SerializeField]
    private RatePanel ratePanel;

    [SerializeField] private SettingPanel settingPanel;

    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W rateButton;
    [SerializeField] private ButtonW2W playButton;
    [SerializeField] private ButtonW2W rankButton;
    [SerializeField] private ButtonW2W settingButton;


    [BoxGroup("Skins features")] public List<Sprite> skins = new List<Sprite>();
    [SerializeField] private RectTransform currentPlayer;
    [SerializeField] private Image currentPlayerSprite;
    [SerializeField] private Image currentUnlockStarImage;
    [SerializeField] private Sprite unlockSkinSprite;
    [SerializeField] private Sprite lockStarSprite;
    [SerializeField] private Sprite unlockStarSprite;
    [SerializeField] private ButtonW2W nextSkinButton;
    [SerializeField] private ButtonW2W previousButton;

    private int _currentSkinIndex = 0;
    private bool _isTransitioning = false;
    private CanvasGroup _canvasGroup;

    #region Skins features

    public void OnLoadSkin()
    {
        PlayerPrefs.SetInt("CurrentSkinIndex",_currentSkinIndex);
        if (PlayerPrefs.GetInt("BestScore", 0) < 10 && _currentSkinIndex > 0)
        {
            currentPlayerSprite.sprite = unlockSkinSprite;
            currentUnlockStarImage.sprite = lockStarSprite;
            return;
        }

        currentPlayerSprite.sprite = skins[_currentSkinIndex];
        currentUnlockStarImage.sprite = unlockStarSprite;
    }

    public void NextSkin()
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
        if (_currentSkinIndex >= skins.Count)
        {
            _currentSkinIndex = 0;
        }

        OnLoadSkin();
    }

    public void PreviousSkin()
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
            _currentSkinIndex = skins.Count - 1;
        }

        OnLoadSkin();
    }

    private void Start()
    {
        Initialize();
    }

    #endregion

    private void Initialize()
    {
        _currentSkinIndex = PlayerPrefs.GetInt("CurrentSkinIndex", 0);
        OnLoadSkin();
        currentPlayer.DOKill();
        currentPlayer.localPosition = Vector3.zero;
        currentPlayer.localScale = Vector3.one;
        currentPlayer.localRotation = Quaternion.identity;

        currentPlayer.DOLocalMoveY(jumpHeight, jumpDuration).SetEase(jumpEase).SetLoops(-1, LoopType.Yoyo);
        currentPlayer.DOScale(scaleWeight, scaleDuration).SetEase(scaleEase).SetLoops(-1, LoopType.Yoyo);
        currentPlayer.DORotate(rotateAngle, rotateDuration, rotateMode).SetEase(rotateEase).SetLoops(-1, LoopType.Yoyo);
        _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        rateButton.onClick.AddListener(ShowRatePanel);
        settingButton.onClick.AddListener(settingPanel.Show);
        playButton.onClick.AddListener(ShowInGamePanel);
        nextSkinButton.onClick.AddListener(NextSkin);
        previousButton.onClick.AddListener(PreviousSkin);
    }

    private void OnDisable()
    {
        rateButton.onClick.RemoveListener(ShowRatePanel);
        settingButton.onClick.RemoveListener(settingPanel.Show);
        playButton.onClick.RemoveListener(ShowInGamePanel);
        nextSkinButton.onClick.RemoveListener(NextSkin);
        previousButton.onClick.RemoveListener(PreviousSkin);
    }

    [Button]
    public void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        Initialize();
    }

    [Button]
    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void ShowRatePanel()
    {
        ratePanel.Show();
    }

    private void ShowInGamePanel()
    {
        SceneManager.LoadSceneAsync("InGame");
        InGameManager.Instance.Reload();
        Hide();
    }
}