using SaveData;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Friends.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendSystemGUI : MonoBehaviour
{
	[Header("Prefabs")]

	[SerializeField] private GameObject myProfilePrefab;
	[SerializeField] private GameObject friendProfilePrefabs;

	[Header("UI Elements")]
	[SerializeField] private GameObject friendListProfileContainer;
	[SerializeField] private GameObject thereIsNoFriendPage;

	[SerializeField] private AnimationBox friendPageBox;

	[SerializeField] private Button addFriendButton;
	[SerializeField] private TMP_InputField addFriendInputField;

	[SerializeField] private List<FriendInfoGUI> friendList;
	private List<PlayerProfile> friends;
	private PlayerProfile myProfile;

	private void Awake()
	{
		addFriendButton.onClick.AddListener(OnAddFriendButtonClicked);
	}

	public async void OnAddFriendButtonClicked()
	{
		if(string.IsNullOrEmpty(addFriendInputField.text))
		{
			Debug.LogError("Input field is empty.");
			friendPageBox.OnClose();
			return;
		}
		await FriendSystemManager.Instance.SendFriendRequest(addFriendInputField.text);
		Debug.Log($"Friend request sent to {addFriendInputField.text}");
		addFriendInputField.text = string.Empty;
	}

	private async void OnEnable()
	{
		friendList = new List<FriendInfoGUI>();
		friends = new List<PlayerProfile>();

		var friendsTemp = FriendSystemManager.Instance.GetFriends();
		myProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

		// Destroy all previous friend profiles
		foreach (Transform child in friendListProfileContainer.transform)
		{
			Destroy(child.gameObject);
		}
		friendListProfileContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(friendListProfileContainer.GetComponent<RectTransform>().sizeDelta.x, 0);

		if (friendsTemp.Count == 0)
		{
			Debug.Log("There is no friend in the list");
			thereIsNoFriendPage.SetActive(true);
			return;
		}
		else
		{
			Debug.Log("There are friends in the list");
			thereIsNoFriendPage.SetActive(false);
		}

		friendList.Clear();
		friends.Clear();

		foreach (var friend in friendsTemp)
		{
			var friendInfo = await CloudSaveManager.Instance.LoadPublicDataByPlayerIdAsync<PlayerProfile>(friend.Member.Id, "PlayerProfile");
			friends.Add(friendInfo);
		}

		friends.Add(myProfile);
		friends.Sort((x, y) => x.Level.CompareTo(y.Level));
		friends.Reverse();


		for (int i = 0; i < friends.Count; i++)
		{
			PlayerProfile friend = friends[i];

			friendListProfileContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(friendListProfileContainer.GetComponent<RectTransform>().sizeDelta.x, friendListProfileContainer.GetComponent<RectTransform>().sizeDelta.y + 200);
			if (friend.PlayerId == myProfile.PlayerId)
			{
				var myInfo = Instantiate(myProfilePrefab, friendListProfileContainer.transform);
				var myInfoGUI = myInfo.GetComponent<FriendInfoGUI>();
				myInfoGUI.SetFriendInfo(friend.PlayerName, friend.PlayerId, friend.Level.ToString(), (i + 1).ToString());
				continue;
			}
			var friendInfo = Instantiate(friendProfilePrefabs, friendListProfileContainer.transform);
			var friendInfoGUI = friendInfo.GetComponent<FriendInfoGUI>();
			friendInfoGUI.SetFriendInfo(friend.PlayerName, friend.PlayerId, friend.Level.ToString(), (i + 1).ToString());
			friendList.Add(friendInfoGUI);
		}
	}
}
