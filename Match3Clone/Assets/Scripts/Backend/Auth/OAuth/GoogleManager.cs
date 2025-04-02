using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;

public class GoogleManager : MonoBehaviour
{
    public TextMeshProUGUI textMessage;
    public TextMeshProUGUI tokenMessage;
    public string Token;
    public string Error;

    private LoginController loginController;

    void Awake()
    {
        loginController = FindAnyObjectByType<LoginController>().GetComponent<LoginController>();
        PlayGamesPlatform.Activate();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

             // Request the server auth token
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
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
                        ImgUrl = ImgUrl
                    };

                    Debug.Log($"Google Play Games authentication successful. Name: {name}, ID: {id}, Auth Code: {code}");

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