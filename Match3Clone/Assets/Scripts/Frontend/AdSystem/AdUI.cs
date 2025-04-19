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
    private void OnEnable() 
    {
        ApplyRemoteConfig();
    }

    public async void ApplyRemoteConfig()
    {
        Debug.Log("Remote Config Fetched Successfully! (AdUI)");
        Debug.Log("Name of this Object " + gameObject.name);
        adManagers = await LocalSaveManager.Instance.LoadDataAsync<List<AdManager>>("AdManager");

        if(adData.Count == 0)
        {
            var jsonData = RemoteConfigService.Instance.appConfig.GetJson("AdRewards");
            adData = JsonConvert.DeserializeObject<List<AdData>>(jsonData);
            Debug.Log("Ad Data Loaded from Remote Config: " + jsonData);
        }
        else
        {
            adUIComponents.Clear();
            foreach(var child in adContainer.GetComponentsInChildren<Transform>())
            {
                if(child != adContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        Debug.Log($"Ad Data Count: {adData.Count}");
        for (int i = 0; i < adData.Count; i++)
        {
            AdData ad = adData[i];
            AdManager adManager;
            if (adManagers.Any(x => x.AdId == ad.AdId))
            {
                Debug.Log($"Ad Found With ID {ad.AdId}");

                adManager = adManagers.FirstOrDefault(x => x.AdId == ad.AdId);

                var adUI =  Instantiate(adPrefabs, adContainer); 
                adUI.GetComponent<AdUIComponent>().SetAdData(ad.AdId,ad.AdClicksGoal.ToString(), adManager.AdCounter.ToString(), ad.AdReward.ToString());
                adUIComponents.Add(adUI);

                continue;
            }
            adManager = new AdManager
            {
                AdId = ad.AdId,
                AdCounter = 0,
                AdCurrentTimer = "",
                AdNextTimer = ""
            };

            var adUIClone =  Instantiate(adPrefabs, adContainer); 
            adUIClone.GetComponent<AdUIComponent>().SetAdData(ad.AdId,ad.AdClicksGoal.ToString(), adManager.AdCounter.ToString(), ad.AdReward.ToString());
            adUIComponents.Add(adUIClone);
            
            adManagers.Add(adManager);

            Debug.Log("Ad Added");
        }
        await CloudSaveManager.Instance.SaveDataAsync("AdManager", adManagers);
    }
}
[Serializable]
public class AdData
{
    public string AdId;
    public int AdReward;
    public int AdClicksGoal;
}
