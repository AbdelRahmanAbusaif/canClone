using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;

public class CheckAdTimer : MonoBehaviour
{
    private List<AdManager> adManagers;
    private async void Awake() 
    {
        adManagers = await LocalSaveManager.Instance.LoadDataAsync<List<AdManager>>("AdManager");

        CheckAds();
    }

    private async void CheckAds()
    {
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

        await CloudSaveManager.Instance.SaveDataAsync<List<AdManager>>("AdManagers", adManagers);
    }

}
