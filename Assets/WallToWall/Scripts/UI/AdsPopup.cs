using TMPro;
using UnityEngine;

public class AdsPopup : BaseScreen
{
    [SerializeField] private ButtonW2W btnBuyRemoveAds;
    [SerializeField] private ButtonW2W btnClose;
    [SerializeField] private TMP_Text txtPrice;

    public override void Initialize()
    {
        base.Initialize();

        txtPrice.SetText($"Buy {IAPManager.Instance.GetLocalizedPrice(IAPManager.RemoveAds)}");
        IAPManager.Instance.OnBuyProductEvent += OnBuyProductEvent;

        btnBuyRemoveAds.onClick.AddListener(() => IAPManager.Instance.BuyProductID(IAPManager.RemoveAds));
        btnClose.onClick.AddListener(Hide);
    }

    private void OnBuyProductEvent(string productId, StateIAP arg2)
    {
        Debug.Log("OnBuyProductEvent " + productId + " " + arg2);

        if (productId == nameof(IAPType.remove_ads) && arg2 == StateIAP.Success)
        {
            NotifyScreen.Instance.ShowNotify("Ads removed successfully!");
            Hide();
        }
    }
}