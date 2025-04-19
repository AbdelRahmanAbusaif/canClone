using System.Collections;
using System.Threading.Tasks;
using GameVanilla.Core;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("Required Components")]
    public RemotelyDownloadAssets remoteAssetDownloader; // Attach the RemoteAssetDownloader script
    public LoginController loginController; // Attach the LoginController script
    [Header("UI Elements")]
    public GameObject LoadingSpinner; 
    public GameObject LoadingBar;
    public GameObject RetryPanel;
    
    [Header("UI Elements for Progress Bar")]
    public Slider slider;
    public TextMeshProUGUI ProgressText;

    public Button RetryButton;
    public UILogin uILogin;

    [Header("Scene Names")]
    public string HomePageScene; 
    public string LoginPageScene;

    [Header("Checkers")]
    public string App_Updated_Number;

    public bool isAssetsDownloaded = false;
    public bool isAPIDataFetched = false;
    

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        PlayerPrefs.SetInt("IsFirstTime", 1);
        PlayerPrefs.SetInt("IsFirstTimeVideoAd", 1);
        PlayerPrefs.Save();

        
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

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        StartCoroutine(LoadGameData());
    }
    public void ApplyRemoteConfig(ConfigResponse response)
    {
        if(response.status == ConfigRequestStatus.Success)
        {
            Debug.Log("Remote Config Fetched Successfully!");
            App_Updated_Number = RemoteConfigService.Instance.appConfig.GetString("app_updated_number");

            Debug.Log("App Updated Number: " + App_Updated_Number);
        }
        else
        {
            Debug.Log("Remote Config Fetch Failed!");
        }
    }
    private IEnumerator LoadGameData()
    {
        // Show loading spinner if available
        if (LoadingSpinner != null)
        {
            LoadingSpinner.SetActive(true);
        }

        yield return new WaitForSeconds(2);
        Debug.Log("Downloading assets...");
        yield return ServerTimeManager.Instance.FetchServerTimeAsync();

        Debug.Log("Fetching API data...");

        if(string.Equals(App_Updated_Number,PlayerPrefs.GetString("App_Updated_Number","0")))
        {
            Debug.Log("App is already updated");
            isAssetsDownloaded = true;
        }
        else
        {
            Debug.Log("App is not updated");
            
            LoadingBar.SetActive(true);
            LoadingSpinner.SetActive(false);

            yield return remoteAssetDownloader.AsyncOperationDownloadWithProgress(remoteAssetDownloader.GameAssetsFiles, slider, ProgressText);

            PlayerPrefs.SetString("App_Updated_Number",App_Updated_Number);
            PlayerPrefs.Save();

        }


        if (isAssetsDownloaded && isAPIDataFetched)
        {
            LoadNextScene();
        }
        else
        {
            LoadingBar.SetActive(false);
            LoadingSpinner.SetActive(false);
            RetryPanel.SetActive(true);
        }
    }

    private void LoadNextScene()
    {
        Debug.Log("Loading next scene...");

        if (AuthenticationService.Instance.SessionTokenExists)
        {
            loginController.InitSignInCachedUser();

            // Hide loading spinner
            if (LoadingSpinner != null)
            {
                LoadingSpinner.SetActive(false);
            }
            // SceneManager.LoadSceneAsync(HomePageScene);
        }
        else
        {
            // Transition.LoadLevel(LoginPageScene,1f,Color.black);
            loginController.InitSignAnonymous();
        }
    }

    private void OnDestroy() 
    {
        RetryButton.onClick.RemoveAllListeners();
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
