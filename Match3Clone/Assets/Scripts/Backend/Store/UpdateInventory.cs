using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;

public class UpdateInventory : MonoBehaviour
{
    [SerializeField] private List<ConsumableItem> avatarContainer;
    [SerializeField] private List<ConsumableItem> coverContainer;
    [SerializeField] private List<ConsumableItem> borderContainer;
    [SerializeField] private ConsumableItem primeSubscription;

    private PlayerProfile playerProfile;
    private async void OnEnable()
    {
        avatarContainer = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileAvatarImages");
        coverContainer = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileCoverImages");
        borderContainer = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileBorders");
        primeSubscription = await LocalSaveManager.Instance.LoadDataAsync<ConsumableItem>("PrimeSubscriptions");

        CheckExpiredList(ref avatarContainer, "ContainerProfileAvatarImages");
        CheckExpiredList(ref coverContainer, "ContainerProfileCoverImages");
        CheckExpiredList(ref borderContainer, "ContainerProfileBorders");
        CheckExpiredItem(ref primeSubscription, "PrimeSubscriptions");

        await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileAvatarImages", avatarContainer);
        await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileCoverImages", coverContainer);
        await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileBorders", borderContainer);
        await CloudSaveManager.Instance.SaveDataAsync("PrimeSubscriptions", primeSubscription);
    }

    private void CheckExpiredList(ref List<ConsumableItem> consumableItems, string dateKey = "DateExpired")
    {
        if(consumableItems == null || consumableItems.Count == 0)
        {
            return;
        }
        consumableItems = consumableItems
        .Where(item =>
            DateTime.TryParse(item.DateExpired, out DateTime date) &&
            date >= ServerTimeManager.Instance.CurrentTime)
        .ToList();
    }
    private void CheckExpiredItem(ref ConsumableItem consumableItem, string dataKey = "")
    {
        Debug.Log("Prime Subscription: " + consumableItem.ConsumableName);

        DateTime expiredDate = DateTime.TryParse(primeSubscription.DateExpired, out DateTime date) ? date : DateTime.MinValue;

        Debug.Log("Expired Date :" + expiredDate);
        if (expiredDate < ServerTimeManager.Instance.CurrentTime)
        {
            consumableItem.DateExpired = "";
            consumableItem.DatePurchased = "";
            Debug.Log("Prime Subscription Removed: " + consumableItem.ConsumableName);
        }
        else
        {
            Debug.Log("Prime Subscription: " + consumableItem.ConsumableName);
        }
    }

}
