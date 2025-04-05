using System.Collections.Generic;
using Facebook.Unity;
using TMPro;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    private LoginController _loginController;
    public string Token;
    public string Error;
    public TextMeshProUGUI URLImageText;

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
                
                FB.API($"/me?fields=name,email,picture.type(large)&access_token={Token}", HttpMethod.GET, userResult =>
                {
                    if (userResult.Error == null)
                    {
                        var userInfo = Facebook.MiniJSON.Json.Deserialize(userResult.RawResult) as Dictionary<string, object>;
                        string email = userInfo["email"] as string;
                        string name = userInfo["name"] as string;

                        // Parse the nested picture dictionary
                        var pictureDict = userInfo["picture"] as Dictionary<string, object>;
                        var dataDict = pictureDict["data"] as Dictionary<string, object>;
                        string pictureUrl = dataDict["url"] as string;

                        Debug.Log($"Facebook User Info: Name: {name}, Email: {email}, Picture URL: {pictureUrl}");
                        FacebookGamesUser fbUser = new()
                        {
                            idToken = Token,
                            name = name.Replace(" ", ""),
                            email = email,
                            ImgUrl = pictureUrl
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
[System.Serializable]
public class FacebookPictureData
{
    public int height;
    public int width;
    public bool is_silhouette;
    public string url;
}

[System.Serializable]
public class FacebookPicture
{
    public FacebookPictureData data;
}
