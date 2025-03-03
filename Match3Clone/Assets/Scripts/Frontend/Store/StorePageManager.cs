using System.Collections.Generic;
using Unity.Services.RemoteConfig;
using UnityEngine;
using System.Linq;
using static RemotelyDownloadAssets;
using Unity.Services.Core;
using Unity.Services.Authentication;


public class StorePageManager : MonoBehaviour
{
    [SerializeField] private List<StoreItem> avatarItems;
    [SerializeField] private List<StoreItem> borderItems;
    [SerializeField] private List<StoreItem> coverProfileItems;
    
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject storeAvatarProfilePrefabs;
    [SerializeField] private GameObject storeBorderProfilePrefabs;
    [SerializeField] private GameObject storeCoverProfilePrefabs;

    [SerializeField] private Transform avatarContent;
    [SerializeField] private Transform borderContent;
    [SerializeField] private Transform coverProfileContent;
    private async void Start()
    {
        loadingPanel.SetActive(true);

        await UnityServices.Instance.InitializeAsync();
        // if(!AuthenticationService.Instance.IsSignedIn)
        // await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Create(Transform content ,GameObject itemPrefabs, List<StoreItem> list)
    {
        foreach (var (item, storeItem) in from item in list
                                          let storeItem = Instantiate(itemPrefabs, content)
                                          select (item, storeItem))
        {
            storeItem.GetComponent<StoreItemUI>().SetItem(item);
        }
    }


    public void ApplyRemoteConfig(ConfigResponse response)
    {
        var jsonData = RemoteConfigService.Instance.appConfig.GetJson("StoreItems");
        var storeItems = JsonUtility.FromJson<StoreItems>(jsonData);

        avatarItems = storeItems.AvatarItems;
        borderItems = storeItems.BorderItems;
        coverProfileItems = storeItems.CoverProfileItems;

        Create(avatarContent, storeAvatarProfilePrefabs, avatarItems);
        Create(borderContent, storeBorderProfilePrefabs, borderItems);
        Create(coverProfileContent, storeCoverProfilePrefabs, coverProfileItems);

        loadingPanel.SetActive(false);
    }    
}

internal class StoreItems
{
    public List<StoreItem> AvatarItems;
    public List<StoreItem> BorderItems;
    public List<StoreItem> CoverProfileItems;
}