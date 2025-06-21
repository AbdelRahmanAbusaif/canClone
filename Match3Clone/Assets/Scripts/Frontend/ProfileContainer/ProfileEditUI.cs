using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEditUI : MonoBehaviour
{
    public static event Action<ConsumableType,Sprite> OnImageChanged;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image textContainer;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI expiredText;
    [SerializeField] private Button itemButton;
    [SerializeField] private ConsumableType consumableType;
    [SerializeField] private GameObject loadingSpinner;

    private void Start() 
    {
        itemButton.onClick.AddListener(OnItemButtonClicked);
    }

    private async void OnItemButtonClicked()
    {
        switch(consumableType)
        {
            case ConsumableType.PlayerProfileAvatar:
                await ChangeImage("PlayerProfileImage", consumableType);
                break;
            case ConsumableType.PlayerProfileCover:
                await ChangeImage("PlayerProfileCoverImage", consumableType);
                break;
            case ConsumableType.PlayerProfileBorder:
                await ChangeImage("PlayerProfileBorderImage", consumableType);
                break;
        }
    }

    private async Task ChangeImage(string key,ConsumableType consumableType)
    {
        loadingSpinner.SetActive(true);
        await CloudSaveManager.Instance.SaveImageAsync(key, itemImage.sprite.texture);
        OnImageChanged?.Invoke(consumableType, itemImage.sprite);
        loadingSpinner.SetActive(false);
    }

    public async void SetItemData(string itemId, string name, ConsumableType consumableType)
    {
        // if(!AuthenticationService.Instance.IsSignedIn)
        // {
        //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //     Debug.Log("Signed in anonymously");
        // }

        var containerProfileAvatarImages = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileAvatarImages");
        var containerProfileCoverImages = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileCoverImages");
        var containerProfileBorders = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileBorders");

        ConsumableItem item = null;
        switch(consumableType)
        {
            case ConsumableType.PlayerProfileAvatar:
                item = containerProfileAvatarImages.Find(x => x.Id == itemId);
                break;
            case ConsumableType.PlayerProfileCover:
                item = containerProfileCoverImages.Find(x => x.Id == itemId);
                break;
            case ConsumableType.PlayerProfileBorder:
                item = containerProfileBorders.Find(x => x.Id == itemId);
                break;
        }

        if (item != null)
        {
            Debug.Log("Item found");
            Debug.Log("Item name: " + item.ConsumableName);
            itemName.text = name;

            DateTime expirationDate = DateTime.Parse(item.DateExpired);
            var timeLeft = expirationDate.Date - DateTime.Now.Date;

            if(timeLeft.Days <= 0)
            {
                timeLeft = expirationDate - ServerTimeManager.Instance.CurrentTime;
                expiredText.text = timeLeft.Hours.ToString() + " H";
            }
            else if(timeLeft.Days >= 1000f)
            {
                expiredText.text = "";
                textContainer.gameObject.SetActive(false);

            }
            else
            {
                expiredText.text = timeLeft.Days.ToString() + " D";
            }

            CloudSaveManager.Instance.LoadImageAsync(item.Id, itemImage, false);
        }
        else 
        {
            Debug.LogError("Item not found");
        }

    }
}
