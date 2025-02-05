using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using UnityEditor.Timeline.Actions;
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

    #if UNITY_EDITOR
        [ContextMenu("Add Score")]
    #endif
    public async void AddScore(int score)
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
    #if UNITY_EDITOR
        [ContextMenu("Add Score")]
    #endif
    public async void GetPlayerScore()
    {
        var scoreResponse = await LeaderboardsService.Instance
            .GetPlayerScoreAsync(leaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }
    public async void GetPaginatedScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
            leaderboardId,
            new GetScoresOptions{ Offset = 25, Limit = 50 }
        );
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
    public async void GetPlayerRange()
    {
        // Returns a total of 11 entries (the given player plus 5 on either side)
        var rangeLimit = 5;
        var scoresResponse = await LeaderboardsService.Instance.GetPlayerRangeAsync(
            leaderboardId,
            new GetPlayerRangeOptions{ RangeLimit = rangeLimit }
        );
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
}