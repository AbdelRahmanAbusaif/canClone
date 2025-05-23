using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using GameVanilla.Core;
using GameVanilla.Game.Popups;
using GameVanilla.Game.Scenes;
using SaveData;
using GameVanilla.Game.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameVanilla.Game.UI
{
    public class LevelButton : MonoBehaviour
    {
        public int numLevel;

        [SerializeField] private Sprite currentButtonSprite;
        [SerializeField] private Sprite playedButtonSprite;
        [SerializeField] private Sprite lockedButtonSprite;
        [SerializeField] private Sprite yellowStarSprite;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Text numLevelTextBlue;
        [SerializeField] private Text numLevelTextPink;
        [SerializeField] private GameObject star1;
        [SerializeField] private GameObject star2;
        [SerializeField] private GameObject star3;
        [SerializeField] private GameObject shineAnimation;

        private PlayerProfile playerProfile;
        private List<LevelComplete> levelCompleteList;
        private LoopingScroll loopingScroll;

        private int index;
        private int loopValue;
        private int prevLoop = -1;

        private void Awake()
        {
            Assert.IsNotNull(currentButtonSprite);
            Assert.IsNotNull(playedButtonSprite);
            Assert.IsNotNull(lockedButtonSprite);
            Assert.IsNotNull(yellowStarSprite);
            Assert.IsNotNull(buttonImage);
            Assert.IsNotNull(numLevelTextBlue);
            Assert.IsNotNull(numLevelTextPink);
            Assert.IsNotNull(star1);
            Assert.IsNotNull(star2);
            Assert.IsNotNull(star3);
            Assert.IsNotNull(shineAnimation);
        }

        private async void Start()
        {
            loopingScroll = GetComponentInParent<LoopingScroll>();
            index = transform.GetSiblingIndex();

          
            loopValue = loopingScroll != null ? loopingScroll.loop : 0;
            numLevel = index + (200 * loopValue) + 1;

            //
            playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
            levelCompleteList = await LocalSaveManager.Instance.LoadDataAsync<List<LevelComplete>>("LevelsComplete");

            UpdateButton(); // 

            InvokeRepeating(nameof(UpdateScroll), 0.5f, 0.5f); // 
        }
        void Update()
        {
           
        }
        public void UpdateScroll()
        {
            if (loopingScroll == null) return;

            int currentLoop = loopingScroll.loop;
            if (currentLoop != prevLoop)
            {
                prevLoop = currentLoop;
                loopValue = currentLoop;
                numLevel = index + (200 * loopValue) + 1;
                UpdateButton();
            }

           
            string levelStr = numLevel.ToString();
            if (numLevelTextBlue.text != levelStr)
            {
                numLevelTextBlue.text = levelStr;
                numLevelTextPink.text = levelStr;
            }
        }

        public void OnButtonPressed()
        {
            if (buttonImage.sprite == lockedButtonSprite) return;

            var scene = GameObject.Find("LevelScene")?.GetComponent<LevelScene>();
            if (scene != null)
            {
                var numLives = PuzzleMatchManager.instance.livesSystem.GetCurrentLives();
                if (numLives > 0)
                {
                    if (!FileUtils.FileExists("Levels/" + numLevel))
                    {
                        scene.OpenPopup<AlertPopup>("Popups/AlertPopup", popup =>
                            popup.SetText("This level does not exist."));
                    }
                    else
                    {
                        scene.OpenPopup<StartGamePopup>("Popups/StartGamePopup", popup =>
                            popup.LoadLevelData(numLevel));
                    }
                }
                else
                {
                    scene.OpenPopup<BuyLivesPopup>("Popups/BuyLivesPopup");
                }
            }
        }

        private async void UpdateButton()
        {
            if (playerProfile == null)
                return;

            int nextLevel = playerProfile.Level > 0 ? playerProfile.Level : 1;

            string levelStr = numLevel.ToString();
            numLevelTextBlue.text = levelStr;
            numLevelTextPink.text = levelStr;

            if (numLevel == nextLevel)
            {
                buttonImage.sprite = currentButtonSprite;
                star1.SetActive(false);
                star2.SetActive(false);
                star3.SetActive(false);
                shineAnimation.SetActive(true);
                numLevelTextPink.gameObject.SetActive(true);
                numLevelTextBlue.gameObject.SetActive(false);
            }
            else if (numLevel < nextLevel)
            {
                buttonImage.sprite = playedButtonSprite;
                numLevelTextPink.gameObject.SetActive(true);
                numLevelTextBlue.gameObject.SetActive(false);
                shineAnimation.SetActive(false);

                int stars = levelCompleteList != null && levelCompleteList.FirstOrDefault(x => x.NumberLevel == numLevel) != null
                ? levelCompleteList.FirstOrDefault(x => x.NumberLevel == numLevel).Stars 
                : 0;

                if (stars >= 1) star1.GetComponent<Image>().sprite = yellowStarSprite;
                if (stars >= 2) star2.GetComponent<Image>().sprite = yellowStarSprite;
                if (stars == 3) star3.GetComponent<Image>().sprite = yellowStarSprite;

                star1.SetActive(stars >= 1);
                star2.SetActive(stars >= 2);
                star3.SetActive(stars == 3);
            }
            else
            { 
                buttonImage.sprite = lockedButtonSprite;
                numLevelTextBlue.gameObject.SetActive(true);
                numLevelTextPink.gameObject.SetActive(false);
                star1.SetActive(false);
                star2.SetActive(false);
                star3.SetActive(false);
                shineAnimation.SetActive(false);
            }
        }
    }
}
