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
    
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject storeItemPrefab;

    [SerializeField] private Transform avatarContent;
    [SerializeField] private Transform borderContent;
    private async void Start()
    {
        await UnityServices.Instance.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn)
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        loadingPanel.SetActive(true);

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());

        Create(avatarContent, avatarItems);
        Create(borderContent, borderItems);

        var storeItems = new StoreItems
        {
            AvatarItems = avatarItems,
            BorderItems = borderItems
        };
        Debug.Log($"{JsonUtility.ToJson(storeItems)}");
    }

    private void Create(Transform content , List<StoreItem> list)
    {
        foreach (var (item, storeItem) in from item in list
                                          let storeItem = Instantiate(storeItemPrefab, content)
                                          select (item, storeItem))
        {
            content.GetComponent<RectTransform>().sizeDelta += new Vector2(300, 0);
            storeItem.GetComponent<StoreItemUI>().SetItem(item);
        }
    }


    private void ApplyRemoteConfig(ConfigResponse response)
    {
        var jsonData = RemoteConfigService.Instance.appConfig.GetJson("StoreItems");
        var storeItems = JsonUtility.FromJson<StoreItems>(jsonData);

        avatarItems = storeItems.AvatarItems;
        borderItems = storeItems.BorderItems;

        loadingPanel.SetActive(false);
    }

    private void OnDestroy() 
    {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
    }
    
}

internal class StoreItems
{
    public List<StoreItem> AvatarItems;
    public List<StoreItem> BorderItems;
}