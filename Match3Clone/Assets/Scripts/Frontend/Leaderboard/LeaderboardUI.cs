using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();


}


[System.Serializable]
public struct LeaderboardEntry
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI score;
    public TextMeshProUGUI rank;
    public Image playerAvatar;
}