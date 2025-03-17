using System.Collections;
using GameVanilla.Core;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("Required Components")]
    public RemotelyDownloadAssets remoteAssetDownloader; // Attach the RemoteAssetDownloader script
    public LoginController loginController; // Attach the LoginController script
    [Header("UI Elements")]
    public GameObject LoadingSpinner; 
    public GameObject RetryPanel;
    
    public Slider slider;
    public TextMeshProUGUI ProgressText;

    public Button RetryButton;
    public UILogin uILogin;

    public string HomePageScene; 
    public string LoginPageScene;

    [Header("Booleans for checking")]
    public bool isAssetsDownloaded = false;
    public bool isAPIDataFetched = false;

    void Start()
    {
        remoteAssetDownloader.OnDownloadCompleted += (bool isAssetsDownloaded) => {
            if(isAssetsDownloaded)
            {
                this.isAssetsDownloaded = true;
            }
            else
            {
                this.isAssetsDownloaded = false;
            }
        };
        ServerTimeManager.Instance.OnServerInitialized += (bool isAPIDataFetched)=>{
            if(isAPIDataFetched)
            {
                this.isAPIDataFetched = true;
            }
            else
            {
                this.isAPIDataFetched = false;
            }
        };

        RetryButton.onClick.AddListener(() => {
            RetryPanel.SetActive(false);
            StartCoroutine(LoadGameData());
        });

        // Add event listener for sign up button
        // this will load the login page scene because player is already signed in but not complete the sign up process
        uILogin.OnSignUp += () => {
            // AuthenticationService.Instance.SignOut();
            // PlayerAccountService.Instance.SignOut();
            Transition.LoadLevel(LoginPageScene,1f,Color.black);
        };
        StartCoroutine(LoadGameData());
    }

    IEnumerator LoadGameData()
    {
        // Show loading spinner if available
        if (LoadingSpinner != null)
        {
            LoadingSpinner.SetActive(true);
        }

        Debug.Log("Downloading assets...");
        yield return ServerTimeManager.Instance.FetchServerTimeAsync();

        Debug.Log("Fetching API data...");

        yield return remoteAssetDownloader.AsyncOperationDownloadWithProgress(remoteAssetDownloader.GameAssetsFiles, slider, ProgressText);

        // Hide loading spinner
        if (LoadingSpinner != null)
        {
            LoadingSpinner.SetActive(false);
        }

        if (isAssetsDownloaded && isAPIDataFetched)
        {
            LoadNextScene();
        }
        else
        {
            RetryPanel.SetActive(true);
        }
    }

    private void LoadNextScene()
    {
        Debug.Log("Loading next scene...");

        if (AuthenticationService.Instance.SessionTokenExists)
        {
            loginController.InitSignInCachedUser();
            // SceneManager.LoadSceneAsync(HomePageScene);
        }
        else
        {
            // Transition.LoadLevel(LoginPageScene,1f,Color.black);
            loginController.InitSignAnonymous();
        }
    }

    private void OnDestroy() {

        remoteAssetDownloader.OnDownloadCompleted -= (bool isAssetsDownloaded) => {
            if(isAssetsDownloaded)
            {
                this.isAssetsDownloaded = true;
            }
            else
            {
                this.isAssetsDownloaded = false;
            }
        };
        ServerTimeManager.Instance.OnServerInitialized -= (bool isAPIDataFetched)=>{
            if(isAPIDataFetched)
            {
                this.isAPIDataFetched = true;
            }
            else
            {
                this.isAPIDataFetched = false;
            }
        };

        uILogin.OnSignUp -= () => {
            Transition.LoadLevel(LoginPageScene,1f,Color.black);
        };
    }
}
