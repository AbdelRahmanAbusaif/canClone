using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.IO;
using Unity.Services.RemoteConfig;
using Newtonsoft.Json;

public class AirshipAdManager : MonoBehaviour
{
    // public static Action<>
    private Queue<AirShipAd> airShipAds = new Queue<AirShipAd>();
    [SerializeField] private Queue<AirAdComponent> waitingAds = new();
    [SerializeField] private GameObject adPrefab;

    [SerializeField] private static AirAdComponent airAdComponent = null;
    public bool isFirstTime = true;
    private void Start()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "AirshipAdConfig.json")))
        {
            var jsonFile = File.ReadAllText(Path.Combine(Application.persistentDataPath, "AirshipAdConfig.json"));
            var newList = JsonConvert.DeserializeObject<List<AirAdComponent>>(jsonFile);

            foreach (var airShip in newList)
            {
                waitingAds.Enqueue(airShip);
            }
            
            Debug.Log("AirshipAdManager Start");
            StartCoroutine(PlayAgain());
        }
        else
        {
            ApplyRemoteConfig(ConfigRequestStatus.Success);
        }
    }
    
    public void ApplyRemoteConfig(ConfigRequestStatus response)
    {
        if (response == ConfigRequestStatus.Success)
        {
            Debug.Log("Remote Config Fetched Successfully!");
            var airshipAdsJson = RemoteConfigService.Instance.appConfig.GetJson("AirshipAd");

            Debug.Log("AirshipAdsJson : " + airshipAdsJson);
            if (!string.IsNullOrEmpty(airshipAdsJson))
            {
                var airshipAds = JsonConvert.DeserializeObject<List<AirShipAd>>(airshipAdsJson);
                foreach (var ad in airshipAds)
                {
                    airShipAds.Enqueue(ad);
                }
                
                StartCoroutine(ShowAds());
            }
        }
        else
        {
            Debug.Log("Remote Config Fetch Failed!");
        }

        // Start the coroutine to show ads
        StartCoroutine(ShowAds());
        isFirstTime = false;

        PlayerPrefs.SetInt("IsFirstTime",0);
        PlayerPrefs.Save();
    }

    private IEnumerator ShowAds()
    {
        while (airShipAds.Count > 0)
        {
            while (!AdCoordinator.Instance.CanShowAd())
            {
                yield return null; // wait until other ad finishes
            }
            var ad = airShipAds.Dequeue();
            Debug.Log("Ad Name: " + ad.AdName);
            Debug.Log("Ad URL: " + ad.Url);
            Debug.Log("Airship Image URL: " + ad.AirShipImageUrl);

            AdCoordinator.Instance.NotifyAdStarted(); // Notify ad start
            // Instantiate the ad prefab and set its data
            var adInstance = Instantiate(adPrefab, transform);
            var adUIComponent = adInstance.GetComponent<AirshipAdUI>();

            if (adUIComponent != null)
            {
                adUIComponent.SetData(ad.Url, ad.AirShipImageUrl);
            }
            else
            {
                Debug.LogError("AirshipAdUI component is missing on the ad prefab.");
                Destroy(adInstance);
                continue;
            }
            
            var airshipAdComponent = new AirAdComponent
            {
                AirShipAd = ad,
                TimeToShow = DateTime.Now.AddSeconds(30).ToString() // Set the time to show the ad
            };
            waitingAds.Enqueue(airshipAdComponent);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "AirshipAdConfig.json"),
                JsonConvert.SerializeObject(waitingAds));
            
            yield return new WaitForSeconds(315f); // Show the 
            AdCoordinator.Instance.NotifyAdEnded();
            Destroy(adInstance); // Destroy the ad instance after showing
        }
    }
    private IEnumerator PlayAgain()
    {
        DateTime timeToShow = DateTime.Parse(waitingAds.Peek().TimeToShow);
        double timeToShowAgain = timeToShow.Subtract(DateTime.Now).TotalSeconds;
        
        Debug.Log("Time To Show Again: " + timeToShowAgain);

        while (timeToShowAgain > 0)
        {
            timeToShowAgain--;
            yield return new WaitForSeconds(1);
            Debug.Log("Time To Show Again: " + timeToShowAgain);

            if (timeToShowAgain <= 0)
            {
                break;
            }
        }
        
        Debug.Log("AirshipAdManager Start");
        
        foreach (var waiting in waitingAds)
        {
            airShipAds.Enqueue(waiting.AirShipAd);
        }
        StartCoroutine(ShowAds());
    }
    private void OnApplicationQuit() 
    {
        // Clean up the queue and waiting ads
        waitingAds.Clear();
        PlayerPrefs.DeleteKey("IsFirstTime");
        PlayerPrefs.Save();
        airAdComponent = null;
        isFirstTime = true;
    }
}

// For Deserialization from Json
[Serializable]
public class Ad
{
    public string AdName;
    public string Url;
}
[Serializable]
public class AirShipAd : Ad
{
    public string AirShipImageUrl;
}
[Serializable]
public class AirAdComponent
{
    public AirShipAd AirShipAd = null;
    public string TimeToShow = null;
}