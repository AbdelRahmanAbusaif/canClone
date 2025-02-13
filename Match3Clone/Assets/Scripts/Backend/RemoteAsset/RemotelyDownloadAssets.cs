using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SaveData;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.Networking;

public class RemotelyDownloadAssets : MonoBehaviour
{
    private string savePath;
    public Action<bool> OnDownloadCompleted;
    public List<GameAssetsFiles> GameAssetsFiles = new List<GameAssetsFiles>();

    private async void Awake()
    {
        savePath = Application.persistentDataPath + "/DownloadedAssets/";
        Directory.CreateDirectory(savePath);

        await UnityServices.Instance.InitializeAsync();

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
    }
    private void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully!");

       var assetKeys = new Dictionary<string, string>
        {
            { "logo.png", "LogoURL" },
            { "background.png", "BackgroundURL" },
            { "BackGroundMusic.wav", "BackgroundMusicURL" },
            { "PurpleCandy.png", "PurpleCandyURL" },
            { "RedCandy.png", "RedCandyURL" },
            { "GreenCandy.png", "GreenCandyURL" },
            { "YellowCandy.png", "YellowCandyURL" },
            { "OrangeCandy.png", "OrangeCandyURL" },
            { "BlueCandy.png", "BlueCandyURL" },
            { "WrappedYellowCandy.png", "WrappedYellowCandyURL" },
            { "WrappedRedCandy.png", "WrappedRedCandyURL" },
            { "WrappedPurpleCandy.png", "WrappedPurpleCandyURL" },
            { "WrappedOrangeCandy.png", "WrappedOrangeCandyURL" },
            { "WrappedGreenCandy.png", "WrappedGreenCandyURL" },
            { "WrappedBlueCandy.png", "WrappedBlueCandyURL" },
            { "StripedVerticalYellowCandy.png", "StripedVerticalYellowCandyURL" },
            { "StripedVerticalRedCandy.png", "StripedVerticalRedCandyURL" },
            { "StripedVerticalPurpleCandy.png", "StripedVerticalPurpleCandyURL" },
            { "StripedVerticalOrangeCandy.png", "StripedVerticalOrangeCandyURL" },
            { "StripedVerticalGreenCandy.png", "StripedVerticalGreenCandyURL" },
            { "StripedVerticalBlueCandy.png", "StripedVerticalBlueCandyURL" },
            { "StripedHorizontalYellowCandy.png", "StripedHorizontalYellowCandyURL" },
            { "StripedHorizontalRedCandy.png", "StripedHorizontalRedCandyURL" },
            { "StripedHorizontalPurpleCandy.png", "StripedHorizontalPurpleCandyURL" },
            { "StripedHorizontalOrangeCandy.png", "StripedHorizontalOrangeCandyURL" },
            { "StripedHorizontalGreenCandy.png", "StripedHorizontalGreenCandyURL" },
            { "StripedHorizontalBlueCandy.png", "StripedHorizontalBlueCandyURL" },
            { "ColorBomb.png", "ColorBombURL" }
        };

        List<GameAssetsFiles> gameAssetsFiles = assetKeys.Select(kvp => new GameAssetsFiles
        {
            FileName = kvp.Key,
            FileURL = RemoteConfigService.Instance.appConfig.GetString(kvp.Value),
            LocalURL = savePath
        }).ToList();

        GameAssetsFiles = gameAssetsFiles;
    }

    public IEnumerator DownloadAndSaveFiles(List<GameAssetsFiles> gameAssetsFiles)
    {
        // Load the existing list of files from storage
        var loadTask = LocalSaveManager.Instance.LoadDataAsync<List<GameAssetsFiles>>("GameAssetsFiles");
        yield return new WaitUntil(() => loadTask.IsCompleted);
        List<GameAssetsFiles> savedFiles = loadTask.Result;
        if (savedFiles == null)
        {
            savedFiles = new List<GameAssetsFiles>();
        }

        foreach (var file in gameAssetsFiles)
        {
            GameAssetsFiles gameAssetsLoad = savedFiles.Find(f => f.FileName == file.FileName);

            if (gameAssetsLoad != null && File.Exists(file.LocalURL + file.FileName))
            {
                Debug.Log($"{file.FileName} exists locally. Checking for updates...");

                if (gameAssetsLoad.FileURL == file.FileURL)
                {
                    using UnityWebRequest requestImage = UnityWebRequest.Head(file.FileURL);
                    yield return requestImage.SendWebRequest();

                    if (requestImage.result == UnityWebRequest.Result.Success)
                    {
                        string remoteFileSize = requestImage.GetResponseHeader("Content-Length");
                        string localFileSize = new FileInfo(file.LocalURL + file.FileName).Length.ToString();

                        OnDownloadCompleted?.Invoke(true);
                        
                        if (remoteFileSize == localFileSize)
                        {
                            Debug.Log($"{file.FileName} is up to date.");

                            continue;
                        }
                        else
                        {
                            Debug.Log($"{file.FileName} has been updated. Downloading new version...");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to check {file.FileName} for updates: {requestImage.error}");

                        OnDownloadCompleted?.Invoke(false);
                    }
                }
                else
                {
                    Debug.Log($"{file.FileName} has been updated. Downloading new version...");
                }
            }

            Debug.Log($"Downloading {file.FileName} from {file.FileURL}");

            using UnityWebRequest request = UnityWebRequest.Get(file.FileURL);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(file.LocalURL + file.FileName, request.downloadHandler.data);

                if (gameAssetsLoad != null)
                {
                    savedFiles.Remove(gameAssetsLoad);
                }
                savedFiles.Add(file);

                Debug.Log($"Downloaded and saved: {file.LocalURL + file.FileName}");

                OnDownloadCompleted?.Invoke(true);
            }
            else
            {
                OnDownloadCompleted?.Invoke(false);

                Debug.LogError($"Failed to download {file.FileName}: {request.error}");
            }
        }
        var saveTask = LocalSaveManager.Instance.SaveDataAsync<List<GameAssetsFiles>>(savedFiles, "GameAssetsFiles");
        yield return new WaitUntil(() => saveTask.IsCompleted);

        Debug.Log("All assets have been updated and saved.");
    }
    private void OnDestroy() {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
    }
    public struct UserAttributes { }
    public struct AppAttributes { }
}
