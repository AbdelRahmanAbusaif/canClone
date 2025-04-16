using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;
using static RemotelyDownloadAssets;

public class LeaderboardButtonManager : MonoBehaviour
{
    [SerializeField] private List<LeaderboardButton> leaderboardButtons;
    bool isFirstLeaderboardEnable = true;
    private void Start() 
    {
        isFirstLeaderboardEnable = true;
    }
    public void ApplyRemoteConfig(ConfigResponse response)
    {
        // Implement the logic to apply remote config settings to leaderboard buttons

        foreach(var leaderboardButton in leaderboardButtons)
        {
            leaderboardButton.isLeaderboardActive = RemoteConfigService.Instance.appConfig.GetBool(leaderboardButton.buttonName);
            leaderboardButton.Button.SetActive(leaderboardButton.isLeaderboardActive);

            if(!leaderboardButton.isLeaderboardActive)
            continue;
            
            Button button = leaderboardButton.Button.GetComponent<Button>();
            button.onClick.AddListener
            (
                ()=>
                {
                    DisableAllLeaderboardButtons();
                    leaderboardButton.LeaderboardPanel.SetActive(true);
                    Image image = leaderboardButton.Button.GetComponent<Image>();
                    image.color = new Color(255f / 255f,162f / 255f,0,255f / 255f);
                }
            );
            leaderboardButton.leaderboardText.text = RemoteConfigService.Instance.appConfig.GetString(leaderboardButton.LeaderboardTitleKey);
            
            if(leaderboardButton.isLeaderboardActive && isFirstLeaderboardEnable)
            {
                isFirstLeaderboardEnable = false;
                leaderboardButton.LeaderboardPanel.SetActive(true);
                Image image = leaderboardButton.Button.GetComponent<Image>();
                image.color = new Color(image.color.r,image.color.g,image.color.b, 255f / 255f); 
                leaderboardButton.Button.GetComponent<Button>().onClick.Invoke();
                Debug.Log("First leaderboard enabled: " + leaderboardButton.buttonName);
            }
        }
    }

    public void DisableAllLeaderboardButtons()
    {
        foreach (var leaderboardButton in leaderboardButtons)
        {
            leaderboardButton.LeaderboardPanel.SetActive(false);
            Image image = leaderboardButton.Button.GetComponent<Image>();
            image.color = new Color(image.color.r,image.color.g,image.color.b,20f / 255f);
            Debug.Log("DisableAllLeaderboardButtons called and reduces the color of the button to 20f alpha with name " + leaderboardButton.buttonName);
        }
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