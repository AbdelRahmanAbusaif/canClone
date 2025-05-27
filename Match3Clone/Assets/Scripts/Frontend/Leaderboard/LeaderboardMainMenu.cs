using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class LeaderboardMainMenu : MonoBehaviour
{
    [SerializeField] private string leaderboardId;
    [SerializeField] private string leaderboardTitleKey;
    [SerializeField] private int playerPerPage = 9;
    [SerializeField] private LeaderboardItem leaderboardItemPrefab;
    [SerializeField] private LeaderboardItem myLeaderboardItemPrefab;
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
                {
                    var playerMainProfile = Instantiate(myLeaderboardItemPrefab, playerContainer);
                    
                    playerMainProfile.Initializer(playerScore);
                    continue;
                }
                LeaderboardItem leaderboardItem = Instantiate(leaderboardItemPrefab, playerContainer);
                playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + leaderboardItem.GetComponent<RectTransform>().sizeDelta.y + 100f);

                leaderboardItem.Initializer(scoreResponse.Results[i]);
            }
            int count = 0;
            foreach (var leaderboardItem in leaderboardItems)
            {
                count++;
                if (scoreResponse.Results.Count <= leaderboardItems.IndexOf(leaderboardItem))
                {
                    Debug.Log("Skipping leaderboard item initialization due to insufficient scores.");
                    MYLeaderboardEntry mYLeaderboardEntry = new MYLeaderboardEntry
                    {
                        PlayerId = System.Guid.NewGuid().ToString(),
                        PlayerName = ((Names)Random.Range(0, Enum.GetValues(typeof(Names)).Length)).ToString(),
                        Score = 0,
                        Rank = count
                    };
                    leaderboardItem.SetFakeData(mYLeaderboardEntry);
                    continue;
                }
                leaderboardItem.Initializer(scoreResponse.Results[leaderboardItems.IndexOf(leaderboardItem)]);
            }
            Debug.Log("Player Id " + playerScore.PlayerId);

            if (scoreResponse.Results.
            FirstOrDefault(x => x.PlayerId == playerScore.PlayerId) == null)
            {

                var playerMainProfile = Instantiate(myLeaderboardItemPrefab, playerContainer);

                playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + playerMainProfile.GetComponent<RectTransform>().sizeDelta.y + 30f);
                playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + playerMainProfile.GetComponent<RectTransform>().sizeDelta.y + 30f);
                playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + playerMainProfile.GetComponent<RectTransform>().sizeDelta.y + 30f);

                playerMainProfile.Initializer(playerScore);
                return;
            }
            
            if (scoreResponse.Results.Count < playerPerPage)
            {
                for (int i = scoreResponse.Results.Count; i < playerPerPage; i++)
                {
                    LeaderboardItem leaderboardItem = Instantiate(leaderboardItemPrefab, playerContainer);

                    Names names = (Names)Random.Range(0, Enum.GetValues(typeof(Names)).Length);
                    MYLeaderboardEntry leaderboardEntry = new MYLeaderboardEntry
                    {
                        PlayerId = System.Guid.NewGuid().ToString(),
                        PlayerName = names.ToString(),
                        Score = 0,
                        Rank = i + 1
                    };
                    leaderboardItem.SetFakeData(leaderboardEntry);
                    playerContainer.sizeDelta = new Vector2(playerContainer.sizeDelta.x, playerContainer.sizeDelta.y + leaderboardItem.GetComponent<RectTransform>().sizeDelta.y + 30f);
                }
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

public class MYLeaderboardEntry
{
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int Score { get; set; }
    public int Rank { get; set; }
}
public enum Names
{
    Mohammad,
    Ali,
    Reza,
    Sara,
    Fatemeh,
    Niloofar,
    Mahsa,
    Parisa,
    Yasaman,
    Narges,
    Shirin,
    Leila,
    Samira,
    Setareh,
    Shadi,
    Yasmin,
    Zeynab,
    Zahra,
    Mahin,
    AbdelRahman,
    Ahmed,
}