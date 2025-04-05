using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileInformationPage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerId;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerEmail;
    [SerializeField] private TextMeshProUGUI playerPhoneNumber;

    [SerializeField] private Image playerImage;
    [SerializeField] private Image playerCoverProfileImage;
    [SerializeField] private Image playerBorderImage;
    private PlayerProfile playerProfile;
    private async void OnEnable() 
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

        PlayerId.text = playerProfile.PlayerId;
        playerName.text = playerProfile.PlayerName;
        playerLevel.text = playerProfile.Level.ToString();
        playerEmail.text = playerProfile.Email;
        playerPhoneNumber.text = playerProfile.PhoneNumber;

        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileImage", playerImage);
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileCoverImage", playerCoverProfileImage);
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileBorderImage", playerBorderImage);
    }
}
