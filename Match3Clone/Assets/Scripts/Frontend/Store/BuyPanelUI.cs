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
    [SerializeField] private List<Bundle> bundles;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemImage;
    [SerializeField] private AnimationBox animationBox;
    private StoreItem storeItem;
    private void Start()
    {
        foreach (var bundle in bundles)
        {
            switch(bundle.DurationType)
            {
                case Duration.OneDay:
                    bundle.buyButton.onClick.AddListener(OnBuyOneDayButtonClicked);
                    break;
                case Duration.SevenDays:
                    bundle.buyButton.onClick.AddListener(OnBuySevenDaysButtonClicked);
                    break;
                case Duration.ThirtyDays:
                    bundle.buyButton.onClick.AddListener(OnBuyThirtyDaysButtonClicked);
                    break;
            }
        }
    }
    public void SetItemDetails(StoreItem item, Image image)
    {
        storeItem = item;

        itemNameText.text = item.Title;

        foreach(var bundle in bundles)
        {
            switch(bundle.DurationType)
            {
                case Duration.OneDay:
                    bundle.piceText.text = item.PriceForOneDay;
                    bundle.price = int.Parse(item.PriceForOneDay);
                    break;
                case Duration.SevenDays:
                    bundle.piceText.text = item.PriceForSevenDay;
                    bundle.price = int.Parse(item.PriceForSevenDay);
                    break;
                case Duration.ThirtyDays:
                    bundle.piceText.text = item.PriceForThirtyDay;
                    bundle.price = int.Parse(item.PriceForThirtyDay);
                    break;
            }
        }

        itemImage = image;
    }
    private async void OnBuyOneDayButtonClicked()
    {
        var bundle = bundles.FirstOrDefault(x => x.DurationType == Duration.OneDay);
        await BuyItemWithCoin(bundle.price, ServerTimeManager.Instance.CurrentTime.AddDays(1).ToString());
    }
    private async void OnBuySevenDaysButtonClicked()
    {        
        var bundle = bundles.FirstOrDefault(x => x.DurationType == Duration.OneDay);
        await BuyItemWithCoin(bundle.price , ServerTimeManager.Instance.CurrentTime.AddDays(7).ToString());
    }
    private async void OnBuyThirtyDaysButtonClicked()
    {
        var bundle = bundles.FirstOrDefault(x => x.DurationType == Duration.OneDay);
        await BuyItemWithCoin(bundle.price , ServerTimeManager.Instance.CurrentTime.AddDays(30).ToString());
    }

    private async System.Threading.Tasks.Task BuyItemWithCoin(int price, string duration)
    {
        PuzzleMatchManager.instance.coinsSystem.SpendCoins(price);
        PlayerProfile playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

        switch (storeItem.Type)
        {
            case StoreItem.ItemType.ProfileImage:
                Add(playerProfile.ContainerProfileAvatarImages, duration);
                break;
            case StoreItem.ItemType.BorderImage:
                Add(playerProfile.ContainerProfileBorders, duration);
                break;
            case StoreItem.ItemType.CoverProfileImage:
                Add(playerProfile.ContainerProfileCoverImages, duration);
                break;
        }

        await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);
        Debug.Log("Buy button clicked");
    }

    private async void Add(List<ConsumableItem> data , string duration)
    {
        ConsumableItem consumableItem = new ConsumableItem
        {
            Id = storeItem.Id,
            ConsumableName = storeItem.Title,
            //this will be updated
        };


        // check if the item is already owned by the player
        // if owned then update the date purchased and date expired
        if (data.Select(x => x.Id).Contains(consumableItem.Id))
        {
            //item already owned
            //then update the date purchased and date expired
            consumableItem = data.FirstOrDefault(x => x.Id == consumableItem.Id);
            consumableItem.DatePurchased  = ServerTimeManager.Instance.CurrentTime.ToString();

            DateTime durationDate = DateTime.Parse(duration);
            int durationDays = durationDate.Day; // Extract the day component
            DateTime dateTime = DateTime.Parse(consumableItem.DateExpired);
            consumableItem.DateExpired = dateTime.AddDays(durationDays).ToString();

            Debug.Log("Item already owned");

            data.Remove(data.FirstOrDefault(x => x.Id == consumableItem.Id));
        }
        else
        {
            // item not owned
            // then add the item to the list
            Debug.Log("Adding item to the list");
            
            consumableItem.DatePurchased = ServerTimeManager.Instance.CurrentTime.ToString();
            consumableItem.DateExpired = duration;
        }

        data.Add(consumableItem);
        await CloudSaveManager.Instance.SaveDataAsyncString(consumableItem.Id, GetItemData());

        animationBox.OnClose();
    }

    public void OnClose()
    {
        Invoke(nameof(DestroyPanel), 1f);
    }

    private void DestroyPanel()
    {
        foreach (var bundle in bundles)
        {
            bundle.buyButton.onClick.RemoveAllListeners();
        }

        Destroy(gameObject);
    }
    private string GetItemData()
    {
        var texture = itemImage.sprite.texture;
        byte[] itemImageData = texture.EncodeToPNG();
        return System.Convert.ToBase64String(itemImageData);
    }
}
[Serializable]
public class Bundle
{
    public TextMeshProUGUI piceText;
    public Button buyButton;
    public int price;
    public string duration;
    public Duration DurationType;
}
public enum Duration
{
    OneDay,
    SevenDays,
    ThirtyDays
}