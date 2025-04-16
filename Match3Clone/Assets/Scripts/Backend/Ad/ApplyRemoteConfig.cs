using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static RemotelyDownloadAssets;

public class ApplyRemoteConfig : MonoBehaviour
{
    [SerializeField] private AdUI adUI;
    [SerializeField] private StorePageManager storePageManager;
    [SerializeField] private RemotelyDownloadAssets remotelyDownloadAssets;
    [SerializeField] private UILogin uILogin;
    [SerializeField] private LoadingManager loadingManager;
    [SerializeField] private LeaderboardButtonManager leaderboardButtonManager;
    private async void Start() 
    {
        try
        {
            await UnityServices.InitializeAsync();
            
            Debug.Log("Unity Services Initialized");
    
            RemoteConfigService.Instance.FetchCompleted += Apply;
            await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
        }
    }

    private void Apply(ConfigResponse response)
    {
        if(remotelyDownloadAssets != null)
        {
            Debug.Log("Remote Config Fetched Successfully in ApplyRemoteConfig for RemotelyDownloadAssets!");
            remotelyDownloadAssets.ApplyRemoteConfig(response);
        }
        if(loadingManager != null)
        {
            Debug.Log("Remote Config Fetched Successfully in ApplyRemoteConfig for LoadingManager!");
            loadingManager.ApplyRemoteConfig(response);
        }
        if(leaderboardButtonManager != null)
        {
            Debug.Log("Remote Config Fetched Successfully in ApplyRemoteConfig for LeaderboardButtonManager!");
            leaderboardButtonManager.ApplyRemoteConfig(response);
        }
        if(adUI != null)
        {
            Debug.Log("Remote Config Fetched Successfully in ApplyRemoteConfig for AdUI!");
            adUI.ApplyRemoteConfig(response);
        }
        // if(storePageManager != null)
        // {
        //     Debug.Log("Remote Config Fetched Successfully in ApplyRemoteConfig for StorePageManager!");
        //     storePageManager.ApplyRemoteConfig();
        // }
    }


    private void OnDestroy()
    {
        RemoteConfigService.Instance.FetchCompleted -= Apply;
    }
}
 public struct UserAttributes { }
public struct AppAttributes { }