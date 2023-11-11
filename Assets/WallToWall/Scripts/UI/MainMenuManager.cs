using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private UnlockSkinPanel unlockSkinPanel;

    [BoxGroup("Buttons")] [SerializeField] private ButtonW2W rateButton;
    [SerializeField] private ButtonW2W playButton;
    [SerializeField] private ButtonW2W rankButton;
    [SerializeField] private ButtonW2W settingButton;


    [BoxGroup("Skins features")] public List<Sprite> skins = new List<Sprite>();
    [BoxGroup("Skins features")] public List<string> skinName = new List<string>();
    [SerializeField] private RectTransform currentPlayer;
    [SerializeField] private Image currentPlayerSprite;
    [SerializeField] private Image currentUnlockStarImage;
    [SerializeField] private Sprite unlockSkinSprite;
    [SerializeField] private Sprite lockStarSprite;
    [SerializeField] private Sprite unlockStarSprite;
    [SerializeField] private ButtonW2W nextSkinButton;
    [SerializeField] private ButtonW2W previousButton;
    [SerializeField] private TMP_Text currentSkinText;

    [BoxGroup("GUI")] [SerializeField] private CanvasGroup canvasGroup;
    [BoxGroup("GUI")] [SerializeField] private Image background;
    [BoxGroup("GUI")] [SerializeField] private Image hole;

    private int _currentSkinIndex = 0;
    private bool _isTransitioning = false;

    #region Skins features

    public void OnLoadSkin()
    {
        PlayerPrefs.SetInt("CurrentSkinIndex", _currentSkinIndex);
        
        if (PlayerPrefs.GetInt("BestScore", 0) < 10 && _currentSkinIndex > 0)
        {
            currentPlayerSprite.sprite = unlockSkinSprite;
            currentUnlockStarImage.sprite = lockStarSprite;
            return;
        }

        currentPlayerSprite.sprite = skins[_currentSkinIndex];
        currentUnlockStarImage.sprite = unlockStarSprite;
        currentSkinText.SetText(skinName[_currentSkinIndex]);

        for (int i = 0; i < skins.Count; i++)
        {
            if (!unlockSkinPanel.IsUnlock(i))
            {
                unlockSkinPanel.Show(i);
            }
        }
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
    
    public Sprite GetCurrentSkin()
    {
        return skins[_currentSkinIndex];
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
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        hole.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        Initialize();
    }

    [Button]
    public void Hide()
    {
        LoadingManager.Instance.Transition(TransitionType.Fade, hole);
        LoadingManager.Instance.Transition(TransitionType.Fade, background, () =>
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }, () =>
        {
            hole.rectTransform.DOKill();
            hole.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        });
    }

    private void ShowRatePanel()
    {
        ratePanel.Show();
    }

    private void ShowInGamePanel()
    {
        SceneManager.LoadSceneAsync("InGame");
        InGameManager.Instance.Reload();
        AudioManager.Instance.StopBGM();
        Hide();
    }
}