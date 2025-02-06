using SaveData;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image avatarImage;

    private LeaderboardEntry player = null;

    public void Initializer(LeaderboardEntry player)
    {
        this.player = player;

        int rankInt = player.Rank + 1;
        rankText.text = rankInt.ToString();
    
        nameText.text = player.PlayerName;
        scoreText.text = player.Score.ToString();

        CloudSaveManager.Instance.LoadImageUsePlayerId(player.PlayerId, avatarImage);
    }

}
