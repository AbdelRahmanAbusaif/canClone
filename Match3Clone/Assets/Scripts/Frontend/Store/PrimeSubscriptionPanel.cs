using System;
using System.Threading.Tasks;
using GameVanilla.Game.Common;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrimeSubscriptionPanel : MonoBehaviour
{
    public static event Action OnSubscriptionPurchased;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private AnimationBox animationBox;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject loadingSpinner;
    [SerializeField] private GameObject notEnoughCoinsPanel;
    private PrimeSubscription primeSubscription;

    private ConsumableItem data;
    private async void OnEnable() 
    {
        data = await LocalSaveManager.Instance.LoadDataAsync<ConsumableItem>("PrimeSubscriptions");
        
        buyButton.onClick.AddListener(OnClickBuyButton);
    }

    public void SetPrimeSubscriptionItem(PrimeSubscription primeSubscription)
    {
        this.primeSubscription = primeSubscription;

        title.text = primeSubscription.Title;
    }
    private void OnClickBuyButton()
    {
        loadingSpinner.SetActive(true);
        int price = int.Parse(primeSubscription.Price);
        int coins = PuzzleMatchManager.instance.coinsSystem.Coins;

        if(coins < price)
        {
            Debug.Log("Not enough coins");

            buyButton.onClick.RemoveAllListeners();
            notEnoughCoinsPanel.SetActive(true);

            Invoke(nameof(DisableNotEnoughCoinsPanel), 2f);

            OnClose();
            return;
        }

        PuzzleMatchManager.instance.coinsSystem.SpendCoins(price);
        switch(primeSubscription.DurationType)
        {
            case Duration.OneDay:
                Add(data, ServerTimeManager.Instance.CurrentTime.AddDays(1).ToString());
                break;
            case Duration.SevenDays:
                Add(data, ServerTimeManager.Instance.CurrentTime.AddDays(7).ToString());
                break;
            case Duration.ThirtyDays:
                Add(data, ServerTimeManager.Instance.CurrentTime.AddDays(30).ToString());
                break;
            case Duration.HalfYear:
                Add(data, ServerTimeManager.Instance.CurrentTime.AddDays(185).ToString());
                break;
            case Duration.OneYear:
                Add(data, ServerTimeManager.Instance.CurrentTime.AddDays(365).ToString());
                break;
        }
    }

    private void DisableNotEnoughCoinsPanel()
    {
        notEnoughCoinsPanel.SetActive(false);
    }

    private async void Add(ConsumableItem data , string duration)
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
        var durationDays = durationDate - ServerTimeManager.Instance.CurrentTime; // Extract the day component
        Debug.Log("Duration Days: " + durationDays.Days);
        DateTime dateTime = DateTime.Parse(data.DateExpired);
        data.DateExpired = dateTime.AddDays(durationDays.Days).ToString();

        this.data.DateExpired = data.DateExpired;
        this.data.DatePurchased = data.DatePurchased;

        await CloudSaveManager.Instance.SaveDataAsync<ConsumableItem>("PrimeSubscriptions", data);

        OnSubscriptionPurchased?.Invoke();
        loadingSpinner.SetActive(false);
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
