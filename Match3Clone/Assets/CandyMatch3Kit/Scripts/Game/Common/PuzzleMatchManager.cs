// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

using FullSerializer;

using GameVanilla.Core;
using SaveData;
using Unity.Services.RemoteConfig;
using System;
using static RemotelyDownloadAssets;
using Unity.Services.Core;
using Newtonsoft.Json;

namespace GameVanilla.Game.Common
{
    /// <summary>
    /// This class is a utility class that allows other classes to easily access the game configuration and
    /// the lives and coins systems.
    /// </summary>
    public class PuzzleMatchManager : MonoBehaviour
    {
        public static PuzzleMatchManager instance;

        public GameConfiguration gameConfig;

        public LivesSystem livesSystem;
        public CoinsSystem coinsSystem;

        public int lastSelectedLevel;
        public bool unlockedNextLevel;

         
        public IapManager iapManager;
    

        /// <summary>
        /// Unity's Awake method.
        /// </summary>
        private async void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);

            if (!PlayerPrefs.HasKey("sound_enabled"))
            {
                PlayerPrefs.SetInt("sound_enabled", 1);
            }
            if (!PlayerPrefs.HasKey("music_enabled"))
            {
                PlayerPrefs.SetInt("music_enabled", 1);
            }
            
            livesSystem = GetComponent<LivesSystem>();
            coinsSystem = GetComponent<CoinsSystem>();

            await UnityServices.Instance.InitializeAsync();

            // if(!PlayerPrefs.HasKey("next_live_time"))
            // {
            //     PlayerPrefs.SetString("next_live_time", "300");
            // }
            // if (!PlayerPrefs.HasKey("num_lives"))
            // {
            //     PlayerPrefs.SetInt("num_lives", gameConfig.maxLives);
            // }
            // if (!PlayerPrefs.HasKey("num_coins"))
            // {
            //     PlayerPrefs.SetInt("num_coins", gameConfig.initialCoins);
            // }

        }
        private void OnEnable() 
        {
            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        }
        private void ApplyRemoteConfig(ConfigResponse response)
        {
            GameConfiguration gameConfig = JsonConvert.DeserializeObject<GameConfiguration>(RemoteConfigService.Instance.appConfig.GetJson("game_configuration"));
            Debug.Log("Remote Config Items Store:" + gameConfig.iapItems.Count);

            Debug.Log("Remote Config Fetched Successfully!");

            if (gameConfig != null)
            {
                Debug.Log("Game Configuration: " + gameConfig);
                this.gameConfig = gameConfig;
                iapManager = new IapManager();
            }
        }

        private void OnDisable() 
        {
            RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
        }
    }
}
