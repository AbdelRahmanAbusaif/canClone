using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;

public class CheckAdTimer : MonoBehaviour
{
    private PlayerProfile playerProfile;
    private async void Awake() 
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

        CheckAds();
    }

    private async void CheckAds()
    {
        List<AdManager> adManagers = playerProfile.AdManager;

        foreach(var ad in adManagers)
        {
            DateTime expiredDate = DateTime.TryParse(ad.AdNextTimer , out DateTime date) ? date : DateTime.MinValue;
            TimeSpan timeSpan = ServerTimeManager.Instance.CurrentTime.Date - expiredDate.Date;

            if (timeSpan.Days >= 1)
            {
                ad.AdCurrentTimer = ad.AdNextTimer;
                ad.AdNextTimer = "";
                ad.AdCounter = 0;
            }
        }

        playerProfile.AdManager = adManagers;
        await CloudSaveManager.Instance.SaveDataAsync<PlayerProfile>("PlayerProfile", playerProfile);
    }

}
