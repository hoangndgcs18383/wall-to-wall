using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
        _currentSkinIndex = PlayerPrefs.GetInt("CurrentSkinIndex", 0);
        LoadSkin();
    }
    
    public void RegisterOnClose(Action onClose)
    {
        _onClose = onClose;
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
            return;
        }

        currentSkinImage.sprite = playerConfig.skins[_currentSkinIndex].unlockSprite;
        currentSkinText.SetText(playerConfig.skins[_currentSkinIndex].nameDisplay);
        currentSkinBackground.sprite = playerConfig.skins[_currentSkinIndex].backgroundMainSprite;
        PlayerPrefs.SetInt("LastSkinIndex", _currentSkinIndex);
    }

    private bool IsNew()
    {
        return PlayerPrefs.GetInt($"IsNewSkin{_currentSkinIndex}", 0) == 1;
    }

    private void SetNew()
    {
        PlayerPrefs.SetInt($"IsNewSkin{_currentSkinIndex}", 1);
    }

    private void OnSelect()
    {
        SkinManager.Instance.SelectSkin($"Skin_{_currentSkinIndex}");
        Hide();
        _onClose?.Invoke();
    }
}