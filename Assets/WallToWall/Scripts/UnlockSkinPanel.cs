using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public struct SkinData : IUIData
{
    public string key;
    public string hash;
    public string nameDisplay;
    public bool isUnlock;
    public Sprite unlockSprite;
    public Sprite lockSprite;
    public Sprite backgroundAllSprite;
    public Sprite backgroundMainSprite;
    [FormerlySerializedAs("backgroundTriangleSprite")] public Sprite triangleSprite;
    public int unlockPoint;
    public EffectData effectData;
}

public class UnlockSkinPanel : BaseScreen
{
    [SerializeField] private Image skinImage;

    //[SerializeField] private List<SkinData> skinsUnlock;
    [SerializeField] private RectTransform unlockTagTf;
    [SerializeField] private RectTransform newTagTf;
    [SerializeField] private RectTransform effectTf;
    [SerializeField] private Image shadowTf;
    [SerializeField] private ButtonW2W tapToClose;
    [SerializeField] private TMP_Text tapToCloseTxt;
    [SerializeField] private SkinTrailFX skinTrailFX;

    private Sequence _showAnim;
    private Material _material;
    private int _currentSkinIndex;
    private Dictionary<string, SkinData> _skinDatas = null;
    private List<SkinTrailFX> _skinTrailFXs;

    public override void Initialize()
    {
        base.Initialize();
        tapToClose.onClick.AddListener(Hide);
        _material = skinImage.material;
        _skinTrailFXs = new List<SkinTrailFX>();
    }


    public override void Show(IUIData data = null)
    {
        base.Show(data);
        //if (data != null) OnShow((SkinData)data);
    }

    public void SetData(Dictionary<string, SkinData> skinDatas)
    {
        _skinDatas = skinDatas;
        OnShow();
    }

    private void OnShow()
    {
        SkinData data = _skinDatas.Values.ToArray()[_currentSkinIndex];
        skinImage.sprite = data.unlockSprite;
        shadowTf.sprite = data.unlockSprite;
        tapToCloseTxt.DOComplete();
        effectTf.transform.DOKill();
        tapToClose.targetGraphic.DOComplete();
        tapToCloseTxt.SetText(_currentSkinIndex < _skinDatas.Count - 1 ? "Tap to continue" : "Tap to close");

        skinImage.transform.localScale = Vector3.zero;
        effectTf.transform.localScale = Vector3.zero;
        effectTf.rotation = Quaternion.identity;
        unlockTagTf.transform.localScale = Vector3.zero;
        newTagTf.transform.localScale = Vector3.zero;
        tapToCloseTxt.transform.localScale = Vector3.zero;
        shadowTf.transform.localScale = Vector3.one * 2f;
        tapToCloseTxt.color = new Color32(255, 255, 255, 0);


        _showAnim = DOTween.Sequence();
        _showAnim.Append(newTagTf.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _showAnim.Append(skinImage.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        _showAnim.Append(skinImage.transform.DOShakeRotation(0.5f, 10, 10, 90, false));
        if (_material)
        {
            _material.SetFloat("_ShineLocation", 1);
            _showAnim.Append(_material.DOFloat(0, "_ShineLocation", 0.2f));
            _showAnim.Append(shadowTf.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack)
                .OnStart(() => { shadowTf.gameObject.SetActive(true); }).OnComplete(() =>
                {
                    shadowTf.gameObject.SetActive(false);
                }));
            _material.SetFloat("_GreyscaleBlend", 1);
            _showAnim.Append(_material.DOFloat(0, "_GreyscaleBlend", 0.2f));
        }

        _showAnim.Append(effectTf.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            effectTf.transform.DORotate(new Vector3(0, 0, 90), 1, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }));
        //_showAnim.Append(newTagTf.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _showAnim.Append(tapToCloseTxt.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _showAnim.OnComplete(() => { tapToCloseTxt.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo); });
    }

    public override void Hide()
    {
        if (_currentSkinIndex < _skinDatas.Count - 1)
        {
            _currentSkinIndex++;
            OnShow();
        }
        else
        {
            base.Hide();
            Timing.RunCoroutine(IEPlayAnimComplete());
        }
    }

    private IEnumerator<float> IEPlayAnimComplete()
    {
        yield return Timing.WaitForOneFrame;

        RectTransform target = UIManager.Instance.GetScreen<MainMenu>().GetInventoryButton();

        int count = 0;
        foreach (var skinData in _skinDatas)
        {
            if (_skinTrailFXs.Count > count)
            {
                _skinTrailFXs[count].Initialize(skinData.Value.unlockSprite, target);
            }
            else
            {
                SkinTrailFX fx = Instantiate(skinTrailFX, target.parent, false);
                fx.Initialize(skinData.Value.unlockSprite, target);
                _skinTrailFXs.Add(fx);
            }

            count++;
            yield return Timing.WaitForSeconds(0.1f);
        }
    }
}