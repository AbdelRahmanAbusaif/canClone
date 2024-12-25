using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private string _facebookToken = "EAAC";
    private async void Awake() {
        await UnityServices.Instance.InitializeAsync();
    }
    public async void SignAsGuest()
    {
        await SingAsGust();
    }
    async Task SingAsGust()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            print($"Sign As Gust with Player ID {AuthenticationService.Instance.PlayerId}");
            print($"Sign As Gust with Player Name {AuthenticationService.Instance.PlayerName}");
            print($"Sign As Gust with Player Info {AuthenticationService.Instance.PlayerInfo}");
        }
        catch (AuthenticationException e)
        {
            print("Here is Error With : " + e.Message);
            throw;
        }
    }

    public async void SignWithFacebook()
    {
        await SignWithFacebookTask(_facebookToken);
    }

    async Task SignWithFacebookTask(string token)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithFacebookAsync(token);
            
            print($"Sign As Gust with Player ID {AuthenticationService.Instance.PlayerId}");
            print($"Sign As Gust with Player Name {AuthenticationService.Instance.PlayerName}");
            print($"Sign As Gust with Player Info {AuthenticationService.Instance.PlayerInfo}");
        }
        catch (AuthenticationException e)
        {
            print("Here is Error With : " + e.Message);
            throw;
        }
    }
    public async void SignWithGoogle()
    {
        await SignWithGoogleTask(_facebookToken);
    }
    async Task SignWithGoogleTask(string token)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGoogleAsync("");
            
            print($"Sign As Gust with Player ID {AuthenticationService.Instance.PlayerId}");
            print($"Sign As Gust with Player Name {AuthenticationService.Instance.PlayerName}");
            print($"Sign As Gust with Player Info {AuthenticationService.Instance.PlayerInfo}");
        }
        catch (AuthenticationException e)
        {
            print("Here is Error With : " + e.Message);
            throw;
        }
    }

}
