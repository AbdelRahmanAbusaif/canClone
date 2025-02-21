using System;
using GameVanilla.Game.Common;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class ShopManager : MonoBehaviour , IDetailedStoreListener
{
    public static ShopManager Instance{get; private set;}
    public ConsumableItem GoldAmount;
    public IStoreController storeController;
    private int amount;

    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }    
    }
    private void Start() 
    {
        InitializePurchasing();
    }
    async void InitializePurchasing()
    {
        await UnityServices.InitializeAsync();

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(GoldAmount.id, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
    }
    public void InitBuyCoin(long amount)
    {
        this.amount = (int)amount;
        BuyCoin();
    }
    private void BuyCoin()
    {
        storeController.InitiatePurchase(GoldAmount.id);
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        if (product.definition.id == GoldAmount.id)
        {
            Debug.Log("Gold Purchased");
            PuzzleMatchManager.instance.coinsSystem.BuyCoins(amount);
            // PuzzleMatchManager.instance.coinsSystem.Subscribe(numCoins => OnCoinsChanged((int)numCoins));
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";
        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }
        Debug.Log(errorMessage);
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }
}
[Serializable]
public class ConsumableItem
{
    public string id;
    public string name;
    public string description;
    public int price;
}