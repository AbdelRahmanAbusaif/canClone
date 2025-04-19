using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using SaveData;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using TMPro;

public class LoginController : MonoBehaviour
{
    public TextMeshProUGUI textMessage;
    public event Action<PlayerProfile>  OnSignInSuccess;
    public static event Action OnSignedOutSuccess;
    public GameObject LoadingPanel;
    async private void Awake() 
    {    
        await UnityServices.Instance.InitializeAsync();

        PlayerAccountService.Instance.SignedIn += OnSignedIn;
        PlayerAccountService.Instance.SignedOut += () => {Debug.Log("Signed out successfully.");};
    }
    #region  Facebook
    public async void InitSignFacebook(FacebookGamesUser user)
    {
        await SignInFacebookAsync(user);
    }
    private async Task SignInFacebookAsync(FacebookGamesUser user)
    {
        try
        {
            LoadingPanel.SetActive(true);
            Debug.Log("Facebook token: " + user.idToken);
            await AuthenticationService.Instance.SignInWithFacebookAsync(user.idToken);
            var playerProfile = new PlayerProfile
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                PlayerName = user.name,
                Email = user.email,
                PlayerImageUrl = user.ImgUrl,
                PhoneNumber = "",
                DataPublicProfileImage = "",
                DataPublicProfileBorder = "",
                Level = 1,
                IsAcceptedTerms = false,
            };
            var heartSystem = new HeartSystem()
            {
                Heart = 5,
                LastHeartTime = "0",
                NextHeartTime = "0"
            };
            var dailyBonus = new DailyBonus()
            {
                DateLastPlayed = "0",
                DailyBonusDayKey = "0"
            };
            var spinWheel = new SpinWheel()
            {
                DateLastSpin = "0",
                DailySpinDayKey = "0"
            };

            var primeSubscriptions = new ConsumableItem()
            {
                Id = "PrimeSubscription",
                ConsumableName = "PrimeSubscription",
                DatePurchased = "0",
                DateExpired = "0"
            };
            var levelsComplete = new List<LevelComplete>();
            var adManager = new List<AdManager>();
            var containerProfileAvatarImages = new List<ConsumableItem>();
            var containerProfileBorders = new List<ConsumableItem>();
            var containerProfileCoverImages = new List<ConsumableItem>();

    
            OnSignInSuccess?.Invoke(playerProfile);

            Debug.Log("Sign in with Facebook succeeded!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            textMessage.text = "Authentication failed. Please try again. because: " + ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            LoadingPanel.SetActive(false);
        }
    }
    #endregion
    #region  Google Play Games
    
    public async void InitSignGooglePlay(GooglePlayGamesUser user)
    {
        await SignInGooglePlayAsync(user);
    }

    private async Task SignInGooglePlayAsync(GooglePlayGamesUser user)
    {
        try
        {
            LoadingPanel.SetActive(true);
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(user.idToken);
            var playerProfile = new PlayerProfile
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                PlayerName = user.name,
                Email = user.email,
                PlayerImageUrl = user.ImgUrl,
                PhoneNumber = "",
                DataPublicProfileImage = "",
                DataPublicProfileBorder = "",
                Level = 1,
                IsAcceptedTerms = false,
            };
            var heartSystem = new HeartSystem()
            {
                Heart = 5,
                LastHeartTime = "0",
                NextHeartTime = "0"
            };
            var dailyBonus = new DailyBonus()
            {
                DateLastPlayed = "0",
                DailyBonusDayKey = "0"
            };
            var spinWheel = new SpinWheel()
            {
                DateLastSpin = "0",
                DailySpinDayKey = "0"
            };
            
            var primeSubscriptions = new ConsumableItem()
            {
                Id = "PrimeSubscription",
                ConsumableName = "PrimeSubscription",
                DatePurchased = "0",
                DateExpired = "0"
            };
            var levelsComplete = new List<LevelComplete>();
            var adManager = new List<AdManager>();
            var containerProfileAvatarImages = new List<ConsumableItem>();
            var containerProfileBorders = new List<ConsumableItem>();
            var containerProfileCoverImages = new List<ConsumableItem>();

            OnSignInSuccess?.Invoke(playerProfile);
            
            Debug.Log("Sign in with Google Play Games succeeded!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            textMessage.text = "Authentication failed. Please try again. because: " + ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            LoadingPanel.SetActive(false);
        }
    }

    #endregion

    #region  Unity Player Account
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
            PlayerAccountService.Instance.SignOut();

            LocalSaveManager.Instance.DeleteData("PlayerProfile");
            LocalSaveManager.Instance.DeleteData("DailyBonus");
            LocalSaveManager.Instance.DeleteData("SpinWheel");
            LocalSaveManager.Instance.DeleteData("HeartSystem");
            LocalSaveManager.Instance.DeleteData("AdManager");
            LocalSaveManager.Instance.DeleteData("LevelsComplete");
            LocalSaveManager.Instance.DeleteData("PrimeSubscriptions");
            LocalSaveManager.Instance.DeleteData("ContainerProfileAvatarImages");
            LocalSaveManager.Instance.DeleteData("ContainerProfileCoverImages");
            LocalSaveManager.Instance.DeleteData("ContainerProfileBorders");

            LocalSaveManager.Instance.DeleteImage("PlayerProfileImage");

            OnSignedOutSuccess?.Invoke();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);            
        }
        
    }
    public async void InitSignAnonymous()
    {
        LocalSaveManager.Instance.DeleteData("PlayerProfile");
        LocalSaveManager.Instance.DeleteImage("PlayerProfileImage");

        await SignInAnonymously();
    }

    private async Task SignInAnonymously()
    {
        try
        {
            LoadingPanel.SetActive(true);
            
            Debug.Log("Signing in anonymously...");

            LocalSaveManager.Instance.DeleteData("PlayerProfile");
            LocalSaveManager.Instance.DeleteImage("PlayerProfileImage");

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
             var playerProfile = new PlayerProfile
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                PlayerName = AuthenticationService.Instance.PlayerName,
                Email = "",
                PlayerImageUrl = "",
                PhoneNumber = "",
                DataPublicProfileImage = "",
                DataPublicProfileBorder = "",
                Level = 1,
                IsAcceptedTerms = false,
            };
            var heartSystem = new HeartSystem()
            {
                Heart = 5,
                LastHeartTime = "0",
                NextHeartTime = "0"
            };
            var dailyBonus = new DailyBonus()
            {
                DateLastPlayed = "0",
                DailyBonusDayKey = "0"
            };
            var spinWheel = new SpinWheel()
            {
                DateLastSpin = "0",
                DailySpinDayKey = "0"
            };
            
            var primeSubscriptions = new List<PrimeSubscription>();
            var levelsComplete = new List<LevelComplete>();
            var adManager = new List<AdManager>();
            var containerProfileAvatarImages = new List<ConsumableItem>();
            var containerProfileBorders = new List<ConsumableItem>();
            var containerProfileCoverImages = new List<ConsumableItem>();
            var primeSubscription = new ConsumableItem
            {
                Id = "PrimeSubscription",
                ConsumableName = "PrimeSubscription",
                DatePurchased = "0",
                DateExpired = "0"
            };

            await CloudSaveManager.Instance.SaveDataAsync("DailyBonus", dailyBonus);
            await CloudSaveManager.Instance.SaveDataAsync("SpinWheel", spinWheel);
            await CloudSaveManager.Instance.SaveDataAsync("HeartSystem", heartSystem);
            await CloudSaveManager.Instance.SaveDataAsync("AdManager", adManager);
            await CloudSaveManager.Instance.SaveDataAsync("LevelsComplete", levelsComplete);
            await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileAvatarImages", containerProfileAvatarImages);
            await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileCoverImages", containerProfileCoverImages);
            await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileBorders", containerProfileBorders);
            await CloudSaveManager.Instance.SaveDataAsync("PrimeSubscriptions", primeSubscription);

            LoadingPanel.SetActive(false);
            OnSignInSuccess?.Invoke(playerProfile);
            Debug.Log("Sign in anonymously succeeded!");
        }
        catch (System.Exception)
        {
            Debug.Log("Sign in anonymously failed.");
            
            throw;
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
                PlayerName = AuthenticationService.Instance.PlayerName,
                Email = "",
                PlayerImageUrl = "user.ImgUrl",
                PhoneNumber = "",
                DataPublicProfileImage = "",
                DataPublicProfileBorder = "",
                Level = 1,
                IsAcceptedTerms = false,
            };
            var heartSystem = new HeartSystem()
            {
                Heart = 5,
                LastHeartTime = "0",
                NextHeartTime = "0"
            };
            var dailyBonus = new DailyBonus()
            {
                DateLastPlayed = "0",
                DailyBonusDayKey = "0"
            };
            var spinWheel = new SpinWheel()
            {
                DateLastSpin = "0",
                DailySpinDayKey = "0"
            };
            
            var primeSubscriptions = new List<PrimeSubscription>();
            var levelsComplete = new List<LevelComplete>();
            var adManager = new List<AdManager>();
            var containerProfileAvatarImages = new List<ConsumableItem>();
            var containerProfileBorders = new List<ConsumableItem>();
            var containerProfileCoverImages = new List<ConsumableItem>();


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
    public async void InitSignInCachedUser()
    {
        await SignInCachedUserAsync();
    }
    async Task SignInCachedUserAsync()
    {
        Debug.Log("Server time initialized.");
        // Check if a cached player already exists by checking if the session token exists
        if (!AuthenticationService.Instance.SessionTokenExists) 
        {
            // if not, then do nothing
            Debug.Log("No cached player found.");
            return;
        }

        // Sign in Anonymously
        // This call will sign in the cached player.
        try
        {
            LocalSaveManager.Instance.DeleteData("PlayerProfile");
            LocalSaveManager.Instance.DeleteData("DailyBonus");
            LocalSaveManager.Instance.DeleteData("SpinWheel");
            LocalSaveManager.Instance.DeleteData("HeartSystem");
            LocalSaveManager.Instance.DeleteData("AdManager");
            LocalSaveManager.Instance.DeleteData("LevelsComplete");
            LocalSaveManager.Instance.DeleteData("PrimeSubscriptions");
            LocalSaveManager.Instance.DeleteData("ContainerProfileAvatarImages");
            LocalSaveManager.Instance.DeleteData("ContainerProfileCoverImages");
            LocalSaveManager.Instance.DeleteData("ContainerProfileBorders");
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
    #endregion
    private async Task GetPlayerProfileAsync()
    {
        try
        {
            LoadingPanel.SetActive(true);
            var playerProfile =  await CloudSaveManager.Instance.LoadPublicDataAsync<PlayerProfile>("PlayerProfile");

            var dailyBonus = await CloudSaveManager.Instance.LoadDataAsync<DailyBonus>("DailyBonus");
            var containerPlayerImage = await CloudSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileAvatarImages");
            var containerPlayerCoverImage = await CloudSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileCoverImages");
            var containerPlayerBorderImage = await CloudSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileBorders");

            var primeSubscription = await CloudSaveManager.Instance.LoadDataAsync<ConsumableItem>("PrimeSubscriptions");
            var heartSystem = await CloudSaveManager.Instance.LoadDataAsync<HeartSystem>("HeartSystem");
            var spinWheel = await CloudSaveManager.Instance.LoadDataAsync<SpinWheel>("SpinWheel");
            var adManager = await CloudSaveManager.Instance.LoadDataAsync<List<AdManager>>("AdManager");
            var levelsComplete = await CloudSaveManager.Instance.LoadDataAsync<List<LevelComplete>>("LevelsComplete");

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
