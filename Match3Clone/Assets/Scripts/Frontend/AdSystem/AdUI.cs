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
    private TimeSpan adTimerSpan = new TimeSpan(0, 0, 0);

    private PlayerProfile playerProfile;
    private async void Awake()
    {
        if(!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public async void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully! (AdUI)");
        Debug.Log("Name of this Object " + gameObject.name);
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");   
        
        var jsonData = RemoteConfigService.Instance.appConfig.GetJson("AdRewards");
        adData = JsonConvert.DeserializeObject<List<AdData>>(jsonData);

        Debug.Log($"Ad Data Count: {adData.Count}");
        for (int i = 0; i < adData.Count; i++)
        {
            AdData ad = adData[i];
            AdManager adManager;
            if (playerProfile.AdManager.Any(x => x.AdId == ad.AdId))
            {
                Debug.Log($"Ad Found With ID {ad.AdId}");

                adManager = playerProfile.AdManager.FirstOrDefault(x => x.AdId == ad.AdId);

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
            
            playerProfile.AdManager.Add(adManager);

            Debug.Log("Ad Added");
        }
        await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);
    }
}
[Serializable]
public class AdData
{
    public string AdId;
    public int AdReward;
    public int AdClicksGoal;
}
