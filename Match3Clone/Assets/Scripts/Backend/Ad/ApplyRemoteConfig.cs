using System;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static RemotelyDownloadAssets;

public class ApplyRemoteConfig : MonoBehaviour
{
    [SerializeField] private AdUI adUI;
    [SerializeField] private StorePageManager storePageManager;
    private async void Awake() 
    {
        RemoteConfigService.Instance.FetchCompleted += Apply;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
    }

    private void Apply(ConfigResponse response)
    {
        storePageManager.ApplyRemoteConfig(response);
        adUI.ApplyRemoteConfig(response);
    }


    private void OnDestroy()
    {
        RemoteConfigService.Instance.FetchCompleted -= Apply;
    }
}
