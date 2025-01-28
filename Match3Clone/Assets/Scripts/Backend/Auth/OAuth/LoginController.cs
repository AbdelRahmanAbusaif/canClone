using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using SaveData;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;

public class LoginController : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignInSuccess;
    public static event Action OnSignedOutSuccess;
    public GameObject LoadingPanel;
    private CloudSaveManager cloudSaveManager;

    async private void Awake() {
        cloudSaveManager = FindAnyObjectByType<CloudSaveManager>().GetComponent<CloudSaveManager>();

        await UnityServices.Instance.InitializeAsync();

        if(SceneManager.GetActiveScene().buildIndex.Equals(0))
        {
            await SignInCachedUserAsync();
        }

        PlayerAccountService.Instance.SignedIn += OnSignedIn;
        PlayerAccountService.Instance.SignedOut += () => {Debug.Log("Signed out successfully.");};
    }

    private async void OnSignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);

            Debug.Log("Access token: " + accessToken);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task InitSign()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }
    public void InitSignOut()
    {
        try
        {
            AuthenticationService.Instance.SignOut(true);

            LocalSaveManager.Instance.DeleteData("PlayerProfile");
            LocalSaveManager.Instance.DeleteImage("PlayerProfileImage");

            OnSignedOutSuccess?.Invoke();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);            
        }
        
    }

    public async void SignInAnonymousButton()
    {
        await SignInAnonymous();
    }
    public async Task SignInAnonymous()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            var playerProfile = new PlayerProfile
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                PlayerName = name,
                Email = "",
                PhoneNumber = "",
                Level = 1,
                LastHeartTime = "0",
                DailyBonus = new DailyBonus()
                {
                    DateLastPlayed = "0",
                    DailyBonusDayKey = "0"
                },
                LevelsComplete = new List<LevelComplete>()
                {
                    
                }
            };

            PlayerPrefs.SetInt("IsAnonymous", 1);
            PlayerPrefs.Save();

            OnSignInSuccess?.Invoke(playerProfile);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError("AuthenticationException: " + ex.Message);
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogError("RequestFailedException: " + ex.Message);
            Debug.LogException(ex);
        }
    }
    async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            var playerProfile = new PlayerProfile
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                PlayerName = name,
                Email = "",
                PhoneNumber = "",
                Level = 1,
                LastHeartTime = "0",
                DailyBonus = new DailyBonus()
                {
                    DateLastPlayed = "0",
                    DailyBonusDayKey = "0"
                },
                LevelsComplete = new List<LevelComplete>()
                {
                    
                }
            };

            PlayerPrefs.SetInt("IsAnonymous", 0);
            PlayerPrefs.Save();
            
            OnSignInSuccess?.Invoke(playerProfile);
            
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError("AuthenticationException: " + ex.Message);
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogError("RequestFailedException: " + ex.Message);
            Debug.LogException(ex);
        }
    }
    async Task SignInCachedUserAsync()
    {

        // Check if a cached player already exists by checking if the session token exists
        if (!AuthenticationService.Instance.SessionTokenExists) 
        {
            // if not, then do nothing
            LoadingPanel.SetActive(false);
            return;
        }

        // Sign in Anonymously
        // This call will sign in the cached player.
        try
        {
            LocalSaveManager.Instance.DeleteData("PlayerProfile");
            LocalSaveManager.Instance.DeleteImage("PlayerProfileImage");
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            await GetPlayerProfileAsync();

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");   
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }  
    }

    private async Task GetPlayerProfileAsync()
    {
        try
        {
            var playerProfile =  await cloudSaveManager.LoadDataAsync<PlayerProfile>("PlayerProfile");
            OnSignInSuccess?.Invoke(playerProfile);

            LoadingPanel.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= OnSignedIn;
        PlayerAccountService.Instance.SignedOut -= () => {Debug.Log("Signed out successfully.");};
    }
}
