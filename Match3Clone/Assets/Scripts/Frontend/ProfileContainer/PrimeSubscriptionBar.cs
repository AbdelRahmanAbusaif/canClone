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
        
        DateTime expiredDate = DateTime.Parse(playerProfile.PrimeSubscriptions.DateExpired);

        Debug.Log("Expired Date :"+expiredDate);

        if(expiredDate < DateTime.Now)
        {
            durationText.text = "0";
        }
        else 
        {
            var timeSpan =  expiredDate.Date - DateTime.Now.Date;
            print(timeSpan.Days);
            durationText.text = timeSpan.Days.ToString();
        }
    }
}
