using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class LeaderboardMainMenu : MonoBehaviour
{
    [SerializeField] private int playerPerPage = 10;
    [SerializeField] private List<LeaderboardItem> leaderboardItemPrefab;
    [SerializeField] private LeaderboardItem leaderboardPlayerProfile;
    [SerializeField] private RectTransform playerContainer;
    
    private void TestAddScore()
    {
        LeaderboardManager.Instance.AddScore(Random.Range(0, 1000));

        Debug.Log("Score added successfully.");
    }
    private async void OnEnable()
    {

        try
        {
            TestAddScore();
            Debug.Log("Test Add Score");

            var playerScore = await LeaderboardManager.Instance.GetPlayerProfileScore();
            leaderboardPlayerProfile.Initializer(playerScore);

            ClearPlayer();
            var scoreResponse = await LeaderboardManager.Instance.GetPlayerScore();

            for (int i = 0; i < scoreResponse.Results.Count && i < playerPerPage ; i++)
            {
                int prefabIndex = Mathf.Min(i, leaderboardItemPrefab.Count - 1);
                LeaderboardItem leaderboardItem = Instantiate(leaderboardItemPrefab[prefabIndex], playerContainer);

                leaderboardItem.Initializer(scoreResponse.Results[i]);
            }
        }
        catch (System.Exception e) when (e is TaskCanceledException || e is TimeoutException)
        {
            Debug.LogError("Failed to get player score: " + e.Message);
            throw;
        }
    }
    private void ClearPlayer()
    {
        foreach (Transform child in playerContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
