using SaveData;
using System.Threading.Tasks;
using UnityEngine;

public class FriendRequestGUI : MonoBehaviour
{
	[Header("Prefabs")]

	[SerializeField] private GameObject friendRequestPrefabs;

	[Header("UI Elements")]
	[SerializeField] private GameObject friendListRequestContainer;
	[SerializeField] private GameObject thereIsNoRequstPage;

	private async void OnEnable()
	{
		var friendRequestList = FriendSystemManager.Instance.GetIncomingRequests();

		// Destroy all previous friend request profiles
		foreach (Transform child in friendListRequestContainer.transform)
		{
			Destroy(child.gameObject);
		}

		friendListRequestContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(friendListRequestContainer.GetComponent<RectTransform>().sizeDelta.x, 0);

		if (friendRequestList.Count == 0)
		{
			Debug.Log("There is no friend request");
			thereIsNoRequstPage.SetActive(true);
			return;
		}
		else
		{
			Debug.Log("There are friend requests");
			thereIsNoRequstPage.SetActive(false);
		}
		foreach (var friendRequest in friendRequestList)
		{
			var friendRequestInfo = Instantiate(friendRequestPrefabs, friendListRequestContainer.transform);

			friendListRequestContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(friendListRequestContainer.GetComponent<RectTransform>().sizeDelta.x, friendListRequestContainer.GetComponent<RectTransform>().sizeDelta.y + 200);

			var friendRequestInfoGUI = friendRequestInfo.GetComponent<FriendRequestInfoGUI>();
			var friendProfile = await CloudSaveManager.Instance.LoadPublicDataByPlayerIdAsync<PlayerProfile>(friendRequest.Member.Id,"PlayerProfile");

			friendRequestInfoGUI.SetFriendInfo(friendProfile.PlayerName, friendRequest.Member.Id);
		}
	}
}
