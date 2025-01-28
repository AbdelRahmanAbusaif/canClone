// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace GameVanilla.Game.Common
{
    /// <summary>
    /// This class handles the lives system in the game. It is used to add and remove lives and other classes
    /// can subscribe to it in order to receive a notification when the number of lives changes.
    /// </summary>
    public class LivesSystem : MonoBehaviour
    {
        private TimeSpan timeSpan;
        private bool runningCountdown;
        private float accTime;
        
        public Action<TimeSpan, int> onCountdownUpdated;
        public Action<int> onCountdownFinished;
        private DateTime dateTime;

        private async void Awake() {
            await PuzzleMatchManager.instance.serverTimeManager.FetchServerTime(OnServerTimeFetched, OnServerTimeFetchFailed);
        }
        /// <summary>
        /// Sets the appropriate number of lives according to the general lives counter.
        /// </summary> 
        private async void Start()
        {            
            await InitializeUnityServices();

            Invoke(nameof(CheckLives), 3f);
        }

        private void OnServerTimeFetchFailed(string obj)
        {
            Debug.LogError($"Error fetching server time: {obj}");
        }

        private void OnServerTimeFetched(DateTime time)
        {
            dateTime = time;
        }

        /// <summary>
        /// Unity's Update method.
        /// </summary>
        private async void Update()
        {
            if (!runningCountdown)
            {
                Debug.Log("runningCountdown: " + runningCountdown);
                return;
            }

            accTime += Time.deltaTime;
            if (accTime >= 1.0f)
            {
                accTime = 0.0f;
                timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1));
                // var numLives = PlayerPrefs.GetInt("num_lives");
                var numLives = await GetCurrentLives();
                if (onCountdownUpdated != null)
                {
                    onCountdownUpdated(timeSpan, numLives);
                }
                if ((int)timeSpan.TotalSeconds == 0)
                {
                    StopCountdown();
                    AddLife();

                    await PuzzleMatchManager.instance.serverTimeManager.FetchServerTime(serverTime =>
                    {
                        PlayerPrefs.SetString("next_life_time", serverTime.Date.ToString()); // ISO 8601 format
                        PlayerPrefs.Save();
                        Debug.Log("Heart claimed and time saved!");
                    }, error =>
                    {
                        Debug.LogError($"Failed to claim heart: {error}");
                    });
                }
            }
        }

        public async Task<int> GetCurrentLives()
        {
            CurrencyDefinition goldCurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency("HEART_ID");
            PlayerBalance playersGoldBarBalance = await goldCurrencyDefinition.GetPlayerBalanceAsync();
            var balanceResult = playersGoldBarBalance;
            return (int)balanceResult.Balance;
        }

        /// <summary>
        /// Make sure to check the lives when the app goes from background to foreground.
        /// </summary>
        /// <param name="pauseStatus">The pause status.</param>
        private  void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                CheckLives();
            }
        }

        /// <summary>
        /// Make sure to remove a life when the app is closed and there is an active game.
        /// </summary>
        private void OnApplicationQuit()
        {
            var gameScene = GameObject.Find("GameScene");
            if (gameScene != null)
            {
                RemoveLife();
            }
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

        private async void CheckLives()
        {
            runningCountdown = false;

            CurrencyDefinition goldCurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency("HEART_ID");
            PlayerBalance playersGoldBarBalance = await goldCurrencyDefinition.GetPlayerBalanceAsync();
            var balanceResult = playersGoldBarBalance;

            var numLives = (int)balanceResult.Balance;
            Debug.Log("From CheckLives: " + numLives);
            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;
            var timeToNextLife = PuzzleMatchManager.instance.gameConfig.timeToNextLife;
            
            if (!PlayerPrefs.HasKey("next_life_time"))
            {
                DateTime nextLifeTime = dateTime.AddMinutes(5); // Example: 5 minutes
                PlayerPrefs.SetString("next_life_time", nextLifeTime.ToBinary().ToString());
                PlayerPrefs.Save();
                Debug.Log($"Initialized next_life_time: {nextLifeTime}");
            }
            if (numLives < maxLives && PlayerPrefs.HasKey("next_life_time") )
            {
                DateTime now;
                if(dateTime == DateTime.MinValue)
                {
                    Debug.LogError("Server time not fetched yet.");
                    now = DateTime.Now;
                }
                else 
                {
                    now = dateTime;
                }

                Debug.Log("From CheckLives Now: " + now);
                var savedNextLifeTime = GetSavedNextLifeTime();
                Debug.Log("From CheckLives savedNextLifeTime: " + savedNextLifeTime);

                if (savedNextLifeTime.Value > now)
                {
                    var remainingTime = savedNextLifeTime.Value - now;
                    if(numLives < maxLives)
                    {
                        StartCountdown((int)remainingTime.TotalSeconds);
                    }
                }
                else
                {
                    var elapsedTime = now - savedNextLifeTime.Value;
                    var livesToGive = ((int)elapsedTime.TotalSeconds / timeToNextLife) + 1;
                    numLives = Mathf.Min(numLives + livesToGive, maxLives);

                    await EconomyService.Instance.PlayerBalances.SetBalanceAsync("HEART_ID", numLives);

                    Debug.Log("Here else");

                    if (numLives < maxLives)
                    {
                        StartCountdown(timeToNextLife - ((int)elapsedTime.TotalSeconds % timeToNextLife));

                        Debug.Log("Here else if");
                    }

                    onCountdownFinished?.Invoke(numLives);
                }
            }
        }


        /// <summary>
        /// Adds a life to the system.
        /// </summary>
        public async void AddLife()
        {
            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;
            CurrencyDefinition goldCurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency("HEART_ID");
            PlayerBalance playersGoldBarBalance = await goldCurrencyDefinition.GetPlayerBalanceAsync();

            var balanceResult = playersGoldBarBalance;
            var numLives = (int)balanceResult.Balance;

            if (numLives < maxLives)
            {
                await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("HEART_ID", 1);

                if (!runningCountdown)
                {
                    var timeToNextLife = PuzzleMatchManager.instance.gameConfig.timeToNextLife;

                    Debug.Log("From AddLife: " + timeToNextLife);
                    StartCountdown(timeToNextLife);
                }
            }
            else
            {
                Debug.Log("From AddLife: else" + numLives);
                StopCountdown();
            }
        }


        /// <summary>
        /// Removes a life from the system.
        /// </summary>
        public async void RemoveLife()
        {
            CurrencyDefinition goldCurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency("HEART_ID");
            PlayerBalance playersGoldBarBalance = await goldCurrencyDefinition.GetPlayerBalanceAsync();
            var balanceResult = playersGoldBarBalance;

            var numLives = (int)balanceResult.Balance;

            if (numLives > 0)
            {
                await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync("HEART_ID", 1);
            }

            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;
            if (numLives < maxLives && !runningCountdown)
            {
                var timeToNextLife = PuzzleMatchManager.instance.gameConfig.timeToNextLife;

                Debug.Log("From RemoveLife: " + timeToNextLife);
                StartCountdown(timeToNextLife);
            }
        }


        /// <summary>
        /// Sets the number of lives to the maximum number allowed by the game configuration.
        /// </summary>
        public async void RefillLives()
        {
            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;
            var refillCost = PuzzleMatchManager.instance.gameConfig.livesRefillCost;

            CurrencyDefinition goldCurrencyDefinition = EconomyService.Instance.Configuration.GetCurrency("TOKEN_COIN");
            PlayerBalance playersGoldBarBalance = await goldCurrencyDefinition.GetPlayerBalanceAsync();
            var coinBalance = playersGoldBarBalance;

            if (coinBalance.Balance >= refillCost)
            {
                PuzzleMatchManager.instance.coinsSystem.SpendCoins(refillCost);
                await EconomyService.Instance.PlayerBalances.SetBalanceAsync("HEART_ID", maxLives);
                StopCountdown();
            }
        }


        /// <summary>
        /// Starts the countdown to give a free life to the player.
        /// </summary>
        /// <param name="timeToNextLife">The time in seconds until the next free life is given.</param>
        public async void StartCountdown(int timeToNextLife)
        {
            SetTimeToNextLife(timeToNextLife);
            timeSpan = TimeSpan.FromSeconds(timeToNextLife);
            runningCountdown = true;

            if (onCountdownUpdated == null) return;

            var numLives = await GetCurrentLives();
            onCountdownUpdated(timeSpan, numLives);
        }

        /// <summary>
        /// Stops the countdown to give a free life to the player.
        /// </summary>
        public async void StopCountdown()
        {
            runningCountdown = false;
            var numLives = await GetCurrentLives();
            if (onCountdownFinished != null)
            {
                onCountdownFinished(numLives);
            }
        }

        /// <summary>
        /// Registers the specified callbacks to be called when the amount of lives changes.
        /// </summary>
        /// <param name="updateCallback">The callback to register for when the number of lives changes.</param>
        /// <param name="finishCallback">The callback to register for when the free life is given.</param>
        public async void Subscribe(Action<TimeSpan, int> updateCallback, Action<int> finishCallback)
        {
            onCountdownUpdated += updateCallback;
            onCountdownFinished += finishCallback;
            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;
            var numLives = await GetCurrentLives();
            if (numLives < maxLives)
            {
                if (onCountdownUpdated != null)
                {
                    onCountdownUpdated(timeSpan, numLives);
                }
            }
            else
            {
                if (onCountdownFinished != null)
                {
                    onCountdownFinished(numLives);
                }
            }
        }
        /// <summary>
        /// Unregisters the specified callbacks to be called when the amount of lives changes.
        /// </summary>
        /// <param name="updateCallback">The callback to unregister for when the number of lives changes.</param>
        /// <param name="finishCallback">The callback to unregister for when the free life is given.</param>
        public void Unsubscribe(Action<TimeSpan, int> updateCallback, Action<int> finishCallback)
        {
            if (onCountdownUpdated != null)
            {
                onCountdownUpdated -= updateCallback;
            }
            if (onCountdownFinished != null)
            {
                onCountdownFinished -= finishCallback;
            }
        }
        private void SetTimeToNextLife(int seconds)
        {
            var nextLifeTime = DateTime.Now.AddSeconds(seconds);
            SaveNextLifeTime(nextLifeTime);
        }
        public void SaveNextLifeTime(DateTime nextLifeTime)
        {
            PlayerPrefs.SetString("next_life_time", nextLifeTime.ToBinary().ToString());
            PlayerPrefs.Save();
        }
        public DateTime? GetSavedNextLifeTime()
        {

            if (PlayerPrefs.HasKey("next_life_time"))
            {
                string binaryString = PlayerPrefs.GetString("next_life_time");
                Debug.Log($"Retrieved next_life_time: {binaryString}");
                if (long.TryParse(binaryString, out long binary))
                {
                    return DateTime.FromBinary(binary);
                }
            }
            Debug.Log("next_life_time not found in PlayerPrefs.");
            return null;
        }
    }
}

