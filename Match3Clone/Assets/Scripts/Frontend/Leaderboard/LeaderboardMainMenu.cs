using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static RemotelyDownloadAssets;
using Random = UnityEngine.Random;

public class LeaderboardMainMenu : MonoBehaviour
{
    [SerializeField] private string leaderboardId;
    [SerializeField] private string leaderboardTitleKey;
    [SerializeField] private int playerPerPage = 10;
    [SerializeField] private List<LeaderboardItem> leaderboardItemPrefab;
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private LeaderboardItem leaderboardPlayerProfile;
    [SerializeField] private RectTransform playerContainer;
    
    private void TestAddScore()
    {
        LeaderboardManager.Instance.AddScore(leaderboardId,Random.Range(0, 1000));
        Debug.Log("Score added successfully.");
    }
    private async void OnEnable()
    {

        try
        {
            // TestAddScore();
            // Debug.Log("Test Add Score");

            var playerScore = await LeaderboardManager.Instance.GetPlayerProfileScore(leaderboardId);
            leaderboardPlayerProfile.Initializer(playerScore);

            ClearPlayer();
            var scoreResponse = await LeaderboardManager.Instance.GetPlayerScore(leaderboardId);

            for (int i = 0; i < scoreResponse.Results.Count && i < playerPerPage ; i++)
            {
                int prefabIndex = Mathf.Min(i, leaderboardItemPrefab.Count - 1);
                LeaderboardItem leaderboardItem = Instantiate(leaderboardItemPrefab[prefabIndex], playerContainer);

                leaderboardItem.Initializer(scoreResponse.Results[i]);
            }
        }
        catch (Exception e) when (e is TaskCanceledException || e is TimeoutException)
        {
            Debug.LogError("Failed to get player score: " + e.Message);
            throw;
        }
    }

    public void ApplyRemoteConfig(ConfigResponse response)
    {
        leaderboardText.text = RemoteConfigService.Instance.appConfig.GetString(leaderboardTitleKey);
    }

    private void ClearPlayer()
    {
        foreach (Transform child in playerContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
