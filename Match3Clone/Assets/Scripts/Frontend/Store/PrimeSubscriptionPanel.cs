using System;
using System.Threading.Tasks;
using GameVanilla.Game.Common;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrimeSubscriptionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private AnimationBox animationBox;
    [SerializeField] private Button buyButton;
    private PlayerProfile playerProfile;
    private PrimeSubscription primeSubscription;
    private async void OnEnable() 
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        buyButton.onClick.AddListener(OnClickBuyButton);
    }

    public void SetPrimeSubscriptionItem(PrimeSubscription primeSubscription)
    {
        this.primeSubscription = primeSubscription;

        title.text = primeSubscription.Title;
    }
    private async void OnClickBuyButton()
    {
        int price = int.Parse(primeSubscription.Price);
        PuzzleMatchManager.instance.coinsSystem.SpendCoins(price);

        switch(primeSubscription.DurationType)
        {
            case Duration.OneDay:
                Add(playerProfile.PrimeSubscriptions, ServerTimeManager.Instance.CurrentTime.AddDays(1).ToString());
                break;
            case Duration.SevenDays:
                Add(playerProfile.PrimeSubscriptions, ServerTimeManager.Instance.CurrentTime.AddDays(7).ToString());
                break;
            case Duration.ThirtyDays:
                Add(playerProfile.PrimeSubscriptions, ServerTimeManager.Instance.CurrentTime.AddDays(30).ToString());
                break;
            case Duration.HalfYear:
                Add(playerProfile.PrimeSubscriptions, ServerTimeManager.Instance.CurrentTime.AddDays(185).ToString());
                break;
            case Duration.OneYear:
                Add(playerProfile.PrimeSubscriptions, ServerTimeManager.Instance.CurrentTime.AddDays(365).ToString());
                break;
        }
        await CloudSaveManager.Instance.SaveDataAsync<PlayerProfile>("PlayerProfile", playerProfile);
    }
    private void Add(ConsumableItem data , string duration)
    {
        data.DatePurchased = ServerTimeManager.Instance.CurrentTime.ToString();

        Debug.Log("Min Value: " + DateTime.MinValue.ToString());
        // if the item is not expired then set the date expired to the date purchased , this will be also for the first time purchase , or the item is was expired
        if(data.DateExpired == DateTime.MinValue.ToString() || String.IsNullOrEmpty(data.DateExpired))
        {
            Debug.Log("Item not owned");
            data.DateExpired = data.DatePurchased;
        }

        DateTime durationDate = DateTime.Parse(duration);
        int durationDays = durationDate.Day - ServerTimeManager.Instance.CurrentTime.Day; // Extract the day component
        DateTime dateTime = DateTime.Parse(data.DateExpired);
        data.DateExpired = dateTime.AddDays(durationDays).ToString();
        playerProfile.PrimeSubscriptions = data;

        animationBox.OnClose();
    }

    public void OnClose()
    {
        Invoke(nameof(DestroyPanel), 1f);
    }

    private void DestroyPanel()
    {
        buyButton.onClick.RemoveAllListeners();
        Destroy(gameObject);
    }


}
