using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SaveData;
using TMPro;
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
        savePath = Path.Combine(Application.persistentDataPath, "DownloadedAssets");
        Directory.CreateDirectory(savePath);

        await UnityServices.Instance.InitializeAsync();
    }
    public void ApplyRemoteConfig(ConfigResponse response)
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
            { "ColorBomb.png", "ColorBombURL" },
            { "Syrup1.png", "Syrup1URL" },
            { "Syrup2.png", "Syrup2URL" },
            { "Unbreakable.png", "UnbreakableURL" },
            { "Watermelon.png", "WatermelonURL" },
            { "Cherry.png", "CherryURL" },
            { "Chocolate.png", "ChocolateURL" },
            { "Honey.png", "HoneyURL" },
            { "Ice.png", "IceURL" },
            { "Marshmallow.png", "MarshmallowURL" },
            { "SubIcon.png", "SubIconURL"}
        };

        List<GameAssetsFiles> gameAssetsFiles = assetKeys.Select(kvp => new GameAssetsFiles
        {
            FileName = kvp.Key,
            FileURL = RemoteConfigService.Instance.appConfig.GetString(kvp.Value),
            LocalURL = savePath
        }).ToList();

        GameAssetsFiles = gameAssetsFiles;
    }

    /// <summary>
    /// Downloads and saves a list of game asset files to the local storage.
    /// </summary>
    /// <param name="gameAssetsFiles">A list of <see cref="GameAssetsFiles"/> objects representing the files to be downloaded.</param>
    /// <returns>
    /// An <see cref="AsyncOperation"/> that represents the asynchronous operation of downloading and saving files.
    /// </returns>
    /// <remarks>
    /// This method checks if the files already exist locally and are up-to-date by comparing their sizes.
    /// If a file is outdated or missing, it downloads the latest version from the specified URL and saves it locally.
    /// The method also updates the saved metadata for the files.
    /// </remarks>
    /// 

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

            string localPath = Path.Combine(file.LocalURL, file.FileName);
            if (gameAssetsLoad != null && File.Exists(localPath))
            {
                Debug.Log($"{file.FileName} exists locally. Checking for updates...");

                if (gameAssetsLoad.FileURL == file.FileURL)
                {
                    using UnityWebRequest requestImage = UnityWebRequest.Head(file.FileURL);
                    yield return requestImage.SendWebRequest();

                    if (requestImage.result == UnityWebRequest.Result.Success)
                    {
                        string remoteFileSize = requestImage.GetResponseHeader("Content-Length");
                        string filePath = Path.Combine(file.LocalURL, file.FileName);
                        string localFileSize = new FileInfo(filePath).Length.ToString();

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
                string filePath = Path.Combine(file.LocalURL, file.FileName);
                File.WriteAllBytes(filePath, request.downloadHandler.data);

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
    public IEnumerator AsyncOperationDownloadWithProgress(List<GameAssetsFiles> gameAssetsFiles, UnityEngine.UI.Slider progressSlider, TextMeshProUGUI progressText)
    {
        // Initialize the slider
        progressSlider.value = 0;
        progressSlider.maxValue = gameAssetsFiles.Count;
        progressText.text = "0";

        // Load the existing list of files from storage
        var loadTask = LocalSaveManager.Instance.LoadDataAsync<List<GameAssetsFiles>>("GameAssetsFiles");
        yield return new WaitUntil(() => loadTask.IsCompleted);
        List<GameAssetsFiles> savedFiles = loadTask.Result ?? new List<GameAssetsFiles>();

        int completedFiles = 0;

        foreach (var file in gameAssetsFiles)
        {
            GameAssetsFiles gameAssetsLoad = savedFiles.Find(f => f.FileName == file.FileName);
            string localPath = Path.Combine(file.LocalURL, file.FileName);

            if (gameAssetsLoad != null && File.Exists(localPath))
            {
                Debug.Log($"{file.FileName} exists locally. Checking for updates...");

                if (gameAssetsLoad.FileURL == file.FileURL)
                {
                    using UnityWebRequest requestImage = UnityWebRequest.Head(file.FileURL);
                    yield return requestImage.SendWebRequest();

                    if (requestImage.result == UnityWebRequest.Result.Success)
                    {
                        string remoteFileHash = requestImage.GetResponseHeader("ETag")?.Trim('"'); // Use ETag as a hash if available
                        string localFileHash = CalculateMD5(localPath);

                        if (!string.IsNullOrEmpty(remoteFileHash) && remoteFileHash == localFileHash)
                        {
                            Debug.Log($"{file.FileName} is up to date.");
                            completedFiles++;
                            progressSlider.value = completedFiles;
                            progressText.text = (completedFiles * 100 / gameAssetsFiles.Count) + "%";

                            OnDownloadCompleted?.Invoke(true);
                            continue;
                        }
                        else
                        {
                            Debug.Log($"{file.FileName} has been updated or hash mismatch. Downloading new version...");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to check {file.FileName} for updates: {requestImage.error}");

                        OnDownloadCompleted?.Invoke(false);
                        continue;
                    }
                }
                else
                {
                    OnDownloadCompleted?.Invoke(false);

                    Debug.Log($"{file.FileName} has been updated. Downloading new version...");
                }
            }

            Debug.Log($"Downloading {file.FileName} from {file.FileURL}");

            using UnityWebRequest request = UnityWebRequest.Get(file.FileURL);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(localPath, request.downloadHandler.data);

                if (gameAssetsLoad != null)
                {
                    savedFiles.Remove(gameAssetsLoad);
                }
                savedFiles.Add(file);

                Debug.Log($"Downloaded and saved: {localPath}");

                OnDownloadCompleted?.Invoke(true);
            }
            else
            {
                Debug.LogError($"Failed to download {file.FileName}: {request.error}");

                OnDownloadCompleted?.Invoke(false);
            }

            // Update the slider progress
            completedFiles++;
            
            progressText.text = (completedFiles * 100 / gameAssetsFiles.Count) + "%";
            progressSlider.value = completedFiles;
        }

        var saveTask = LocalSaveManager.Instance.SaveDataAsync(savedFiles, "GameAssetsFiles");
        yield return new WaitUntil(() => saveTask.IsCompleted);

        Debug.Log("All assets have been updated and saved.");
    }
    private void OnDestroy() {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
    }
    private string CalculateMD5(string filePath)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public struct UserAttributes { }
    public struct AppAttributes { }
}
