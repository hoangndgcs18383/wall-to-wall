using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : BaseScreen
{
    [SerializeField] private ButtonW2W selectButton;
    [SerializeField] private ButtonW2W nextSkinButton;
    [SerializeField] private ButtonW2W previousSkinButton;
    [SerializeField] private ButtonW2W closeButton;

    [SerializeField] private TMP_Text currentSkinText;
    [SerializeField] private Image currentSkinImage;
    [SerializeField] private Image currentSkinBackground;
    [SerializeField] private Image selectSkinImage;
    [SerializeField] private Sprite selectSkinSprite;
    [SerializeField] private Sprite unselectSkinSprite;
    [SerializeField] private TMP_Text selectSkinText;
    [SerializeField] private GameObject newTag;

    [SerializeField] private PlayerConfig playerConfig;

    private int _currentSkinIndex;
    private bool _isTransitioning = false;
    private Action _onClose;

    public override void Initialize()
    {
        base.Initialize();
        selectButton.onClick.AddListener(OnSelect);
        nextSkinButton.onClick.AddListener(OnNextSkin);
        previousSkinButton.onClick.AddListener(OnPreviousSkin);
        closeButton.onClick.AddListener(OnClose);
    }

    public void RegisterOnClose(Action onClose)
    {
        _onClose = onClose;
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        _currentSkinIndex = SkinManager.Instance.GetCurrentSkinIndex();
        LoadSkin();
    }

    private void OnClose()
    {
        Hide();
    }

    private void OnPreviousSkin()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        currentSkinImage.transform.DOScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
        {
            _isTransitioning = false;
            PreviousSkinIndex();
            currentSkinImage.transform.DOScale(Vector3.one, 0.2f);
        });
    }

    private void PreviousSkinIndex()
    {
        _currentSkinIndex--;
        if (_currentSkinIndex < 0)
        {
            _currentSkinIndex = playerConfig.skins.Count - 1;
        }

        LoadSkin();
    }

    private void OnNextSkin()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        currentSkinImage.transform.DOScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
        {
            _isTransitioning = false;
            NextSkinIndex();
            currentSkinImage.transform.DOScale(Vector3.one, 0.2f);
        });
    }

    private void NextSkinIndex()
    {
        _currentSkinIndex++;
        if (_currentSkinIndex >= playerConfig.skins.Count)
        {
            _currentSkinIndex = 0;
        }

        LoadSkin();
    }

    private void LoadSkin()
    {
        PlayerPrefs.SetInt("CurrentSkinIndex", _currentSkinIndex);

        if (!IsNew())
        {
            newTag.SetActive(true);
            SetNew();
        }
        else
        {
            newTag.SetActive(false);
        }

        if (PlayerPrefs.GetInt("BestScore", 0) < playerConfig.skins[_currentSkinIndex].unlockPoint &&
            _currentSkinIndex > 0)
        {
            currentSkinImage.sprite = playerConfig.skins[_currentSkinIndex].lockSprite;
            currentSkinText.SetText(playerConfig.skins[_currentSkinIndex].nameDisplay);
            currentSkinBackground.sprite = playerConfig.skins[_currentSkinIndex].backgroundMainSprite;
            OnSelectSkin(false, playerConfig.skins[_currentSkinIndex].unlockPoint);
            return;
        }

        currentSkinImage.sprite = playerConfig.skins[_currentSkinIndex].unlockSprite;
        currentSkinText.SetText(playerConfig.skins[_currentSkinIndex].nameDisplay);
        currentSkinBackground.sprite = playerConfig.skins[_currentSkinIndex].backgroundMainSprite;
        OnSelectSkin(true, playerConfig.skins[_currentSkinIndex].unlockPoint);
        PlayerPrefs.SetInt("LastSkinIndex", _currentSkinIndex);
    }

    private void OnSelectSkin(bool isUnlock, int levelUnlock)
    {
        Unlock(isUnlock);
        if (isUnlock)
        {
            selectSkinImage.sprite = selectSkinSprite;
            selectSkinText.SetText("Select");
        }
        else
        {
            selectSkinImage.sprite = unselectSkinSprite;
            selectSkinText.SetText(levelUnlock.ToString());
        }

        selectButton.targetGraphic.raycastTarget = isUnlock;
    }

    private bool IsNew()
    {
        if (!SkinManager.Instance.IsSkinUnlocked(_currentSkinIndex)) return true;
        return PlayerPrefs.GetInt($"IsNewSkin{_currentSkinIndex}", 0) == 1;
    }

    private void SetNew()
    {
        if (!SkinManager.Instance.IsSkinUnlocked(_currentSkinIndex)) return;
        PlayerPrefs.SetInt($"IsNewSkin{_currentSkinIndex}", 1);
    }

    private void OnSelect()
    {
        SkinManager.Instance.SelectSkin($"Skin_{_currentSkinIndex}");
        Hide();
        _onClose?.Invoke();
    }

    private void Unlock(bool isUnlock)
    {
        if (!isUnlock)
        {
            currentSkinBackground.material.EnableKeyword("HSV_ON");
            currentSkinBackground.material.EnableKeyword("GREYSCALE_ON");
        }
        else
        {
            currentSkinBackground.material.DisableKeyword("HSV_ON");
            currentSkinBackground.material.DisableKeyword("GREYSCALE_ON");
        }
    }
}