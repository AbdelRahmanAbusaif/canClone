using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameVanilla.Game.Common;
using GameVanilla.Game.Scenes;
using SaveData;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;
using static RemotelyDownloadAssets;

public class ScoreSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private FxPool fxPool;
    [SerializeField] private GameScene gameScene;

    [SerializeField] private List<Score> scores;

    private PlayerProfile playerProfile;    

    private async void Start()
    {   
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());

        fxPool.OnExplode += OnCandyExplode;
        gameScene.OnWinPopupOpened += OnGameWin;
        gameScene.OnLosePopupOpened += OnGameLose;
    }

    private void ApplyRemoteConfig(ConfigResponse response)
    {
        foreach (var score in scores)
        {
            score.isScoreAvailable = RemoteConfigService.Instance.appConfig.GetBool(score.remoteConfigKey);

            if (score.isScoreAvailable)
            {
                Debug.Log($"Score for {score.candyColor} candy is available");
                score.scorePanel.SetActive(true);
            }
        }
    }

    // here we can implement the logic to save the score when the game is won

    private void OnGameWin()
    {
        // Implement the logic to save the score when the game is won
        Debug.Log("Game won");

        foreach (var score in scores)
        {
            if (score.isScoreAvailable && playerProfile.LevelsComplete.Find(L => L.NumberLevel == gameScene.level.id) == null)
            {
                LeaderboardManager.Instance.AddScore(score.leaderboardId, score.score);
                Debug.Log($"Score for {score.candyColor} candy is {score.score}");
            }
        }
    }
    private void OnGameLose()
    {
        // Implement the logic to save the score when the game is lost
        Debug.Log("Game lost");

        foreach (var score in scores)
        {
            if (score.isScoreAvailable && playerProfile.LevelsComplete.Find(L => L.NumberLevel == gameScene.level.id) == null)
            {
                score.score = 0;
                score.scoreText.text = score.score.ToString();
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
                score.scoreText.text = score.score.ToString();
            }
        }
    }

    private void OnDestroy() 
    {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;

        fxPool.OnExplode -= OnCandyExplode;
        gameScene.OnWinPopupOpened -= OnGameWin;
    }
    [Serializable]
    public class Score
    {
        internal int score;
        internal bool isScoreAvailable;
        public string leaderboardId;
        public string remoteConfigKey;
        public CandyColor candyColor;
        public TextMeshProUGUI scoreText;
        public GameObject scorePanel;
    }
}