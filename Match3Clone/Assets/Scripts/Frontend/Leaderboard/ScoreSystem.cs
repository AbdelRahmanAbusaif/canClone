using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameVanilla.Game.Common;
using GameVanilla.Game.Scenes;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static RemotelyDownloadAssets;

public class ScoreSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private FxPool fxPool;
    [SerializeField] private GameScene gameScene;

    [SerializeField] private List<Score> scores;


    private async void Start()
    {
        
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());

        fxPool.OnExplode += OnCandyExplode;
        gameScene.OnWinPopupOpened += OnGameWin;
    }

    private void ApplyRemoteConfig(ConfigResponse response)
    {
        foreach (var score in scores)
        {
            score.isScoreAvailable = RemoteConfigService.Instance.appConfig.GetBool(score.remoteConfigKey);
        }
    }

    // here we can implement the logic to save the score when the game is won

    private void OnGameWin()
    {
        // Implement the logic to save the score when the game is won
        Debug.Log("Game won");

        foreach (var score in scores)
        {
            if (score.isScoreAvailable)
            {
                LeaderboardManager.Instance.AddScore(score.leaderboardId, score.score);
                Debug.Log($"Score for {score.candyColor} candy is {score.score}");
            }
        }
    }


    private void OnCandyExplode(CandyColor candy)
    {
        Debug.Log($"{candy} candy exploded");

        foreach (var score in scores)
        {
            if (score.candyColor == candy && score.isScoreAvailable)
            {
                score.score++;
            }
        }
    }

    private void OnDestroy() 
    {
        fxPool.OnExplode -= OnCandyExplode;
    }
    [Serializable]
    public class Score
    {
        internal int score;
        internal bool isScoreAvailable;
        public string leaderboardId;
        public string remoteConfigKey;
        public CandyColor candyColor;
    }
}