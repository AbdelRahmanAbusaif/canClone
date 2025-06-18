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
using System.Collections.Generic;
using System.IO;

namespace GameVanilla.Game.Scenes
{
    /// <summary>
    /// This class contains the logic associated to the home scene.
    /// </summary>
    public class HomeScene : BaseScene
    {
#pragma warning disable 649
        // [SerializeField]
        // private AnimatedButton soundButton;

        [SerializeField]
        private AnimatedButton musicButton;

        private PlayerProfile playerProfile;
        private DailyBonus dailyBonus;
#pragma warning restore 649

        private readonly string dateLastPlayedKey = "date_last_played";
        private readonly string dailyBonusDayKey = "daily_bonus_day";
        
        /// <summary>
        /// Unity's Awake method.
        /// </summary>
        private void Awake()
        {
            // Assert.IsNotNull(soundButton);
            Assert.IsNotNull(musicButton);
        }

        /// <summary>
        /// Unity's Start method.
        /// </summary>
        private async void Start()
        {
            dailyBonus = await LocalSaveManager.Instance.LoadDataAsync<DailyBonus>("DailyBonus");
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
            Debug.Log("Checking daily bonus...");

            // GetPlayerProfile();
            // yield return new WaitUntil(() => playerProfile != null);

            var date_last_played = dailyBonus.DateLastPlayed;
            Debug.Log($"Date last played from Home Screen: {date_last_played}");

            if (date_last_played == "0" || string.IsNullOrEmpty(date_last_played))
            {
                dailyBonus.DateLastPlayed = "0";
                dailyBonus.DailyBonusDayKey = "0";
                
                AwardDailyBonus();

                yield break;
            }
            
            var dateLastPlayedStr = date_last_played;
            var dateLastPlayed = Convert.ToDateTime(dateLastPlayedStr);
            Debug.Log($"Date last played: {dateLastPlayed}");

            var dateNow = ServerTimeManager.Instance.CurrentTime.Date;

            Debug.Log($"Date now from Home Screen: {dateNow}");
            var diff = dateNow.Subtract(dateLastPlayed);

            Debug.Log($"Difference from Home Screen: {diff}");

            if (diff.TotalHours >= 24)
            {
                if (diff.TotalHours < 48)
                {
                    AwardDailyBonus();
                }
                else
                {
                    dailyBonus.DateLastPlayed = "0";
                    dailyBonus.DailyBonusDayKey = "0";

                    yield return CloudSaveManager.Instance.SaveDataAsync("DailyBonus", dailyBonus);

                    AwardDailyBonus();
                }
            }
        }

        /// <summary>
        /// Rewards the player with the corresponding daily bonus.
        /// </summary>
        private async void AwardDailyBonus()
        {
            var dateToday = ServerTimeManager.Instance.CurrentTime.Date;
            Debug.Log($"Date today DateTime: {dateToday}");
            var dateLastPlayedStr = Convert.ToString(dateToday);

            Debug.Log($"Date today: {dateToday}");

            // PlayerPrefs.SetString(dateLastPlayedKey, dateLastPlayedStr);
            dailyBonus.DateLastPlayed = dateLastPlayedStr;

            Debug.Log($"Date last played from Home Screen: {dateLastPlayedStr}");
                        
            // var dailyBonusDay = PlayerPrefs.GetInt(dailyBonusDayKey);
            var dailyBonusDayString = dailyBonus.DailyBonusDayKey;
            var dailyBonusDay = Convert.ToInt32(dailyBonusDayString);

            Debug.Log($"Daily bonus day: {dailyBonusDay}");
            var newDailyBonusDay = (dailyBonusDay + 1) % 7;
            // PlayerPrefs.SetInt(dailyBonusDayKey, newDailyBonusDay);
            dailyBonus.DailyBonusDayKey = newDailyBonusDay.ToString();

            Debug.Log($"New daily bonus day: {newDailyBonusDay}");

            // Only the first 7 days are available for the daily bonus
            int key = Int32.Parse(dailyBonus.DailyBonusDayKey);
            if (key <= 7)
            {
                PuzzleMatchManager.instance.notificationController.ScheduleNotification("يوم جديد", "لا تنسى الحصول على مكافأتك اليومية!", DateTime.Parse(dailyBonus.DateLastPlayed).AddDays(1));
            }
            
            await CloudSaveManager.Instance.SaveDataAsync("DailyBonus", dailyBonus);

            OpenPopup<DailyBonusPopup>("Popups/DailyBonusPopup", popup => { popup.SetInfo(dailyBonusDay); });
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
            // soundButton.transform.GetChild(0).GetComponent<SpriteSwapper>().SetEnabled(sound == 1);
            var music = PlayerPrefs.GetInt("music_enabled");
            Debug.Log("Music enabled: " + music);
            musicButton.transform.GetChild(0).GetComponent<SpriteSwapper>().SetEnabled(music == 1);
        }
    }
}
