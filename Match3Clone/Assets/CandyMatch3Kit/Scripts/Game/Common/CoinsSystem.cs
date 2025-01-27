using Unity.Services.Core;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class CoinsSystem : MonoBehaviour
{
    private Action<long> onCoinsUpdated;
    private const string COIN_CURRENCY_ID = "TOKEN_COIN";
    
    public int Coins { get; private set; }
    private async void Start()
    {
        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await EconomyService.Instance.Configuration.SyncConfigurationAsync();
            Debug.Log("Unity Economy initialized successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to initialize Unity Economy: " + e.Message);
        }
    }

    /// <summary>
    /// Buys the specified amount of coins.
    /// </summary>
    /// <param name="amount">The amount of coins to buy.</param>
    public async void BuyCoins(int amount)
    {
        try
        {
            await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(COIN_CURRENCY_ID, amount);
            long newBalance = await GetCurrentCoins();
            onCoinsUpdated?.Invoke(newBalance);

            Coins = (int)newBalance;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to buy coins: " + e.Message);
        }
    }

    /// <summary>
    /// Spends the specified amount of coins.
    /// </summary>
    /// <param name="amount">The amount of coins to spend.</param>
    public async void SpendCoins(int amount)
    {
        try
        {
            long currentBalance = await GetCurrentCoins();
            if (currentBalance >= amount)
            {
                await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync(COIN_CURRENCY_ID, amount);
                long newBalance = await GetCurrentCoins();
                onCoinsUpdated?.Invoke(newBalance);

                Coins = (int)newBalance;
            }
            else
            {
                Debug.LogWarning("Not enough coins to spend.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to spend coins: " + e.Message);
        }
    }

    /// <summary>
    /// Gets the player's current coin balance.
    /// </summary>
    public async Task<long> GetCurrentCoins()
    {
        try
        {
            CurrencyDefinition goldCurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency(COIN_CURRENCY_ID);
            PlayerBalance playersGoldBarBalance = await goldCurrencyDefinition.GetPlayerBalanceAsync();
            return playersGoldBarBalance.Balance;
            
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get coin balance: " + e.Message);
            return 0;
        }
    }

    /// <summary>
    /// Registers the specified callback to be called when the amount of coins changes.
    /// </summary>
    /// <param name="callback">The callback to register.</param>
    public async void Subscribe(Action<long> callback)
    {
        onCoinsUpdated += callback;
        callback?.Invoke(await GetCurrentCoins()); // Provide initial balance on subscribe
    }

    /// <summary>
    /// Unregisters the specified callback to be called when the amount of coins changes.
    /// </summary>
    /// <param name="callback">The callback to unregister.</param>
    public void Unsubscribe(Action<long> callback)
    {
        onCoinsUpdated -= callback;
    }
}
