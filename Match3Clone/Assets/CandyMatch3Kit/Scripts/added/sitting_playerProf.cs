// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using GameVanilla.Core;
using GameVanilla.Game.Common;
using GameVanilla.Game.Scenes;
using Unity.Services.Core;
using Unity.Services.Authentication;

using SaveData;

namespace GameVanilla.Game.Popups
{
    /// <summary>
    /// This class contains the logic associated to the settings popup.
    /// </summary>
    public class sitting_playerProf : Popup
    {
#pragma warning disable 649
        [SerializeField]
        private Image avatarImage;

        [SerializeField]
        private int anonymousValue;

#pragma warning restore 649

        private int currentAvatar;
        private CloudSaveManager cloudSaveManager;
        private PlayerProfile playerProfile;
        /// <summary>
        /// Unity's Awake method.
        /// </summary>
        protected override async void Awake()
        {

            base.Awake();
        

            await UnityServices.Instance.InitializeAsync();

            // if player Sign in with unity account, load the player profile images

            // cloudSaveManager.LoadImageAsync("PlayerProfileImage", avatarImage);
            // playerProfile = await cloudSaveManager.LoadDataAsync<PlayerProfile>("PlayerProfile");

            //else load the default avatar
        }

        /// <summary>
        /// Unity's Start method.
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Called when the close button is pressed.
        /// </summary>
        public void OnCloseButtonPressed()
        {
            Close();
        }

       
    }
}
