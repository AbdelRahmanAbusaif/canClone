using System;
using System.Collections.Generic;
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
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

        avatarContainer = playerProfile.ContainerProfileAvatarImages;
        coverContainer = playerProfile.ContainerProfileCoverImages;
        borderContainer = playerProfile.ContainerProfileBorders;
        primeSubscription = playerProfile.PrimeSubscriptions;

        CheckExpiredList(avatarContainer);
        CheckExpiredList(coverContainer);
        CheckExpiredList(borderContainer);

        CheckExpiredItem(primeSubscription);

        playerProfile.ContainerProfileAvatarImages = avatarContainer;
        playerProfile.ContainerProfileCoverImages = coverContainer;
        playerProfile.ContainerProfileBorders = borderContainer;
        playerProfile.PrimeSubscriptions = primeSubscription;

        await CloudSaveManager.Instance.SaveDataAsync<PlayerProfile>("PlayerProfile", playerProfile);
    }

    private void CheckExpiredList(List<ConsumableItem> consumableItems)
    {
        if(consumableItems == null || consumableItems.Count == 0)
        {
            return;
        }
        foreach (var item in consumableItems)
        {
            Debug.Log("Item: " + item.ConsumableName);

            DateTime expiredDate = DateTime.TryParse(item.DateExpired , out DateTime date) ? date : DateTime.MinValue;

            Debug.Log("Expired Date :" + expiredDate);
            if (expiredDate < ServerTimeManager.Instance.CurrentTime)
            {
                Debug.Log("Item Removed: " + item.ConsumableName);
                consumableItems.Remove(item);
            }
            else
            {
                Debug.Log("Item: " + item.ConsumableName);
            }
        }
    }
    private void CheckExpiredItem(ConsumableItem consumableItem)
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
