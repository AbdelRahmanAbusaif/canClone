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
                await ChangeImage("PlayerCoverProfileImage", consumableType);
                break;
            case ConsumableType.PlayerProfileBorder:
                await ChangeImage("PlayerProfileBorder", consumableType);
                break;
        }

        await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);
    }

    private async Task ChangeImage(string key,ConsumableType consumableType)
    {
        await CloudSaveManager.Instance.SaveImageAsync(key, itemImage.sprite.texture);
        OnImageChanged?.Invoke(consumableType, itemImage.sprite);
    }

    public async void SetItemData(string itemId, string name)
    {
        // if(!AuthenticationService.Instance.IsSignedIn)
        // {
        //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //     Debug.Log("Signed in anonymously");
        // }

        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        
        var item = playerProfile.ContainerProfileAvatarImages.Find(x => x.Id == itemId);
        Debug.Log("Item Id: " + item.Id);
        if (item != null)
        {
            Debug.Log("Item found");
            Debug.Log("Item name: " + item.ConsumableName);
            itemName.text = name;
            CloudSaveManager.Instance.LoadImageAsync(item.Id, itemImage, false);
        }
        else 
        {
            Debug.LogError("Item not found");
        }

    }
}
