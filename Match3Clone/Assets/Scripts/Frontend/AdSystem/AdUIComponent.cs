using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameVanilla.Game.Common;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdUIComponent : MonoBehaviour 
{
    public string AdId;
    public TextMeshProUGUI goalClicksText;
    public TextMeshProUGUI currentClicksText;
    public TextMeshProUGUI rewardText;
    public Button adButton;

    private GoogleAdManager googleAdManager;
    private List<AdManager> adManagers;
    private AdData adData;

    private async void Start()
    {
        adManagers = await LocalSaveManager.Instance.LoadDataAsync<List<AdManager>>("AdManager");

        adButton.onClick.AddListener(OnClick);

        googleAdManager = FindAnyObjectByType<GoogleAdManager>().GetComponent<GoogleAdManager>();
        googleAdManager.OnAdLoaded += OnAdLoaded;

        if(adManagers.Find(x => x.AdId == AdId) != null)
        {
            AdManager adManager = adManagers.Find(x => x.AdId == AdId);
            if(adManager.AdCounter >= adData.AdClicksGoal)
            {
                adButton.interactable = false;
            }
        }
    }

    private async void OnAdLoaded(string id)
    {
        if (id == AdId)
        {
            Debug.Log("Ad Loaded with ID: " + id + " and AdId: " + AdId + " and AdButton: " + adButton.name + " and AdButtonActive: " + adButton.gameObject.activeSelf);

            AdManager adManager = adManagers.Find(x => x.AdId == AdId);
            
            if (adManager != null)
            {
                adManager.AdCounter++;
                // Check if the counter is less than the goal
                if (adManager.AdCounter >= adData.AdClicksGoal)
                {
                    adButton.interactable = false;

                    // Reward the player
                    int reward = adData.AdReward;

                    PuzzleMatchManager.instance.coinsSystem.BuyCoins(reward);
                    
                    Debug.Log("Rewarded Player with: " + reward + " coins");
                    Debug.Log("Ad Counter: " + adManager.AdCounter + " for AdId: " + adManager.AdId);
					adManager.AdCurrentTimer = ServerTimeManager.Instance.CurrentTime.Date.ToString();
                    adManager.AdNextTimer = ServerTimeManager.Instance.CurrentTime.Date.AddDays(1).ToString();

                    adManagers[adManagers.FindIndex(x => x.AdId == AdId)] = adManager;
					Debug.Log("Ad Reward: " + reward);
                }
                else
                {
                    // will be plus 1 to the counter and save the data
                    adButton.interactable = true;
                    Debug.Log("Ad Counter: " + adManager.AdCounter + " for AdId: " + adManager.AdId);
                    adManagers[adManagers.FindIndex(x => x.AdId == AdId)] = adManager;

                }
                currentClicksText.text = adManager.AdCounter.ToString();
                await CloudSaveManager.Instance.SaveDataAsync<List<AdManager>>("AdManager", adManagers);           
            }
        }
    }

    private void OnClick()
    {
        Debug.Log("Open Ad");
        adButton.interactable = false;
        OpenAd();
    }

    private void OpenAd()
    {
        Debug.Log("Ad Opened");
        googleAdManager.ShowRewardedAd(this);
    }

    public void SetAdData(string Id,string goalClicks, string currentClicks, string reward)
    {
        AdId = Id;
        goalClicksText.text = goalClicks;
        currentClicksText.text = currentClicks;
        rewardText.text = reward;

        adData = new AdData
        {
            AdId = AdId,
            AdClicksGoal = int.Parse(goalClicks),
            AdReward = int.Parse(reward)
        };
    }
    private void OnDestroy() 
    {
        adButton.onClick.RemoveListener(OnClick);
        googleAdManager.OnAdLoaded -= OnAdLoaded;
    }
}
