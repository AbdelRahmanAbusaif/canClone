using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Unity.Services.RemoteConfig;
using Newtonsoft.Json;
using static RemotelyDownloadAssets;
using Unity.Services.Core;

public class AirshipAdManager : MonoBehaviour
{
    // public static Action<>
    private Queue<AirShipAd> airShipAds = new Queue<AirShipAd>();
    [SerializeField] private static List<AirAdComponent> waitingAds = new List<AirAdComponent>();
    [SerializeField] private GameObject adPrefab;

    [SerializeField] private static AirAdComponent airAdComponent = null;
    public bool isFirstTime = true;
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());

        if(PlayerPrefs.GetInt("IsFirstTime",1) == 1)
        {
            ApplyRemoteConfig(ConfigRequestStatus.Success);
        }
    }
    private void Update() 
    {
        if(waitingAds.Count > 0 || airAdComponent != null)
        {
            if(airAdComponent == null)
            {
                Debug.Log("AirAdComponent is null");
                airAdComponent = waitingAds.FirstOrDefault();
            }
            if(airAdComponent != null && waitingAds.Contains(airAdComponent))
            {
                Debug.Log("Ad Name: " + airAdComponent.AirShipAd.AdName);
                Debug.Log("Ad URL: " + airAdComponent.AirShipAd.Url);
                Debug.Log("Airship Image URL: " + airAdComponent.AirShipAd.AirShipImageUrl);
                waitingAds.Remove(airAdComponent);
            }
            Debug.Log("Current Time: " + DateTime.Now.ToString() + " TimeToShow: " + airAdComponent.TimeToShow + "true or false: " + (DateTime.Now.ToString() == airAdComponent.TimeToShow));
            if(DateTime.Now.ToString() == airAdComponent.TimeToShow)
            {
                Debug.Log("Ad Show");
                airShipAds.Enqueue(airAdComponent.AirShipAd);
                airAdComponent = null;
                StartCoroutine(ShowAds());
            }
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
            var ad = airShipAds.Dequeue();
            Debug.Log("Ad Name: " + ad.AdName);
            Debug.Log("Ad URL: " + ad.Url);
            Debug.Log("Airship Image URL: " + ad.AirShipImageUrl);

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

            yield return new WaitForSeconds(10f); // Show the ad for 5 seconds
            var airshipAdComponent = new AirAdComponent
            {
                AirShipAd = ad,
                TimeToShow = DateTime.Now.AddSeconds(315).ToString() // Set the time to show the ad
            };
            waitingAds.Add(airshipAdComponent);
            Destroy(adInstance); // Destroy the ad instance after showing
        }
    }
    private void OnApplicationQuit() 
    {
        // Clean up the queue and waiting ads
        waitingAds.Clear();
        PlayerPrefs.DeleteKey("IsFirstTime");
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