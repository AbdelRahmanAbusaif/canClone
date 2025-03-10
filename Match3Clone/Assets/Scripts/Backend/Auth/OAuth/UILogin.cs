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
    [SerializeField] private Texture2D defaultBorderImage;
    [SerializeField] private Texture2D defaultBackgroundImage;
    
    bool isBlueLeaderboard = false;
    bool isRedLeaderboard = false;
    bool isGreenLeaderboard = false;
    bool isYellowLeaderboard = false;
    bool isOrangeLeaderboard = false;
    bool isPurpleLeaderboard = false;

    private async void OnEnable() {

        if(signInButton !=null)
        {
            signInButton.onClick.AddListener(OnSignInButtonClicked);
        }

        loginController.OnSignInSuccess += OnSignInSuccess;

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
    }

    private void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully!");
        
        isBlueLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsBlueLeaderboard");
        isRedLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsRedLeaderboard");
        isGreenLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsGreenLeaderboard");
        isOrangeLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsOrangeLeaderboard");
        isYellowLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsYellowLeaderboard");
        isPurpleLeaderboard = RemoteConfigService.Instance.appConfig.GetBool("IsPurpleLeaderboard");
    }

    private async void OnSignInButtonClicked()
    {
        Debug.Log("Sign in button clicked");
        await loginController.InitSign();
    }

    private async void OnSignInSuccess(PlayerProfile playerData)
    {
        Debug.Log("Sign in success");
        // Here will be the code for get the player info and save it to the database
        
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerProfile" });
        var dataImage = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerProfileImage" }, new Unity.Services.CloudSave.Models.Data.Player.LoadOptions(new Unity.Services.CloudSave.Models.Data.Player.PublicReadAccessClassOptions()));

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
            // loginPanel.SetActive(true);
            termPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Login panel is not assigned");

            await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerData);
            await CloudSaveManager.Instance.SaveImageAsync("PlayerProfileImage", defaultImage);
            await CloudSaveManager.Instance.SaveImageAsync("PlayerProfileBorderImage", defaultBorderImage);
            await CloudSaveManager.Instance.SaveImageAsync("PlayerProfileBackgroundImage", defaultBackgroundImage);

            OnSignUp?.Invoke();
        }
    }

    // Update is called once per frame
    private void OnDisable() {

        if(signInButton !=null)
        {
            signInButton.onClick.RemoveListener(OnSignInButtonClicked);
        }
        loginController.OnSignInSuccess -= OnSignInSuccess;
    }

}