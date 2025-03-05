using System;
using SaveData;
using TMPro;
using UnityEngine;

public class PrimeSubscriptionBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI durationText;

    private PlayerProfile playerProfile;
    private async void OnEnable() 
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        
        TimeSpan timeSpan = DateTime.Parse(playerProfile.PrimeSubscriptions.DateExpired) - DateTime.Now;
        durationText.text = timeSpan.Days.ToString();
    }
}
