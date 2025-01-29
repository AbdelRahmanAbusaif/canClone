using System.Collections;
using SaveData;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [Header("Required Components")]
    public RemotlyDownloadAssets remoteAssetDownloader; // Attach the RemoteAssetDownloader script
    public LoginController loginController; // Attach the LoginController script
    [Header("UI Elements")]
    public GameObject loadingSpinner; 
    public string HomePageScene; 
    public string LoginPageScene;

    void Start()
    {
        StartCoroutine(LoadGameData());
    }

    IEnumerator LoadGameData()
    {
        // Show loading spinner if available
        if (loadingSpinner != null)
        {
            loadingSpinner.SetActive(true);
        }

        // Task 1: Simulate downloading assets from the cloud
        Debug.Log("Downloading assets...");
        yield return ServerTimeManager.Instance.FetchServerTimeAsync();

        // Task 3: Simulate fetching API data
        Debug.Log("Fetching API data...");
        yield return remoteAssetDownloader.DownloadAndSaveFiles(remoteAssetDownloader.GameAssetsFiles);

        // Hide loading spinner
        if (loadingSpinner != null)
        {
            loadingSpinner.SetActive(false);
        }

        // Load the next scene
        Debug.Log("Loading next scene...");

        if(AuthenticationService.Instance.SessionTokenExists)
        {
            loginController.InitSignInCachedUser();
            // SceneManager.LoadSceneAsync(HomePageScene);
        }
        else
        {
            SceneManager.LoadScene(LoginPageScene);
        }
    }
}
