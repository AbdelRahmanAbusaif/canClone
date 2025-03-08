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
        LocalSaveManager.Instance.LoadImageAsync("PlayerCoverProfileImage", playerCoverProfileImage);
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileBorder", playerBorderImage);
    }
}