using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public event Action<PlayerInfo, string> OnSignInSuccess;
    private PlayerInfo playerInfo;
    async private void Awake() {
        await UnityServices.Instance.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += OnSignedIn;
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
    async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            playerInfo =  AuthenticationService.Instance.PlayerInfo;
            OnSignInSuccess?.Invoke(playerInfo, accessToken);
            
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
    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= OnSignedIn;
    }
}
