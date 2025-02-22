using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameVanilla.Game.Common;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanelUI : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image itemImage;

    private int price;
    private StoreItem storeItem;
    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }


    public void SetItemDetails(StoreItem item, Image image)
    {
        storeItem = item;

        itemNameText.text = item.Title;
        itemPriceText.text = item.Price;
        price = int.Parse(item.Price);
        itemImage = image;
    }
    private async void OnBuyButtonClicked()
    {
        PuzzleMatchManager.instance.coinsSystem.SpendCoins(price);
        PlayerProfile playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        
        switch (storeItem.Type)
        {
            case StoreItem.ItemType.ProfileImage:
                Add(playerProfile.ContainerProfileImages);
                break;
            case StoreItem.ItemType.BorderImage:
                Add(playerProfile.ContainerProfileImages);
                break;
        }

        await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);
        Debug.Log("Buy button clicked");
    }

    private void Add(List<string> data)
    {
        if (data.Contains(GetItemData()))
        {
            Debug.Log("Item already owned");
            return;
        }
        data.Add(GetItemData());;
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