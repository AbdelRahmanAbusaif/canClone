using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SaveData;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class RemotlyDownloadAssets : MonoBehaviour
{
    private string savePath;
    private async Task Awake()
    {
        savePath = Application.persistentDataPath + "/DownloadedAssets/";
        Directory.CreateDirectory(savePath);

        await UnityServices.Instance.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
    }

    private async void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully!");

        List<GameAssetsFiles> gameAssetsFiles = new List<GameAssetsFiles>
        {
            new() {
                FileName = "logo.png",
                FileURL = RemoteConfigService.Instance.appConfig.GetString("LogoURL"),
                LocalURL = savePath
            },
            new() {
                FileName = "background.png",
                FileURL = RemoteConfigService.Instance.appConfig.GetString("BackgroundURL"),
                LocalURL = savePath
            }
        };

        await DownloadAndSaveFiles(gameAssetsFiles);
    }

    private async Task DownloadAndSaveFiles(List<GameAssetsFiles> gameAssetsFiles)
    {
        // Load the existing list of files from storage
        List<GameAssetsFiles> savedFiles = await LocalSaveManager.Instance.LoadDataAsync<List<GameAssetsFiles>>("GameAssetsFiles");
        if (savedFiles == null)
        {
            savedFiles = new List<GameAssetsFiles>();
        }

        foreach(var file in savedFiles)
        {
            Debug.Log($"Saved File: {file.FileName}");
        }
        foreach (var file in gameAssetsFiles)
        {
            GameAssetsFiles gameAssetsLoad = savedFiles.Find(f => f.FileName == file.FileName);

            if(gameAssetsLoad != null)
            {
                Debug.Log($"{file.FileName} exists locally. Checking for updates...");

                if(gameAssetsLoad.FileURL == file.FileURL)
                {
                    Debug.Log($"{file.FileName} is up to date. Skipping download.");
                    continue;
                }
                else
                {
                    Debug.Log($"{file.FileName} has been updated. Downloading new version...");
                }
            }

            Debug.Log($"Downloading {file.FileName} from {file.FileURL}");

            using UnityWebRequest request = UnityWebRequest.Get(file.FileURL);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                await File.WriteAllBytesAsync(file.LocalURL + file.FileName, request.downloadHandler.data);

                if(gameAssetsLoad != null)
                {
                    savedFiles.Remove(gameAssetsLoad);
                }
                savedFiles.Add(file);

                Debug.Log($"Downloaded and saved: {file.LocalURL + file.FileName}");

            }
            else
            {
                Debug.LogError($"Failed to download {file.FileName}: {request.error}");
            }
        }

        foreach (var file in savedFiles)
        {
            Debug.Log($"Saved File: {file.FileName}");
        }

        await LocalSaveManager.Instance.SaveDataAsync<List<GameAssetsFiles>>(savedFiles, "GameAssetsFiles");
        Debug.Log("All assets have been updated and saved.");
    }
    
    private void OnDestroy() {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
    }
    public struct UserAttributes { }
    public struct AppAttributes { }
}
