using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockSkinPanel : MonoBehaviour
{
    [SerializeField] private Image skinImage;
    [SerializeField] private List<Sprite> skinsUnlock;
    [SerializeField] private RectTransform unlockTagTf;
    [SerializeField] private RectTransform newTagTf;
    [SerializeField] private RectTransform effectTf;
    [SerializeField] private ButtonW2W tapToClose;
    [SerializeField] private TMP_Text tapToCloseTxt;

    private Sequence _showAnim;

    private void Start()
    {
        tapToClose.onClick.AddListener(Hide);
    }

    private void OnShow(int skinIndex)
    {
        skinImage.sprite = skinsUnlock[skinIndex];

        skinImage.transform.localScale = Vector3.zero;
        effectTf.transform.localScale = Vector3.zero;
        unlockTagTf.transform.localScale = Vector3.zero;
        newTagTf.transform.localScale = Vector3.zero;
        tapToCloseTxt.transform.localScale = Vector3.zero;
        tapToCloseTxt.color = new Color32(255, 255, 255, 255);

        effectTf.DOKill();
        tapToClose.targetGraphic.DOKill();
        _showAnim?.Kill();
        _showAnim = DOTween.Sequence();
        _showAnim.Append(unlockTagTf.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _showAnim.Append(skinImage.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        _showAnim.Append(effectTf.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            effectTf.rotation = Quaternion.identity;
            effectTf.transform.DORotate(new Vector3(0, 0, 90), 1, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }));
        _showAnim.Append(newTagTf.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _showAnim.Append(tapToCloseTxt.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        _showAnim.OnComplete(() =>
        {
            tapToCloseTxt.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        });
    }

    private void OnHide()
    {
        unlockTagTf.gameObject.SetActive(false);
        newTagTf.gameObject.SetActive(false);
        effectTf.gameObject.SetActive(false);
    }

    [Button]
    public void Show()
    {
        gameObject.SetActive(true);
        OnShow(1);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}