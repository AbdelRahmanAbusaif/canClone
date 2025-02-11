using System.Collections;
using System.Threading.Tasks;
using GameVanilla.Core;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Economy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("Required Components")]
    public RemotlyDownloadAssets remoteAssetDownloader; // Attach the RemoteAssetDownloader script
    public LoginController loginController; // Attach the LoginController script
    [Header("UI Elements")]
    public GameObject LoadingSpinner; 
    public GameObject RetryPanel;

    public Button RetryButton;

    public string HomePageScene; 
    public string LoginPageScene;

    [Header("Booleans for checking")]
    public bool isAssetsDownloaded = false;
    public bool isAPIDataFetched = false;

    async void Start()
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

        await UnityServices.Instance.InitializeAsync();
        await EconomyService.Instance.Configuration.SyncConfigurationAsync();
        
        RetryButton.onClick.AddListener(() => {
            RetryPanel.SetActive(false);
            StartCoroutine(LoadGameData());
        });

        StartCoroutine(LoadGameData());
    }

    IEnumerator LoadGameData()
    {
        // Show loading spinner if available
        if (LoadingSpinner != null)
        {
            LoadingSpinner.SetActive(true);
        }

        // Task 1: Simulate downloading assets from the cloud
        Debug.Log("Downloading assets...");
        yield return ServerTimeManager.Instance.FetchServerTimeAsync();

        // Task 3: Simulate fetching API data
        Debug.Log("Fetching API data...");
        yield return remoteAssetDownloader.DownloadAndSaveFiles(remoteAssetDownloader.GameAssetsFiles);

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
            Transition.LoadLevel(LoginPageScene,1f,Color.black);
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
    }
}
