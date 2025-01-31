// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections;
using System.Globalization;

using UnityEngine;
using UnityEngine.Assertions;

using GameVanilla.Core;
using GameVanilla.Game.Popups;
using SaveData;
using System.Threading.Tasks;
using GameVanilla.Game.Common;

namespace GameVanilla.Game.Scenes
{
    /// <summary>
    /// This class contains the logic associated to the home scene.
    /// </summary>
    public class HomeScene : BaseScene
    {
#pragma warning disable 649
        [SerializeField]
        private AnimatedButton soundButton;

        [SerializeField]
        private AnimatedButton musicButton;

        private PlayerProfile playerProfile;
#pragma warning restore 649

        private readonly string dateLastPlayedKey = "date_last_played";
        private readonly string dailyBonusDayKey = "daily_bonus_day";
        
        /// <summary>
        /// Unity's Awake method.
        /// </summary>
        private async void Awake()
        {
            Assert.IsNotNull(soundButton);
            Assert.IsNotNull(musicButton);

            playerProfile = await CloudSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        }

        /// <summary>
        /// Unity's Start method.
        /// </summary>
        private void Start()
        {
            CheckDailyBonus();
            UpdateButtons();
        }

        /// <summary>
        /// Checks the daily bonus.
        /// </summary>
        private void CheckDailyBonus()
        {
            StartCoroutine(CheckDailyBonusAsync());
        }

        /// <summary>
        /// Internal coroutine to check the daily bonus.
        /// </summary>
        private IEnumerator CheckDailyBonusAsync()
        {
            yield return new WaitForSeconds(0.5f);

            var date_last_played = playerProfile.DailyBonus.DateLastPlayed;
            if (date_last_played == "0")
            {
                AwardDailyBonus();

                yield break;
            }
            
            var dateLastPlayedStr = date_last_played;
            var dateLastPlayed = Convert.ToDateTime(dateLastPlayedStr, CultureInfo.CurrentCulture);

            var dateNow = DateTime.Now;
            var diff = dateNow.Subtract(dateLastPlayed);
            if (diff.TotalHours >= 24)
            {
                if (diff.TotalHours < 48)
                {
                    AwardDailyBonus();
                }
                else
                {
                    playerProfile.DailyBonus.DateLastPlayed = "0";
                    playerProfile.DailyBonus.DailyBonusDayKey = "0";
                    yield return CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);

                    AwardDailyBonus();
                }
            }
        }

        /// <summary>
        /// Rewards the player with the corresponding daily bonus.
        /// </summary>
        private async void AwardDailyBonus()
        {
            var dateToday = ServerTimeManager.Instance.CurrentTime;

            var dateLastPlayedStr = Convert.ToString(dateToday, CultureInfo.InvariantCulture);
            // PlayerPrefs.SetString(dateLastPlayedKey, dateLastPlayedStr);
            playerProfile.DailyBonus.DateLastPlayed = dateLastPlayedStr;
            await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);

            // var dailyBonusDay = PlayerPrefs.GetInt(dailyBonusDayKey);
            var dailyBonusDayString = playerProfile.DailyBonus.DailyBonusDayKey;
            var dailyBonusDay = Convert.ToInt32(dailyBonusDayString);

            Debug.Log($"Daily bonus day: {dailyBonusDay}");
            OpenPopup<DailyBonusPopup>("Popups/DailyBonusPopup", popup => { popup.SetInfo(dailyBonusDay); });

            var newDailyBonusDay = (dailyBonusDay + 1) % 7;
            // PlayerPrefs.SetInt(dailyBonusDayKey, newDailyBonusDay);
            playerProfile.DailyBonus.DailyBonusDayKey = newDailyBonusDay.ToString();
            await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);
        }

        /// <summary>
        /// Called when the settings button is pressed.
        /// </summary>
        public void OnSettingsButtonPressed()
        {
            OpenPopup<SettingsPopup>("Popups/SettingsPopup");
        }

        /// <summary>
        /// Called when the sound button is pressed.
        /// </summary>
        public void OnSoundButtonPressed()
        {
            SoundManager.instance.ToggleSound();
        }

        /// <summary>
        /// Called when the music button is pressed.
        /// </summary>
        public void OnMusicButtonPressed()
        {
            SoundManager.instance.ToggleMusic();
        }

        /// <summary>
        /// Updates the state of the UI buttons according to the values stored in PlayerPrefs.
        /// </summary>
        public void UpdateButtons()
        {
            var sound = PlayerPrefs.GetInt("sound_enabled");
            soundButton.transform.GetChild(0).GetComponent<SpriteSwapper>().SetEnabled(sound == 1);
            var music = PlayerPrefs.GetInt("music_enabled");
            musicButton.transform.GetChild(0).GetComponent<SpriteSwapper>().SetEnabled(music == 1);
        }
    }
}
