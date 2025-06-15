using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class ApplyRemoteLicense : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI licenseText;
    private async void Start()
    {
        RemoteConfigService.Instance.FetchCompleted += Apply;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
    }

    private void Apply(ConfigResponse obj)
    {
        if (obj.status == ConfigRequestStatus.Success)
        {
            Debug.Log("Remote Config Fetched Successfully in ApplyRemoteLicense!");
            // Here you can apply the remote config settings as needed
            // For example, you might want to update some UI elements or game settings
            
            licenseText.text = RemoteConfigService.Instance.appConfig.GetString("MusicLicense", "Default License Text");
        }
        else
        {
            Debug.LogError("Failed to fetch remote config: " + obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
