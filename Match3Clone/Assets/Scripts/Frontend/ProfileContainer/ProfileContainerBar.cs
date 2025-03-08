using System.Threading.Tasks;
using SaveData;
using UnityEngine;
using UnityEngine.UI;

public class ProfileContainerBar : MonoBehaviour
{
    [SerializeField] private Image borderImage;
    [SerializeField] private Image avatarImage;

    private PlayerProfile playerProfile;

    private async void OnEnable() 
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileImage", avatarImage);
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileBorder", borderImage);
    }
}
