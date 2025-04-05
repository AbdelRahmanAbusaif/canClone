using System;
using System.Threading.Tasks;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEditUI : MonoBehaviour
{
    public static event Action<ConsumableType,Sprite> OnImageChanged;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI expiredText;
    [SerializeField] private Button itemButton;
    [SerializeField] private ConsumableType consumableType;

    private PlayerProfile playerProfile;
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

        await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);
    }

    private async Task ChangeImage(string key,ConsumableType consumableType)
    {
        await CloudSaveManager.Instance.SaveImageAsync(key, itemImage.sprite.texture);
        OnImageChanged?.Invoke(consumableType, itemImage.sprite);
    }

    public async void SetItemData(string itemId, string name, ConsumableType consumableType)
    {
        // if(!AuthenticationService.Instance.IsSignedIn)
        // {
        //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //     Debug.Log("Signed in anonymously");
        // }

        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        ConsumableItem item = null;
        switch(consumableType)
        {
            case ConsumableType.PlayerProfileAvatar:
            item = playerProfile.ContainerProfileAvatarImages.Find(x => x.Id == itemId);
            break;
            case ConsumableType.PlayerProfileCover:
            item = playerProfile.ContainerProfileCoverImages.Find(x => x.Id == itemId);
            break;
            case ConsumableType.PlayerProfileBorder:
            item = playerProfile.ContainerProfileBorders.Find(x => x.Id == itemId);
            break;
        }

        if (item != null)
        {
            Debug.Log("Item found");
            Debug.Log("Item name: " + item.ConsumableName);
            itemName.text = name;

            DateTime expirationDate = DateTime.Parse(item.DateExpired);
            var timeLeft = expirationDate - DateTime.Now;
            
            expiredText.text = timeLeft.Days.ToString();

            CloudSaveManager.Instance.LoadImageAsync(item.Id, itemImage, false);
        }
        else 
        {
            Debug.LogError("Item not found");
        }

    }
}
