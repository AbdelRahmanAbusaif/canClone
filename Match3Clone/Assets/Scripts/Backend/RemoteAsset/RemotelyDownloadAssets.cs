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

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "DownloadedAssets");
        Directory.CreateDirectory(savePath);
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
    public IEnumerator AsyncOperationDownloadWithProgress(List<GameAssetsFiles> gameAssetsFiles, UnityEngine.UI.Slider progressSlider, TextMeshProUGUI progressText)
    {
        // Initialize the slider
        progressSlider.value = 0;
        progressSlider.maxValue = gameAssetsFiles.Count;
        progressText.text = "0";

        int completedFiles = 0;

        foreach (var file in gameAssetsFiles)
        {
            string localPath = Path.Combine(file.LocalURL, file.FileName);

            Debug.Log($"Downloading {file.FileName} from {file.FileURL}");

            using UnityWebRequest request = UnityWebRequest.Get(file.FileURL);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(localPath, request.downloadHandler.data);

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

        Debug.Log("All assets have been downloaded.");
    }

    private void OnDestroy() {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
    }

    public struct UserAttributes { }
    public struct AppAttributes { }
}
