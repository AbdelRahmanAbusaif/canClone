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

        if (adManagers != null)
        {
            Debug.Log("Ad Managers data loaded successfully, checking ad timers.");
			CheckAds();
        }
		else
		{
			Debug.LogWarning("Ad Managers data not found, initializing with empty list.");
			adManagers = new List<AdManager>();
		}
	}

    private async void CheckAds()
    {
        foreach(var ad in adManagers)
        {
            DateTime expiredDate = DateTime.TryParse(ad.AdNextTimer , out DateTime date) ? date : DateTime.MinValue;
            TimeSpan timeSpan = ServerTimeManager.Instance.CurrentTime.Date - expiredDate.Date;

            Debug.Log($"Checking ad {ad.AdId} with current timer: {ad.AdCurrentTimer}, next timer: {ad.AdNextTimer}, expired date: {expiredDate}, time span: {timeSpan}");
			if (timeSpan.Days >= 0)
            {
				Debug.Log($"Ad {ad.AdId} timer expired. Resetting ad counter and timers.");
				ad.AdCurrentTimer = ad.AdNextTimer;
                ad.AdNextTimer = "";
                ad.AdCounter = 0;
            }
        }

        Debug.Log("Saving updated Ad Managers data after checking timers.");
		await CloudSaveManager.Instance.SaveDataAsync<List<AdManager>>("AdManager", adManagers);
    }

}
