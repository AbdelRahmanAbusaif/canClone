using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SaveData;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UIElements;

public class AdUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI adTimerText;
    [SerializeField] private Transform adContainer;
    [SerializeField] private GameObject adPrefabs;
    [SerializeField] private List<AdData> adData;
    [SerializeField] private List<GameObject> adUIComponents;
    private int adCounter = 0;
    private DateTime adTimer;
    private List<AdManager> adManagers;
    private TimeSpan adTimerSpan = new TimeSpan(0, 0, 0);

    private async void Awake()
    {
        if(!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public async void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully! (AdUI)");
        Debug.Log("Name of this Object " + gameObject.name);
        adManagers = await LocalSaveManager.Instance.LoadDataAsync<List<AdManager>>("AdManagers");

        var jsonData = RemoteConfigService.Instance.appConfig.GetJson("AdRewards");
        adData = JsonConvert.DeserializeObject<List<AdData>>(jsonData);

        Debug.Log($"Ad Data Count: {adData.Count}");
        for (int i = 0; i < adData.Count; i++)
        {
            AdData ad = adData[i];
            AdManager adManager;
            if (adManagers.Any(x => x.AdId == ad.AdId))
            {
                Debug.Log($"Ad Found With ID {ad.AdId}");

                adManager = adManagers.FirstOrDefault(x => x.AdId == ad.AdId);

                Instantiate(adPrefabs, adContainer).GetComponent<AdUIComponent>().SetAdData(ad.AdId,ad.AdClicksGoal.ToString(), adManager.AdCounter.ToString(), ad.AdReward.ToString());
                adUIComponents.Add(adPrefabs);

                continue;
            }
            adManager = new AdManager
            {
                AdId = ad.AdId,
                AdCounter = 0,
                AdCurrentTimer = "",
                AdNextTimer = ""
            };

            Instantiate(adPrefabs, adContainer).GetComponent<AdUIComponent>().SetAdData(ad.AdId,ad.AdClicksGoal.ToString(), adManager.AdCounter.ToString(), ad.AdReward.ToString());
            adUIComponents.Add(adPrefabs);
            
            adManagers.Add(adManager);

            Debug.Log("Ad Added");
        }
        await CloudSaveManager.Instance.SaveDataAsync("AdManagers", adManagers);
    }
}
[Serializable]
public class AdData
{
    public string AdId;
    public int AdReward;
    public int AdClicksGoal;
}
