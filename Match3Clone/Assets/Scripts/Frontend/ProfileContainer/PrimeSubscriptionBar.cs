using System;
using System.Threading.Tasks;
using SaveData;
using TMPro;
using UnityEngine;

public class PrimeSubscriptionBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI durationText;
    private PlayerProfile playerProfile;
    private async void OnEnable()

    {
        PrimeSubscriptionPanel.OnSubscriptionPurchased += OnPrimeSubscriptionPurchased;
        await UpdateUI();
    }

    private async System.Threading.Tasks.Task UpdateUI()
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

        DateTime expiredDate = DateTime.TryParse(playerProfile.PrimeSubscriptions.DateExpired , out DateTime date) ? date : DateTime.MinValue;

        Debug.Log("Expired Date :" + expiredDate);

        if (expiredDate < ServerTimeManager.Instance.CurrentTime)
        {
            durationText.text = "0";
        }
        else
        {
            var timeSpan = expiredDate.Date - ServerTimeManager.Instance.CurrentTime.Date;
            print(timeSpan.Days);
            durationText.text = timeSpan.Days.ToString();
        }
    }


    private async void OnPrimeSubscriptionPurchased()
    {
        await UpdateUI();
    }

}
