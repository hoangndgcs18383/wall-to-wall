using System.Collections.Generic;
using DG.Tweening;
using Hzeff.Events;
using MEC;
using TMPro;
using UnityEngine;

public class AdsPopup : BaseScreen
{
    [SerializeField] private ButtonW2W btnBuyRemoveAds;
    [SerializeField] private ButtonW2W btnClose;

    [SerializeField] private GameObject removedAdsGO;

    [SerializeField] private TMP_Text txtPrice;
    [SerializeField] private TMP_Text txtAds;
    [SerializeField] private TMP_Text thankAdsText;

    DOTweenTMPAnimator animator;

    public override void Initialize()
    {
        base.Initialize();

        if (IAPManager.Instance.IsRemoveAdsPurchased())
        {
            OnRemoveAdsComplete();
        }
        else
        {
            removedAdsGO.SetActive(false);
            txtAds.gameObject.SetActive(true);
            txtPrice.SetText($"Buy {IAPManager.Instance.GetLocalizedPrice(IAPManager.RemoveAds)}");
            IAPManager.Instance.OnBuyProductEvent += OnBuyProductEvent;
            btnBuyRemoveAds.onClick.AddListener(() => IAPManager.Instance.BuyProductID(IAPManager.RemoveAds));
        }

        btnClose.onClick.AddListener(Hide);
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        if (IAPManager.Instance.IsRemoveAdsPurchased())
        {
            Timing.RunCoroutine(IEShowTextAnimations());
        }
    }

    private IEnumerator<float> IEShowTextAnimations()
    {
        animator = new DOTweenTMPAnimator(thankAdsText);

        for (int i = 0; i < animator.textInfo.characterCount; ++i)
        {
            Vector3 currCharOffset = animator.GetCharOffset(i);
            animator.DOOffsetChar(i, currCharOffset + new Vector3(0, 10, 0), 1).SetLoops(-1, LoopType.Yoyo);
            //do color yellow
            animator.DOColorChar(i, Color.yellow, 1).SetLoops(-1, LoopType.Yoyo);
            animator.DOFadeChar(i, 1, 1).SetLoops(-1, LoopType.Yoyo);
            //loop
            yield return Timing.WaitForSeconds(0.05f);
        }
    }

    private void OnBuyProductEvent(string productId, StateIAP arg2)
    {
        Debug.Log("OnBuyProductEvent " + productId + " " + arg2);

        if (productId == nameof(IAPType.remove_ads) && arg2 == StateIAP.Success)
        {
            IAPManager.Instance.SetRemoveAdsPurchased();
            EventDispatcher<MainMenuEvent>.Dispatch(new MainMenuEvent());
            OnRemoveAdsComplete();
            Hide();
        }
    }

    private void OnRemoveAdsComplete()
    {
        txtPrice.SetText("Purchased!");
        btnBuyRemoveAds.targetGraphic.raycastTarget = false;
        btnBuyRemoveAds.targetGraphic.color = new Color(0.5f, 0.5f, 0.5f, 1);
        removedAdsGO.SetActive(true);
        txtAds.gameObject.SetActive(false);
    }
}