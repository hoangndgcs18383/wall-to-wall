using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

public enum IAPType
{
    remove_ads
}

public enum StateIAP
{
    Success,
    Fail,
    Cancel
}

public class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
{
    private IStoreController _storeController;
    private IExtensionProvider _storeExtensionProvider;

    public Action<string, StateIAP> OnBuyProductEvent;
    public const string RemoveAds = "remove_ads";

    /*private void Start()
    {
        InitializedIAP();
    }*/

    private bool IsInitializedIAP()
    {
        return _storeController != null && _storeExtensionProvider != null;
    }

    public void InitializedIAP()
    {
        if (IsInitializedIAP()) return;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(RemoveAds, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
        SetRemoveAdsNoPurchased();
    }

    private void SetRemoveAdsNoPurchased()
    {
#if UNITY_EDITOR
        //SaveSystem.Instance.SetInt(RemoveAds, 0);
#endif
    }

    public void SetRemoveAdsPurchased()
    {
        SaveSystem.Instance.SetInt(RemoveAds, 1);
    }

    public bool IsRemoveAdsPurchased()
    {
        return SaveSystem.Instance.GetInt(RemoveAds, 0) == 1;
    }

    public void BuyProductID(string productId)
    {
#if UNITY_EDITOR
        OnBuyProductEvent?.Invoke(productId, StateIAP.Success);

#else
            if (IsInitializedIAP())
            {
                var product = _storeController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    _storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
                OnBuyProductEvent?.Invoke(productId, StateIAP.Fail);
            }
#endif
    }

    #region Callback

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error + " message:" + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log("ProcessPurchase: " + purchaseEvent.purchasedProduct.definition.id);

        bool validPurchase = true;

        var validator =
            new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

        try
        {
            var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);

            foreach (IPurchaseReceipt product in result)
            {
                Debug.Log(product.productID);
                Debug.Log(product.purchaseDate);
                Debug.Log(product.transactionID);
            }
        }
        catch (Exception e)
        {
            validPurchase = false;
        }

        OnBuyProductEvent?.Invoke(purchaseEvent.purchasedProduct.definition.id,
            validPurchase ? StateIAP.Success : StateIAP.Fail);

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError(
            $"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
        OnBuyProductEvent?.Invoke(product.definition.id, StateIAP.Fail);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        _storeExtensionProvider = extensions;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError(
            $"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureDescription}");
        OnBuyProductEvent?.Invoke(product.definition.id, StateIAP.Fail);
    }

    public string GetLocalizedPrice(string productId)
    {
        if (IsInitializedIAP())
        {
            var product = _storeController.products.WithID(productId);
            if (product != null)
            {
                return product.metadata.localizedPriceString;
            }
        }

#if UNITY_EDITOR
        return "1.99$";
#endif
        return string.Empty;
    }

    #endregion
}