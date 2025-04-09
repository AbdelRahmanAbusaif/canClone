using System;
using System.Collections.Generic;
using GameVanilla.Core;
using SaveData;
using Unity.Services.CloudSave;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;
using static RemotelyDownloadAssets;

public class UILogin : MonoBehaviour
{
    public static Action OnSignIn;
    public Action OnSignUp;
    [SerializeField] private Button signInButton;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private LoginController loginController;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject termPanel;
    [SerializeField] private Texture2D defaultImage;
    [SerializeField] private Texture2D borderImage;
    [SerializeField] private Texture2D profileCoverImage;
    
    bool isBlueLeaderboard = false;
    bool isRedLeaderboard = false;
    bool isGreenLeaderboard = false;
    bool isYellowLeaderboard = false;
    bool isOrangeLeaderboard = false;
    bool isPurpleLeaderboard = false;

    private void OnEnable() 
    {

        if(signInButton !=null)
        {
            signInButton.onClick.AddListener(OnSignInButtonClicked);
        }

        loginController.OnSignInSuccess += OnSignInSuccess;
    }

    public void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully!");
        
        isBlueLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsBlueLeaderboard");
        isRedLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsRedLeaderboard");
        isGreenLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsGreenLeaderboard");
        isOrangeLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsOrangeLeaderboard");
        isYellowLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsYellowLeaderboard");
        isPurpleLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsPurpleLeaderboard");
    }

    private void OnSignInButtonClicked()
    {
        Debug.Log("Sign in button clicked");
        // await loginController.InitSign();
    }

    private async void OnSignInSuccess(PlayerProfile playerData)
    {
        Debug.Log("Sign in success");
        // Here will be the code for get the player info and save it to the database
        
        // Here will be the code for get the player info and save it to the database
        // Here will be the Save Data to the database as public data
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerProfile" }, new Unity.Services.CloudSave.Models.Data.Player.LoadOptions(new Unity.Services.CloudSave.Models.Data.Player.PublicReadAccessClassOptions()));
        var dataImage = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerProfileImage" }, new Unity.Services.CloudSave.Models.Data.Player.LoadOptions(new Unity.Services.CloudSave.Models.Data.Player.PublicReadAccessClassOptions()));

        // Here will be the code for get the player info and save it to the database
        // Here will be the Save Data to the database as private data
        // Here will be the code for get the player info and save it to the database

        //Seed the default data
        var heartSystem = new HeartSystem{
            Heart = 5,
            LastHeartTime = "0",
            NextHeartTime = "0",
        };
        var dailyBonus = new DailyBonus{
            DailyBonusDayKey = "0",
            DateLastPlayed = "0"
        };
        var spinWheel = new SpinWheel{
            DailySpinDayKey = "0",
            DateLastSpin = "0"
        };
        var primeSubscription = new ConsumableItem
        {
            Id = "PrimeSubscription",
            ConsumableName = "PrimeSubscription",
            DatePurchased = "0",
            DateExpired = "0"
        };

        var containerProfileAvatar = new List<ConsumableItem>();
        var containerProfileCover = new List<ConsumableItem>();
        var containerProfileBorder = new List<ConsumableItem>();
        var AdManagers = new List<AdManager>();

        var LevelCompletes = new List<LevelComplete>();
        
        // LeaderboardManager.Instance.AddScore(0);
        if(isBlueLeaderboard)
        {
            LeaderboardManager.Instance.AddScore("BLUE_LEADERBOARD",0);
        }
        if(isRedLeaderboard)
        {
            LeaderboardManager.Instance.AddScore("RED_LEADERBOARD",0);
        }
        if(isGreenLeaderboard)
        {
            LeaderboardManager.Instance.AddScore("GREEN_LEADERBOARD",0);
        }
        if(isYellowLeaderboard)
        {
            LeaderboardManager.Instance.AddScore("YELLOW_LEADERBOARD",0);
        }
        if(isOrangeLeaderboard)
        {
            LeaderboardManager.Instance.AddScore("ORANGE_LEADERBOARD",0);
        }
        if(isPurpleLeaderboard)
        {
            LeaderboardManager.Instance.AddScore("PURPLE_LEADERBOARD",0);
        }
        
        if(data.ContainsKey("PlayerProfile") && dataImage.ContainsKey("PlayerProfileImage"))
        {
            Debug.Log("Player profile already exists");

            OnSignIn?.Invoke();
            return;
        }
        if(termPanel != null)
        {
            
            await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);
            await CloudSaveManager.Instance.SaveDataAsync("DailyBonus", dailyBonus);
            await CloudSaveManager.Instance.SaveDataAsync("SpinWheel", spinWheel);
            await CloudSaveManager.Instance.SaveDataAsync("PrimeSubscriptions", primeSubscription);
            await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileAvatarImages", containerProfileAvatar);
            await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileCoverImages", containerProfileCover);
            await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileBorders", containerProfileBorder);
            await CloudSaveManager.Instance.SaveDataAsync("LevelsComplete", LevelCompletes);
            await CloudSaveManager.Instance.SaveDataAsync("AdManager", AdManagers);

            await CloudSaveManager.Instance.SavePublicDataAsync("PlayerProfile", playerData);

            termPanel.SetActive(true);
        }
        else
        {
            // This when the plyer in already accepted the terms and conditions
            // and when player in loading page
            await CloudSaveManager.Instance.SavePublicDataAsync<PlayerProfile>("PlayerProfile", playerData);

            OnSignUp?.Invoke();
        }
    }

    // Update is called once per frame
    private void OnDestroy() {

        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
        loginController.OnSignInSuccess -= OnSignInSuccess;
        if(signInButton !=null)
        {
            signInButton.onClick.RemoveListener(OnSignInButtonClicked);
        }
    }

}