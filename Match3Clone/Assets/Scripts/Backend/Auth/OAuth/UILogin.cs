using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameVanilla.Core;
using SaveData;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;
using static RemotelyDownloadAssets;

public class UILogin : MonoBehaviour
{
    public static Action OnSignIn;
    public Action OnSignUp;
    [SerializeField] private Button signInAnynoumuse;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private LoginController loginController;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject loadingPanel;
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

        if(signInAnynoumuse != null)
        {
            signInAnynoumuse.onClick.AddListener(OnSignInButtonClicked);
        }

        loginController.OnSignInSuccess += OnSignInSuccess;
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
    }

    public void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully!");
        
        isBlueLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsBlueLeaderboard");
        isRedLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsRedLeaerboard");
        isGreenLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsGreenLeaderboard");
        isOrangeLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsOrangeLeadeboard");
        isYellowLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsYellowLeaderboard");
        isPurpleLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsPurpleLeaderboard");
    }

    private void OnSignInButtonClicked()
    {
        loginController.InitSignAnonymous();
        // await loginController.InitSign();
    }

    private async void OnSignInSuccess(PlayerProfile playerData)
    {
        loadingPanel.SetActive(true);
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
		if (isBlueLeaderboard)
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

            playerData = await CloudSaveManager.Instance.LoadPublicDataAsync<PlayerProfile>("PlayerProfile");
            heartSystem = await CloudSaveManager.Instance.LoadDataAsync<HeartSystem>("HeartSystem");
            dailyBonus = await CloudSaveManager.Instance.LoadDataAsync<DailyBonus>("DailyBonus");
            spinWheel = await CloudSaveManager.Instance.LoadDataAsync<SpinWheel>("SpinWheel");
            primeSubscription = await CloudSaveManager.Instance.LoadDataAsync<ConsumableItem>("PrimeSubscriptions");
            containerProfileAvatar = await CloudSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileAvatarImages");
            containerProfileCover = await CloudSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileCoverImages");
            containerProfileBorder = await CloudSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileBorders");
            LevelCompletes = await CloudSaveManager.Instance.LoadDataAsync<List<LevelComplete>>("LevelsComplete");
            AdManagers = await CloudSaveManager.Instance.LoadDataAsync<List<AdManager>>("AdManager");

			OnSignIn?.Invoke();
            loadingPanel.SetActive(false);
            return;
        }
        if (termPanel != null)
        {
            loadingPanel.SetActive(true);

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
            loadingPanel.SetActive(false);
        }
        else
        {
            OnSignUp?.Invoke();
            loadingPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    private void OnDestroy() {

        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
        loginController.OnSignInSuccess -= OnSignInSuccess;
        if(signInAnynoumuse !=null)
        {
            signInAnynoumuse.onClick.RemoveListener(OnSignInButtonClicked);
        }
    }

}