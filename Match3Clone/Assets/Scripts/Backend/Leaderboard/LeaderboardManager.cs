using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }
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
    public async void AddScore(string leaderboardId,int score)
    {   
        var playerEntry = await LeaderboardsService.Instance
        .AddPlayerScoreAsync(
            leaderboardId,
            score
        );
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
    public async Task<LeaderboardScoresPage> GetPlayerScore(string leaderboardId)
    {
        var scoreResponse = await LeaderboardsService.Instance
        .GetScoresAsync(
            leaderboardId,
            new GetScoresOptions { IncludeMetadata = true }
        );

        Debug.Log( "Score respawn:" + JsonConvert.SerializeObject(scoreResponse));

        return scoreResponse;
    }
	public async Task<LeaderboardVersionScoresPage> GetPaginatedVersionScores(string leaderboardId, string versionId)
	{
		var scoresResponse = await LeaderboardsService.Instance.GetVersionScoresAsync(
			leaderboardId,
			versionId,
			new GetVersionScoresOptions { Offset = 25, Limit = 50 }
		);
		Debug.Log(JsonConvert.SerializeObject(scoresResponse));

		return scoresResponse;
	}
	public async Task<LeaderboardEntry> GetPlayerProfileScore(string leaderboardId)
    {
        var scoreResponse = await LeaderboardsService.Instance
        .GetPlayerScoreAsync(
            leaderboardId
        );

        return scoreResponse;
    }
}