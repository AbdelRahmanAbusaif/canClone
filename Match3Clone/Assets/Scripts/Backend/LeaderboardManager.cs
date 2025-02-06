using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }
    string leaderboardId  = "COIN_LEADERBOARD";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public async void AddScore(int score)
    {   
        var playerEntry = await LeaderboardsService.Instance
        .AddPlayerScoreAsync(
            leaderboardId,
            score
        );
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
    public async Task<LeaderboardScoresPage> GetPlayerScore()
    {
        var scoreResponse = await LeaderboardsService.Instance
        .GetScoresAsync(
            leaderboardId,
            new GetScoresOptions { IncludeMetadata = true }
        );

        return scoreResponse;
    }
    public async Task<LeaderboardEntry> GetPlayerProfileScore()
    {
        var scoreResponse = await LeaderboardsService.Instance
        .GetPlayerScoreAsync(
            leaderboardId
        );

        Debug.Log(JsonConvert.SerializeObject(scoreResponse));

        return scoreResponse;
    }
}
[Serializable]
public class MetadataScore
{
    public string playerId;
    public string timeTaken;
}