// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using GameVanilla.Core;
using GameVanilla.Game.Common;
using GameVanilla.Game.Popups;
using GameVanilla.Game.UI;
using System.Threading.Tasks;
using SaveData;
using System;


namespace GameVanilla.Game.Scenes
{
    /// <summary>
    /// This class contains the logic associated to the level scene.
    /// </summary>
    public class LevelScene : BaseScene
    {
#pragma warning disable 649
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private GameObject scrollView;

        [SerializeField]
        private GameObject avatarPrefab;

        [SerializeField]
        private GameObject rewardedAdButton;

        [SerializeField]
        private Text scoreText;

        public int score;

        PlayerProfile playerProfile;

#pragma warning restore 649

        /// <summary>
        /// Unity's Awake method.
        /// </summary>
        private void Awake()
        {
            Assert.IsNotNull(scrollRect);
            Assert.IsNotNull(scrollView);
            Assert.IsNotNull(avatarPrefab);
            Assert.IsNotNull(rewardedAdButton);
        }

        /// <summary>
        /// Unity's Start method.
        /// </summary>
        private async void Start()
        {
           
            playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

           
            var nextLevel = playerProfile.Level;


           Debug.Log("nextLevel from levelstart: " + nextLevel);

            scrollRect.vertical = false;

            var avatar = Instantiate(avatarPrefab);
            avatar.transform.SetParent(scrollView.transform, false);

            

            if (nextLevel == 0)
            {
                nextLevel = 1;
                
            }
            score = nextLevel * 10;
            // scoreText.text = score.ToString();


            ////////////// 
            LevelButton prevButton = null;
           

            LevelButton currentButton = null;
            var levelButtons = scrollView.GetComponentsInChildren<LevelButton>();
            foreach (var button in levelButtons)
            {
                if (button.numLevel != nextLevel %200)
                {
                    continue;
                }
                currentButton = button;
                break;
            }
            
            if (currentButton == null)
            {
                 currentButton = levelButtons[levelButtons.Length - 1];
                
            }

            var newPos = scrollView.GetComponent<RectTransform>().anchoredPosition;
           
            newPos.y =
             scrollRect.transform.InverseTransformPoint(scrollView.GetComponent<RectTransform>().position).y
              - scrollRect.transform.InverseTransformPoint(currentButton.transform.position).y;
           
            if (newPos.y < scrollView.GetComponent<RectTransform>().anchoredPosition.y)
            {
                scrollView.GetComponent<RectTransform>().anchoredPosition = newPos;
            }

            var targetPos = currentButton.transform.position + new Vector3(0, 1.0f, 0);

           
            if (PuzzleMatchManager.instance.unlockedNextLevel)
            {
                foreach (var button in scrollView.GetComponentsInChildren<LevelButton>())
                {
                    if (button.numLevel != PuzzleMatchManager.instance.lastSelectedLevel)
                    {
                        // continue;
                       
                    }
                    prevButton = button;
                    break;
                }
            }
           

            if (prevButton != null)
            {
                avatar.transform.position = prevButton.transform.position + new Vector3(0, 1.0f, 0);
                var sequence = LeanTween.LeanTween.sequence();
                sequence.append(0.5f);
                sequence.append(LeanTween.LeanTween.move(avatar, targetPos, 0.8f));
                sequence.append(() => avatar.GetComponent<LevelAvatar>().StartFloatingAnimation());
                sequence.append(() => scrollRect.vertical = true);
            }
            else 
            {
                avatar.transform.position = targetPos;
                avatar.GetComponent<LevelAvatar>().StartFloatingAnimation();
                scrollRect.vertical = true;
            }
        }

        public void OnSpinWheelButtonPressed()
        {
            OpenPopup<SpinWheelPopup>("Popups/SpinWheelPopup", popup =>
            {
                var gameConfig = PuzzleMatchManager.instance.gameConfig;
                popup.SetInfo(gameConfig.spinWheelItems, gameConfig.spinWheelCost);
            });
        }
        public void OnLeaderBoardPressed()
        {
            // OpenPopup<BuyCoinsPopup>("Popups/LB_popup");
        }
    }
}
