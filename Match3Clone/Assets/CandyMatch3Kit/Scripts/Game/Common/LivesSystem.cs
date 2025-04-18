// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Threading.Tasks;
using SaveData;
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

        private HeartSystem heartSystem;
        private async void Awake() 
        {
            heartSystem = await LocalSaveManager.Instance.LoadDataAsync<HeartSystem>("HeartSystem");    
        }
        /// <summary>
        /// Sets the appropriate number of lives according to the general lives counter.
        /// </summary> 
        private void Start()
        {            
            Invoke(nameof(CheckLives), 1f);
        }
        /// <summary>
        /// Unity's Update method.
        /// </summary>
        private void Update()
        {
            if (!runningCountdown)
            {
                return;
            }

            accTime += Time.deltaTime;
            if (accTime >= 1.0f)
            {
                accTime = 0.0f;
                timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1));
                // var numLives = PlayerPrefs.GetInt("num_lives");
                var numLives = GetCurrentLives();
                if (onCountdownUpdated != null)
                {
                    onCountdownUpdated(timeSpan, numLives);
                }
                if ((int)timeSpan.TotalSeconds == 0)
                {
                    StopCountdown();
                    AddLife();
                }
            }
        }

        public int GetCurrentLives()
        {
            var numberOfLives = heartSystem.Heart;
            Debug.Log("From GetCurrentLives: " + numberOfLives);
            return 999999;
        }

        /// <summary>
        /// Make sure to check the lives when the app goes from background to foreground.
        /// </summary>
        /// <param name="pauseStatus">The pause status.</param>
        private  void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                Invoke(nameof(CheckLives), 1f);
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
                Debug.Log("Removed a life when the app was closed.");
            }
            else 
            {
                Debug.Log("No active game to remove a life from.");
            }
        }
        private async void CheckLives()
        {
            runningCountdown = false;
            var numLives = GetCurrentLives();

            Debug.Log("From CheckLives: " + numLives);
            
            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;
            var timeToNextLife = PuzzleMatchManager.instance.gameConfig.timeToNextLife;
            
            if (heartSystem.LastHeartTime == "0" && string.IsNullOrEmpty(heartSystem.LastHeartTime) &&numLives < maxLives)
            {
                DateTime nextLifeTime = ServerTimeManager.Instance.CurrentTime.AddMinutes(5);
                // PlayerPrefs.SetString("next_life_time", nextLifeTime.ToBinary().ToString());
                // PlayerPrefs.Save();
                heartSystem.LastHeartTime = nextLifeTime.ToString();
                await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);

                Debug.Log($"Initialized next_life_time: {nextLifeTime}");
            }
            if (numLives < maxLives && heartSystem.LastHeartTime !="0" && !string.IsNullOrEmpty(heartSystem.LastHeartTime))
            {
                DateTime nowTime;
                if(ServerTimeManager.Instance.CurrentTime == DateTime.MinValue)
                {
                    Debug.LogError("Server time not fetched yet.");
                    nowTime = DateTime.Now;
                }
                else 
                {
                    nowTime = ServerTimeManager.Instance.CurrentTime;
                    Debug.Log("Server time fetched: " + nowTime);
                }

                Debug.Log("From CheckLives Now: " + nowTime);
                var savedNextLifeTime = GetSavedNextLifeTime();
                Debug.Log("From CheckLives savedNextLifeTime: " + savedNextLifeTime);

                if (savedNextLifeTime.Value > nowTime)
                {
                    var remainingTime = savedNextLifeTime.Value - nowTime;
                    if(numLives < maxLives)
                    {
                        StartCountdown((int)remainingTime.TotalSeconds);
                    }
                }
                else
                {
                    var elapsedTime = nowTime - savedNextLifeTime.Value;
                    var livesToGive = ((int)elapsedTime.TotalSeconds / timeToNextLife) + 1;
                    numLives = Mathf.Min(numLives + livesToGive, maxLives);

                    heartSystem.Heart = numLives;
                    await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);

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
            var numLives = GetCurrentLives();

            if (numLives < maxLives)
            {
                heartSystem.Heart++;
                numLives = GetCurrentLives();
                await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);

                if (!runningCountdown)
                {
                    var timeToNextLife = PuzzleMatchManager.instance.gameConfig.timeToNextLife;

                    Debug.Log("From AddLife: " + timeToNextLife);

                    if(numLives < maxLives)
                    {
                        StartCountdown(timeToNextLife);
                    }
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
            var numLives = GetCurrentLives();
            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;

            if (numLives > 0)
            {
                heartSystem.Heart--;
                numLives = GetCurrentLives();
                await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);
            }

            if (numLives < maxLives && !runningCountdown)
            {
                var timeToNextLife = PuzzleMatchManager.instance.gameConfig.timeToNextLife;
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
                heartSystem.Heart = maxLives;
                await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);

                StopCountdown();
            }
        }


        /// <summary>
        /// Starts the countdown to give a free life to the player.
        /// </summary>
        /// <param name="timeToNextLife">The time in seconds until the next free life is given.</param>
        public void StartCountdown(int timeToNextLife)
        {
            SetTimeToNextLife(timeToNextLife);
            timeSpan = TimeSpan.FromSeconds(timeToNextLife);
            runningCountdown = true;

            if (onCountdownUpdated == null) return;

            var numLives = GetCurrentLives();
            onCountdownUpdated(timeSpan, numLives);
        }

        /// <summary>
        /// Stops the countdown to give a free life to the player.
        /// </summary>
        public void StopCountdown()
        {
            runningCountdown = false;
            var numLives = GetCurrentLives();
            if (onCountdownFinished != null)
            {
                onCountdownFinished(numLives);
            }
        }
        private void SetTimeToNextLife(int seconds)
        {
            var nextLifeTime = ServerTimeManager.Instance.CurrentTime.AddSeconds(seconds);
            SaveNextLifeTime(nextLifeTime);
        }
        public async void SaveNextLifeTime(DateTime nextLifeTime)
        {
            // PlayerPrefs.SetString("next_life_time", nextLifeTime.ToBinary().ToString());
            // PlayerPrefs.Save();
            heartSystem = await LocalSaveManager.Instance.LoadDataAsync<HeartSystem>("HeartSystem");
            
            heartSystem.LastHeartTime = nextLifeTime.ToString();

            await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);
        }
        public DateTime? GetSavedNextLifeTime()
        {

            if (heartSystem.LastHeartTime != "0" && !string.IsNullOrEmpty(heartSystem.LastHeartTime))
            {
                // string binaryString = PlayerPrefs.GetString("next_life_time");
                string dateString = heartSystem.LastHeartTime;
                Debug.Log($"Retrieved next_life_time: {dateString}");
                return DateTime.Parse(dateString);
            }
            
            Debug.Log("next_life_time not found in PlayerPrefs.");
            return null;
        }

        /// <summary>
        /// Registers the specified callbacks to be called when the amount of lives changes.
        /// </summary>
        /// <param name="updateCallback">The callback to register for when the number of lives changes.</param>
        /// <param name="finishCallback">The callback to register for when the free life is given.</param>
        public void Subscribe(Action<TimeSpan, int> updateCallback, Action<int> finishCallback)
        {
            onCountdownUpdated += updateCallback;
            onCountdownFinished += finishCallback;
            var maxLives = PuzzleMatchManager.instance.gameConfig.maxLives;
            var numLives = GetCurrentLives();
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
    }
}

