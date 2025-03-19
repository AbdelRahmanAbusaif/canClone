using System;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static RemotelyDownloadAssets;

public class ApplyRemoteConfig : MonoBehaviour
{
    [SerializeField] private AdUI adUI;
    [SerializeField] private StorePageManager storePageManager;
    [SerializeField] private RemotelyDownloadAssets remotelyDownloadAssets;
    [SerializeField] private UILogin uILogin;
    private async void Awake() 
    {
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
    }


    private void OnDestroy()
    {
        RemoteConfigService.Instance.FetchCompleted -= Apply;
    }
}
