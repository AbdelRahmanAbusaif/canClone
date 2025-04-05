using System;
using System.Threading.Tasks;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemInformationPage : MonoBehaviour
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Image playerCoverProfileImage;
    [SerializeField] private Image playerBorderImage;

    [SerializeField] private TextMeshProUGUI playerName;
    private PlayerProfile playerProfile;
    private async void OnEnable() 
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        playerName.text = playerProfile.PlayerName;

        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileImage", playerImage);
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileCoverImage", playerCoverProfileImage);
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileBorderImage", playerBorderImage);

        ProfileEditUI.OnImageChanged += OnImageChanged;
    }

    private void OnImageChanged(ConsumableType type, Sprite sprite)
    {
        switch(type)
        {
            case ConsumableType.PlayerProfileAvatar:
                playerImage.sprite = sprite;
                break;
            case ConsumableType.PlayerProfileCover:
                playerCoverProfileImage.sprite = sprite;
                break;
            case ConsumableType.PlayerProfileBorder:
                playerBorderImage.sprite = sprite;
                break;
        }
    }
    private void OnDisable() 
    {
        ProfileEditUI.OnImageChanged -= OnImageChanged;
    }
}