using System;
using System.Threading.Tasks;
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
    private async void Awake() 
    {
        await UnityServices.InitializeAsync();

        RemoteConfigService.Instance.FetchCompleted += Apply;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
    }

    private void Apply(ConfigResponse response)
    {
        if(uILogin != null)
        {
            uILogin.ApplyRemoteConfig(response);
        }
        if(adUI != null)
        {
            adUI.ApplyRemoteConfig(response);
        }
        if(storePageManager != null)
        {
            storePageManager.ApplyRemoteConfig(response);
        }
        if(remotelyDownloadAssets != null)
        {
            remotelyDownloadAssets.ApplyRemoteConfig(response);
        }
        if(loadingManager != null)
        {
            loadingManager.ApplyRemoteConfig(response);
        }
    }


    private void OnDestroy()
    {
        RemoteConfigService.Instance.FetchCompleted -= Apply;
    }
}
