using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SkinData : IUIData
{
    public string key;
    public bool isUnlock;
    public Sprite sprite;
    public int unlockPoint;
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

    private Sequence _showAnim;
    private Material _material;

    public override void Initialize()
    {
        base.Initialize();
        tapToClose.onClick.AddListener(Hide);
        _material = skinImage.material;
    }


    public override void Show(IUIData data = null)
    {
        base.Show(data);
        if (data != null) OnShow((SkinData)data);
    }

    private void OnShow(SkinData data)
    {
        skinImage.sprite = data.sprite;
        shadowTf.sprite = data.sprite;
        
        skinImage.transform.localScale = Vector3.zero;
        effectTf.transform.localScale = Vector3.zero;
        effectTf.rotation = Quaternion.identity;
        unlockTagTf.transform.localScale = Vector3.zero;
        newTagTf.transform.localScale = Vector3.zero;
        tapToCloseTxt.transform.localScale = Vector3.zero;
        shadowTf.transform.localScale = Vector3.one * 2f;
        tapToCloseTxt.color = new Color32(255, 255, 255, 255);

        effectTf.DOKill();
        tapToClose.targetGraphic.DOKill();
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
}