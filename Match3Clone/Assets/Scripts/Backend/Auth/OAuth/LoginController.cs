using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using SaveData;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using TMPro;
using System.Security.Cryptography;
using System.Text;

public class LoginController : MonoBehaviour
{
    public event Action<PlayerProfile>  OnSignInSuccess;
    public event Action OnLinkedAccount;
    public event Action OnSignedOutSuccess;
    public GameObject LoadingPanel;
    
    public event Action OnLinkedAccountIsAlreadyExists;

	async private void Awake() 
    {    
        await UnityServices.Instance.InitializeAsync();

        PlayerAccountService.Instance.SignedIn += OnSignedIn;
        PlayerAccountService.Instance.SignedOut += () => {Debug.Log("Signed out successfully.");};
    }
    #region Username and Password
    public async void InitSignUpWithUsernameAndPassword(string email, string password)
    {
        await SignUpWithUsernameAndPasswordAsync(email, password);
    }

    private async Task SignUpWithUsernameAndPasswordAsync(string email, string password)
    {
        string baseName = email.Split('@')[0];

        // Get 4-character hash suffix
        string hashSuffix = GetShortHash(email, 4);

        // Trim baseName if needed
        int maxBaseLength = 20 - hashSuffix.Length;
        if (baseName.Length > maxBaseLength)
            baseName = baseName.Substring(0, maxBaseLength);

        string username = baseName + hashSuffix;

        Debug.Log("Generated username: " + username);

        try
        {
            LoadingPanel.SetActive(true);
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, username + "*9jJ");
            var playerProfile = new PlayerProfile
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                PlayerName = AuthenticationService.Instance.PlayerName,
                Email = email,
                PlayerImageUrl = "",
                PhoneNumber = "",
                DataPublicProfileImage = "",
                DataPublicProfileBorder = "",
                Level = 1,
                IsAcceptedTerms = false,
            };

            PlayerPrefs.SetInt("IsLinkAccount", 1);
            PlayerPrefs.Save();

            OnSignInSuccess?.Invoke(playerProfile);
            Debug.Log("Sign in with username and password succeeded!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
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

    public async void InitSignInWithUsernameAndPassword(string email, string password)
    {
        await SignInWithUsernameAndPasswordAsync(email, password);
    }
    private async Task SignInWithUsernameAndPasswordAsync(string email, string password)
    {
        string baseName = email.Split('@')[0];

        // Get 4-character hash suffix
        string hashSuffix = GetShortHash(email, 4);

        // Trim baseName if needed
        int maxBaseLength = 20 - hashSuffix.Length;
        if (baseName.Length > maxBaseLength)
            baseName = baseName.Substring(0, maxBaseLength);

        string username = baseName + hashSuffix;

        Debug.Log("Username: " + username);

        try
        {
            LoadingPanel.SetActive(true);
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, username + "*9jJ");
            var playerProfile = new PlayerProfile
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                PlayerName = AuthenticationService.Instance.PlayerName,
                Email = email,
                PlayerImageUrl = "",
                PhoneNumber = "",
                DataPublicProfileImage = "",
                DataPublicProfileBorder = "",
                Level = 1,
                IsAcceptedTerms = false,
            };

            PlayerPrefs.SetInt("IsLinkAccount", 1);
            PlayerPrefs.Save();

            OnSignInSuccess?.Invoke(playerProfile);
            Debug.Log("Sign in with username and password succeeded!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
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

    public async void InitLinkAccountWithUsernamePassword(string email, string password)
    {
        await LinkAccountWithUsernamePasswordAsync(email, password);
    }

    private async Task LinkAccountWithUsernamePasswordAsync(string email, string password)
    {
        LoadingPanel.SetActive(true);
        try
        {
            string baseName = email.Split('@')[0];

            // Get 4-character hash suffix
            string hashSuffix = GetShortHash(email, 4);

            // Trim baseName if needed
            int maxBaseLength = 20 - hashSuffix.Length;
            if (baseName.Length > maxBaseLength)
                baseName = baseName.Substring(0, maxBaseLength);

            string username = baseName + hashSuffix;

            Debug.Log("Username: " + username);

            await AuthenticationService.Instance.AddUsernamePasswordAsync(username, username + "*9jJ");

            var playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

            playerProfile.Email = email;
            playerProfile.PlayerName = username;

            await CloudSaveManager.Instance.SaveDataAsync("PlayerProfile", playerProfile);

            PlayerPrefs.SetInt("IsLinkAccount", 1);
            PlayerPrefs.Save();

            OnLinkedAccount?.Invoke();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);

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
            
            PlayerPrefs.SetInt("IsLinkAccount", 1);
            PlayerPrefs.Save();
            
            OnSignInSuccess?.Invoke(playerProfile);
            
            Debug.Log("Sign in with Facebook succeeded!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
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
    public async void InitLinkAccountWithFacebook(FacebookGamesUser user)
    {
        try
        {
            LoadingPanel.SetActive(true);
            Debug.Log("Facebook token: " + user.idToken);
            await AuthenticationService.Instance.LinkWithFacebookAsync(user.idToken);
            var playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

            playerProfile.Email = user.email;
            playerProfile.PlayerName = user.name;
            
            playerProfile.PlayerImageUrl = user.ImgUrl;
            
            // Save the updated player profile
            await CloudSaveManager.Instance.SavePublicDataAsync("PlayerProfile", playerProfile);
            
            // Update Player Image from URL
            StartCoroutine(UpdatePlayerImage(playerProfile.PlayerImageUrl));

            PlayerPrefs.SetInt("IsLinkAccount", 1);
            PlayerPrefs.Save();

            OnLinkedAccount?.Invoke();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);

            Debug.LogError("Request failed: " + ex.Message);
			// Handle specific error: Facebook already linked to another account
            Debug.LogError("Error code: " + ex.ErrorCode);
            Debug.LogError("Authentication Code Error: " + AuthenticationErrorCodes.AccountAlreadyLinked);
			// Check if the error code indicates that the Facebook account is already linked to another Unity account


			if (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
			{
				Debug.LogWarning("This Facebook account is already linked to another Unity account.");

				// Optionally show a UI message to the user:
				//ShowPopup("This Facebook account is already linked to another account.");
				OnLinkedAccountIsAlreadyExists?.Invoke();

				// Optional: Sign in directly with Facebook instead
				// await AuthenticationService.Instance.SignInWithFacebookAsync(user.idToken);
			}
            else
            {
				Debug.LogError("An error occurred while linking the Facebook account: " + ex.Message);
			}
		}
		finally
        {
            LoadingPanel.SetActive(false);
        }
    }

    private IEnumerator UpdatePlayerImage(string playerProfilePlayerImageUrl)
    {
        // Download the image from the URL
        var unityWebRequest = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(playerProfilePlayerImageUrl);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || 
            unityWebRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading image: " + unityWebRequest.error);
        }
        else
        {
            // Get the texture from the response
            var texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(unityWebRequest);
            // Save the texture to local storage
            CallAsyncMethod(texture);
            Debug.Log("Player profile image updated successfully.");
        }
    }

    private async void CallAsyncMethod(Texture2D texture) => await CloudSaveManager.Instance.SaveImageAsync("PlayerProfileImage", texture);
    
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

            PlayerPrefs.DeleteAll();

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
                PlayerName = await AuthenticationService.Instance.GetPlayerNameAsync(),
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

            PlayerPrefs.SetInt("IsLinkAccount", 0);
            PlayerPrefs.Save();

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
            LocalSaveManager.Instance.DeleteImage("FriendList");
			LocalSaveManager.Instance.DeleteImage("FriendRequests");

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
    private string GetShortHash(string input, int length)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length && sb.Length < length; i++)
            {
                sb.Append(bytes[i].ToString("x2")); // hex format
            }

            return sb.ToString().Substring(0, length);
        }
    }
}
