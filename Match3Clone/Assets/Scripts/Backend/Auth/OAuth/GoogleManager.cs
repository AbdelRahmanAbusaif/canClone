using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using Unity.Services.Core;
using UnityEngine;

public class GoogleManager : MonoBehaviour
{
    public TextMeshProUGUI textMessage;
    public string Token;
    public string Error;

    private LoginController loginController;

    void Awake()
    {
        loginController = FindAnyObjectByType<LoginController>().GetComponent<LoginController>();
        StartClientService();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    async void StartClientService()
    {
         try
         {
             if (UnityServices.State != ServicesInitializationState.Initialized)
             {
                 var options = new InitializationOptions();
                 await UnityServices.InitializeAsync();
                 PlayGamesPlatform.Activate();
             }
         }
         catch (Exception e)
         {
             Debug.LogException(e);
         }
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
            string email = PlayGamesPlatform.Instance.localUser.userName;
             // Request the server auth token
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, code =>
                {
                    if (string.IsNullOrEmpty(code))
                    {
                        Debug.LogError("Failed to retrieve Auth Code from Google Play Games.");
                        textMessage.text = "Failed to retrieve Auth Code from Google Play Games.";
                        return;
                    }

                    GooglePlayGamesUser user = new GooglePlayGamesUser
                    {
                        idToken = code,  // ✅ Now using the correct Auth Code
                        id = id,
                        name = name,
                        ImgUrl = ImgUrl,
                    };

                    Debug.Log($"Google Play Games authentication successful. Name: {name}, ID: {id}, Auth Code: {code}");
                    textMessage.text = $"Google Play Games authentication successful. Name: {name}, ID: {id}, Auth Code: {code}";
                    loginController.InitSignGooglePlay(user);
                }
            );
        }
        else
        {
            Debug.LogError("Google Play Games authentication failed: " + status);
            textMessage.text = "Google Play Games authentication failed: " + status;
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
}

[System.Serializable]
public class GooglePlayGamesUser
{
    public string idToken;
    public string id;
    public string name;
    public string email;
    public string ImgUrl;
}