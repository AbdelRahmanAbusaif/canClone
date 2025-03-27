using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static RemotelyDownloadAssets;

public class LeaderboardButtonManager : MonoBehaviour
{
    [SerializeField] private List<LeaderboardButton> leaderboardButtons;
    bool isFirstLeaderboardEnable = true;
    public void ApplyRemoteConfig(ConfigResponse response)
    {
        // Implement the logic to apply remote config settings to leaderboard buttons

        foreach(var leaderboardButton in leaderboardButtons)
        {
            leaderboardButton.isLeaderboardActive = RemoteConfigService.Instance.appConfig.GetBool(leaderboardButton.buttonName);
            leaderboardButton.Button.SetActive(leaderboardButton.isLeaderboardActive);

            leaderboardButton.leaderboardText.text = RemoteConfigService.Instance.appConfig.GetString(leaderboardButton.LeaderboardTitleKey);
            
            if(leaderboardButton.isLeaderboardActive && isFirstLeaderboardEnable)
            {
                isFirstLeaderboardEnable = false;
                leaderboardButton.LeaderboardPanel.SetActive(true);
            }
        }
    }

    public void DisableAllLeaderboardButtons()
    {
        foreach (var leaderboardButton in leaderboardButtons)
        {
            leaderboardButton.LeaderboardPanel.SetActive(false);
        }
    }
    private void OnDestroy() 
    {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
    }
}
[Serializable]
public class LeaderboardButton
{
    public GameObject Button;
    public GameObject LeaderboardPanel;
    public TextMeshProUGUI leaderboardText;
    public string LeaderboardTitleKey;
    public string buttonName;
    public bool isLeaderboardActive;
}