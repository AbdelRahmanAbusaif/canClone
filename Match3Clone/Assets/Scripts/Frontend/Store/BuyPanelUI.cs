using System;
using System.Collections.Generic;
using System.Linq;
using GameVanilla.Game.Common;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanelUI : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceOneDayText;
    [SerializeField] private TextMeshProUGUI itemPriceSevenDayText;
    [SerializeField] private TextMeshProUGUI itemPriceThirtyDayText;
    [SerializeField] private Image itemImage;

    private int priceForOneDay;
    private int priceForSevenDays;
    private int priceForThirtyDays;
    private StoreItem storeItem;
    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }
    public void SetItemDetails(StoreItem item, Image image)
    {
        storeItem = item;

        itemNameText.text = item.Title;

        itemPriceOneDayText.text = item.PriceForOneDay;
        itemPriceSevenDayText.text = item.PriceForSevenDay;
        itemPriceThirtyDayText.text = item.PriceForThirtyDay;

        priceForOneDay = int.Parse(item.PriceForOneDay);
        priceForSevenDays = int.Parse(item.PriceForSevenDay);
        priceForThirtyDays = int.Parse(item.PriceForThirtyDay);

        itemImage = image;
    }
    private async void OnBuyButtonClicked()
    {
        PuzzleMatchManager.instance.coinsSystem.SpendCoins(priceForOneDay);
        PlayerProfile playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        
        switch (storeItem.Type)
        {
            case StoreItem.ItemType.ProfileImage:
                Add(playerProfile.ContainerProfileAvatarImages);
                break;
            case StoreItem.ItemType.BorderImage:
                Add(playerProfile.ContainerProfileBorders);
                break;
        }

        await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);
        Debug.Log("Buy button clicked");
    }

    private async void Add(List<ConsumableItem> data)
    {
        ConsumableItem consumableItem = new ConsumableItem
        {
            ConsumableName = storeItem.Title,
            //this will be updated
        };
        if (data.Select(x => x.ConsumableName).Contains(consumableItem.ConsumableName))
        {
            //item already owned
            //then update the date purchased and date expired
            Debug.Log("Item already owned");
            return;
        }
        Debug.Log("Adding item to the list");
        
        consumableItem.DatePurchased = ServerTimeManager.Instance.CurrentTime.ToString();
        consumableItem.DateExpired = ServerTimeManager.Instance.CurrentTime.AddDays(30).ToString();

        data.Add(consumableItem);
        await CloudSaveManager.Instance.SaveDataAsyncString(storeItem.Title.Replace(" ", String.Empty), GetItemData());
    }

    public void OnClose()
    {
        Invoke(nameof(DestroyPanel), 1f);
    }

    private void DestroyPanel()
    {
        Destroy(gameObject);
    }
    private string GetItemData()
    {
        var texture = itemImage.sprite.texture;
        byte[] itemImageData = texture.EncodeToPNG();
        return System.Convert.ToBase64String(itemImageData);
    }
}