using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteAssetsLoader : MonoBehaviour
{private string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/DownloadedAssets/";
        Directory.CreateDirectory(savePath);

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
    }

    private void ApplyRemoteConfig(ConfigResponse response)
    {
        Debug.Log("Remote Config Fetched Successfully!");

        Dictionary<string, string> fileUrls = new()

        {
            { "logo.png", RemoteConfigService.Instance.appConfig.GetString("LogoURL") }
        };

        StartCoroutine(DownloadAndSaveFiles(fileUrls));
    }

    private IEnumerator DownloadAndSaveFiles(Dictionary<string, string> fileUrls)
    {
        foreach (var file in fileUrls)
        {
            string localFilePath = Path.Combine(savePath, file.Key);

            if (File.Exists(localFilePath))
            {
                Debug.Log($"{file.Key} already exists locally.");
                continue;
            }

            Debug.Log($"Downloading {file.Key} from {file.Value}");

            using UnityWebRequest request = UnityWebRequest.Get(file.Value);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytesAsync(localFilePath, request.downloadHandler.data);
                Debug.Log($"Downloaded and saved: {localFilePath}");
            }
            else
            {
                Debug.LogError($"Failed to download {file.Key}: {request.error}");

            }
        }

        Debug.Log("All assets downloaded successfully!");
    }
    

    public struct UserAttributes { }
    public struct AppAttributes { }
}
