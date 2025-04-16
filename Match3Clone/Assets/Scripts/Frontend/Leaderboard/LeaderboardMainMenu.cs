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
    [SerializeField] private int playerPerPage = 9;
    [SerializeField] private LeaderboardItem leaderboardItemPrefab;
    [SerializeField] private RectTransform playerContainer;
    
    [SerializeField] private List<LeaderboardItem> leaderboardItems = new List<LeaderboardItem>();
    private void TestAddScore()
    {
        LeaderboardManager.Instance.AddScore(leaderboardId,Random.Range(0, 1000));
        Debug.Log("Score added successfully.");
    }
    private async void OnEnable()
    {

        try
        {
            ClearPlayer();
           
            var playerScore = await LeaderboardManager.Instance.GetPlayerProfileScore(leaderboardId);
            var scoreResponse = await LeaderboardManager.Instance.GetPlayerScore(leaderboardId);

            for (int i = 3; i < scoreResponse.Results.Count && i < playerPerPage; i++)
            {
                if (scoreResponse.Results[i].PlayerId == playerScore.PlayerId)
                    continue;
                LeaderboardItem leaderboardItem = Instantiate(leaderboardItemPrefab, playerContainer);
                playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + leaderboardItem.GetComponent<RectTransform>().sizeDelta.y + 30f);

                leaderboardItem.Initializer(scoreResponse.Results[i]);
            }

            var playerMainProfile = Instantiate(leaderboardItemPrefab, playerContainer);

            playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + playerMainProfile.GetComponent<RectTransform>().sizeDelta.y + 30f);
            playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + playerMainProfile.GetComponent<RectTransform>().sizeDelta.y + 30f);
            playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + playerMainProfile.GetComponent<RectTransform>().sizeDelta.y + 30f);
            
            playerMainProfile.Initializer(playerScore);

            foreach (var leaderboardItem in leaderboardItems)
            {
                leaderboardItem.Initializer(scoreResponse.Results[leaderboardItems.IndexOf(leaderboardItem)]);
            }

        }
        catch (Exception e) when (e is TaskCanceledException || e is TimeoutException)
        {
            Debug.LogError("Failed to get player score: " + e.Message);
            throw;
        }
    }
    private void ClearPlayer()
    {
        playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, 0);
        foreach (Transform child in playerContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
