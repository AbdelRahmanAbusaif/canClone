using System.Collections.Generic;
using Unity.Services.RemoteConfig;
using UnityEngine;
using System.Linq;
using Unity.Services.Core;
using Newtonsoft.Json;


public class StorePageManager : MonoBehaviour
{
    [SerializeField] private List<StoreItem> avatarItems;
    [SerializeField] private List<StoreItem> borderItems;
    [SerializeField] private List<StoreItem> coverProfileItems;

    [SerializeField] private List<PrimeSubscription> primeSubscriptionItems;
    
    [SerializeField] private GameObject storeAvatarProfilePrefabs;
    [SerializeField] private GameObject storeBorderProfilePrefabs;
    [SerializeField] private GameObject storeCoverProfilePrefabs;
    [SerializeField] private GameObject storePrimeSubscriptionPrefabs;

    [SerializeField] private Transform avatarContent;
    [SerializeField] private Transform borderContent;
    [SerializeField] private Transform coverProfileContent;
    [SerializeField] private Transform primeSubscriptionContent;

    [SerializeField] private GameObject loadingPanel;
    private void OnEnable()
    {
        loadingPanel.SetActive(true);

        ApplyRemoteConfig();
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
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetComponent<RectTransform>().sizeDelta.y + 200);
        }
    }
    private void Create(Transform content ,GameObject itemPrefabs, List<PrimeSubscription> list)
    {
        foreach (var (item, storeItem) in from item in list
                                          let storeItem = Instantiate(itemPrefabs, content)
                                          select (item, storeItem))
        {
            storeItem.GetComponent<StoreItemUI>().SetPrimeSubscriptionItem(item);
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetComponent<RectTransform>().sizeDelta.y + 200);
        }
    }


    public void ApplyRemoteConfig()
    {
        var jsonData = RemoteConfigService.Instance.appConfig.GetJson("StoreItems");
        var storeItems = JsonConvert.DeserializeObject<StoreItems>(jsonData);

        foreach (var child in avatarContent.GetComponentsInChildren<StoreItemUI>())
        {
            Destroy(child.gameObject);
        }
        foreach (var child in borderContent.GetComponentsInChildren<StoreItemUI>())
        {
            Destroy(child.gameObject);
        }
        foreach (var child in coverProfileContent.GetComponentsInChildren<StoreItemUI>())
        {
            Destroy(child.gameObject);
        }
        foreach (var child in primeSubscriptionContent.GetComponentsInChildren<StoreItemUI>())
        {
            Destroy(child.gameObject);
        }
        

        avatarItems?.Clear();
        borderItems?.Clear();
        coverProfileItems?.Clear();
        primeSubscriptionItems?.Clear();
        
        avatarItems = storeItems.AvatarItems;
        borderItems = storeItems.BorderItems;
        coverProfileItems = storeItems.CoverProfileItems;
        primeSubscriptionItems = storeItems.PrimeSubscriptionItems;

        Create(avatarContent, storeAvatarProfilePrefabs, avatarItems);
        Create(borderContent, storeBorderProfilePrefabs, borderItems);
        Create(coverProfileContent, storeCoverProfilePrefabs, coverProfileItems);
        Create(primeSubscriptionContent, storePrimeSubscriptionPrefabs, primeSubscriptionItems);

        loadingPanel.SetActive(false);
    }    
}

internal class StoreItems
{
    public List<StoreItem> AvatarItems;
    public List<StoreItem> BorderItems;
    public List<StoreItem> CoverProfileItems;
    public List<PrimeSubscription> PrimeSubscriptionItems;
}