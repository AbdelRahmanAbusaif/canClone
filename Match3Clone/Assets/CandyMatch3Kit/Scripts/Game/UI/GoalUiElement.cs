// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

using GameVanilla.Core;
using GameVanilla.Game.Common;
using SaveData;
using System.Threading.Tasks;
using Unity.Mathematics;
using System.IO;

namespace GameVanilla.Game.UI
{
    /// <summary>
    /// This class manages a single goal element within the in-game user interface for goals.
    /// </summary>
    public class GoalUiElement : MonoBehaviour
    {
        public Image image;
        public Text amountText;
        public Image tickImage;
        public Image crossImage;

        public bool isCompleted { get; private set; }

        private Goal currentGoal;
        private int targetAmount;
        private int currentAmount;

        /// <summary>
        /// Unity's Awake method.
        /// </summary>
        private void Awake()
        {
            tickImage.gameObject.SetActive(false);
            crossImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Fills this element with the information of the specified goal.
        /// </summary>
        /// <param name="goal">The associated goal.</param>
        public virtual async void Fill(Goal goal)
        {
            currentGoal = goal;
            if (goal is CollectCandyGoal)
            {
            var candyGoal = (CollectCandyGoal)goal;
            var path = "";
            switch(candyGoal.candyType)
            {
                case CandyColor.Blue:
                path = Path.Combine("DownloadedAssets", "BlueCandy");
                break;
                case CandyColor.Green:
                path = Path.Combine("DownloadedAssets", "GreenCandy");
                break;
                case CandyColor.Orange:
                path = Path.Combine("DownloadedAssets", "OrangeCandy");
                break;
                case CandyColor.Purple:
                path = Path.Combine("DownloadedAssets", "PurpleCandy");
                break;
                case CandyColor.Red:
                path = Path.Combine("DownloadedAssets", "RedCandy");
                break; 
                case CandyColor.Yellow:
                path = Path.Combine("DownloadedAssets", "YellowCandy");
                break;
            }
            Debug.Log(path);
            var sprite = await LocalSaveManager.Instance.LoadSpriteAsync(path);
            var texture = sprite.texture;
            if (texture != null)
            {
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100);
            }
            targetAmount = candyGoal.amount;
            amountText.text = targetAmount.ToString();
            }
            else if (goal is CollectElementGoal)
            {
            var elementGoal = (CollectElementGoal)goal;
            var path = "";
            switch(elementGoal.elementType)
            {
                case ElementType.Honey:
                path = Path.Combine("DownloadedAssets", "Honey");
                break;
                case ElementType.Ice:
                path = Path.Combine("DownloadedAssets", "Ice");
                break;
                case ElementType.Syrup1:
                path = Path.Combine("DownloadedAssets", "Syrup1");
                break;
                case ElementType.Syrup2:
                path = Path.Combine("DownloadedAssets", "Syrup2");
                break;
            }
            var sprite = await LocalSaveManager.Instance.LoadSpriteAsync(path);
            var texture = sprite.texture;
            if (texture != null)
            {
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100);
            }
            targetAmount = elementGoal.amount;
            amountText.text = targetAmount.ToString();
            }
            else if (goal is CollectSpecialBlockGoal)
            {
            var specialGoal = (CollectSpecialBlockGoal)goal;
            var path = "";

            switch(specialGoal.specialBlockType)
            {
                case SpecialBlockType.Chocolate:
                path = Path.Combine("DownloadedAssets", "Chocolate");
                break;
                case SpecialBlockType.Marshmallow:
                path = Path.Combine("DownloadedAssets", "Marshmallow");
                break;
                case SpecialBlockType.Unbreakable:
                path = Path.Combine("DownloadedAssets", "Unbreakable");
                break;
            }
            var sprite = await LocalSaveManager.Instance.LoadSpriteAsync(path);
            var texture = sprite.texture;
            if (texture != null)
            {
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100);
            }
            targetAmount = specialGoal.amount;
            amountText.text = targetAmount.ToString();
            }
            else if (goal is CollectCollectableGoal)
            {
            var collectableGoal = (CollectCollectableGoal)goal;
            var path = "";
            switch(collectableGoal.collectableType)
            {
                case CollectableType.Cherry:
                path = Path.Combine("DownloadedAssets", "Cherry");
                break;
                case CollectableType.Watermelon:
                path = Path.Combine("DownloadedAssets", "Watermelon");
                break;
            }
            var sprite = await LocalSaveManager.Instance.LoadSpriteAsync(path);
            var texture = sprite.texture;
            if (texture != null)
            {
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100);
            }
            targetAmount = collectableGoal.amount;
            amountText.text = targetAmount.ToString();
            }
            else if (goal is DestroyAllChocolateGoal)
            {
            var path = Path.Combine("DownloadedAssets", "Chocolate");
            var sprite = await LocalSaveManager.Instance.LoadSpriteAsync(path);
            var texture = sprite.texture;
            if (texture != null)
            {
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100);
            }
            amountText.text = "All";
            }
        }

        /// <summary>
        /// Updates this element based on the current state of the game.
        /// </summary>
        /// <param name="state">The current game state.</param>
        public virtual void UpdateGoal(GameState state)
        {
            var chocolateGoal = currentGoal as DestroyAllChocolateGoal;
            if (chocolateGoal != null)
            {
                if (state.destroyedAllChocolates && !chocolateGoal.completed)
                {
                    chocolateGoal.completed = true;
                    SetCompletedTick(true);
                    SoundManager.instance.PlaySound("ReachedGoal");
                    return;
                }
            }

            if (currentAmount == targetAmount)
            {
                return;
            }

            var newAmount = 0;
            var candyGoal = currentGoal as CollectCandyGoal;
            if (candyGoal != null)
            {
                newAmount = state.collectedCandies[candyGoal.candyType];
            }
            else
            {
                var elementGoal = currentGoal as CollectElementGoal;
                if (elementGoal != null)
                {
                    newAmount = state.collectedElements[elementGoal.elementType];
                }
                else
                {
                    var specialGoal = currentGoal as CollectSpecialBlockGoal;
                    if (specialGoal != null)
                    {
                        newAmount = state.collectedSpecialBlocks[specialGoal.specialBlockType];
                    }
                    else
                    {
                        var collectableGoal = currentGoal as CollectCollectableGoal;
                        if (collectableGoal != null)
                        {
                            newAmount = state.collectedCollectables[collectableGoal.collectableType];
                        }
                    }
                }
            }

            if (newAmount == currentAmount)
            {
                return;
            }

            currentAmount = newAmount;
            if (currentAmount >= targetAmount)
            {
                currentAmount = targetAmount;
                SetCompletedTick(true);
                SoundManager.instance.PlaySound("ReachedGoal");
            }
            amountText.text = (targetAmount - currentAmount).ToString();
            if (amountText.gameObject.activeSelf)
            {
                amountText.GetComponent<Animator>().SetTrigger("GoalAchieved");
            }
        }

        /// <summary>
        /// Sets the goal tick as completed/not completed.
        /// </summary>
        /// <param name="completed">True if the completion tick should be shown; false otherwise.</param>
        public void SetCompletedTick(bool completed)
        {
            isCompleted = completed;
            amountText.gameObject.SetActive(false);
            if (completed)
            {
                tickImage.gameObject.SetActive(true);
                image.GetComponent<Animator>().SetTrigger("GoalAchieved");
                tickImage.GetComponent<Animator>().SetTrigger("GoalAchieved");
            }
            else
            {
                crossImage.gameObject.SetActive(true);
            }
        }
    }
}
