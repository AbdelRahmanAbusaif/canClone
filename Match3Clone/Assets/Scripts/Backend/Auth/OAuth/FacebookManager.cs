using System.Collections.Generic;
using Facebook.Unity;
using TMPro;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    private LoginController _loginController;
    public string Token;
    public string Error;

    // Awake function from Unity's MonoBehaviour
    void Awake()
    {
        _loginController = FindAnyObjectByType<LoginController>().GetComponent<LoginController>();
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Login()
    {
        // Define the permissions
        var perms = new List<string>() { "public_profile", "email" };

        FB.LogInWithReadPermissions(perms, result =>
        {
            if (FB.IsLoggedIn)
            {
                Token = AccessToken.CurrentAccessToken.TokenString;
                Debug.Log($"Facebook Login token: {Token}");

                string name = AccessToken.CurrentAccessToken.UserId;
                string ImgUrl = "https://graph.facebook.com/" + name + "/picture?type=large";
                
                FB.API("/me?fields=name,email", HttpMethod.GET, userResult =>
                {
                    if (userResult.Error == null)
                    {
                        var userInfo = Facebook.MiniJSON.Json.Deserialize(userResult.RawResult) as Dictionary<string, object>;
                        string email = userInfo["email"] as string;
                        string name = userInfo["name"] as string;

                        FacebookGamesUser fbUser = new FacebookGamesUser
                        {
                            idToken = Token,
                            name = name,
                            email = email,
                            ImgUrl = ImgUrl
                        };

                        Debug.Log($"Facebook User Info: Name: {name}, Email: {email}");
                        _loginController.InitSignFacebook(fbUser);
                    }
                    else
                    {
                        Error = userResult.Error;
                        Debug.LogError($"Error fetching user info: {Error}");
                    }
                });
            }
            else
            {
                Error = "User cancelled login";
                Debug.Log("[Facebook Login] User cancelled login");
            }
        });
    }
}
[System.Serializable]
public class FacebookGamesUser
{
    public string idToken;
    public string name;
    public string email;
    public string ImgUrl;
}
