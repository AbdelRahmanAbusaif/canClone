using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardHistory : MonoBehaviour
{
    private readonly string REMOTE_CONFIG_LEADERBOARD_KEY = "LeaderboardHistoryDatas";

	[SerializeField] private Transform contentParent;
	[SerializeField] private GameObject leaderboardHistoryPrefab;

	[SerializeField] private List<LeaderboardHistoryData> leaderboardHistoryDatas = new List<LeaderboardHistoryData>();
	void OnEnable()
    {
		Debug.Log("Fetching leaderboard history from remote config...");
		RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetched;
	}

	private void OnRemoteConfigFetched(ConfigResponse response)
	{
		var json = RemoteConfigService.Instance.appConfig
			.GetJson(REMOTE_CONFIG_LEADERBOARD_KEY);

		Debug.Log($"Leaderboard History JSON: {json}");
		leaderboardHistoryDatas = JsonConvert.DeserializeObject<List<LeaderboardHistoryData>>(json);

		leaderboardHistoryDatas.Sort((x, y) => string.Compare(x.versionId, y.versionId, StringComparison.Ordinal));

		if (leaderboardHistoryDatas != null && leaderboardHistoryDatas.Count > 0)
		{
			foreach (var data in leaderboardHistoryDatas)
			{
				var historyItem = Instantiate(leaderboardHistoryPrefab, contentParent);
				historyItem.GetComponent<LeaderboardHistoryItem>().SetData(data);

				contentParent.GetComponent<VerticalLayoutGroup>().enabled = false; // Disable layout to prevent immediate re-layout
				// Set the leaderboard type as a tag for filtering if needed

				contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(contentParent.GetComponent<RectTransform>().sizeDelta.x,
				contentParent.GetComponent<RectTransform>().sizeDelta.y + historyItem.GetComponent<RectTransform>().sizeDelta.y + 20f); // Adjust height based on number of items

				contentParent.GetComponent<VerticalLayoutGroup>().enabled = true; // Re-enable layout after instantiation
			}
		}
		else
		{
			Debug.LogWarning("No leaderboard history data found.");
		}
	}
	private void OnDisable()
	{
		RemoteConfigService.Instance.FetchCompleted -= OnRemoteConfigFetched;
	}
}
[Serializable]
public class LeaderboardHistoryData
{
    public string versionId;
    public string title; 
	
	public LeaderboardType leaderboardType;

	public string contentEn;
	public string contentAr;

	public string imageURL;
}
public enum LeaderboardType
{
	BLUE,
	RED,
	GREEN,
	YELLOW,
	PURPLE,
	ORANGE,
	None
}
