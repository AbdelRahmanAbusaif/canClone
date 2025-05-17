using SaveData;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FriendRequestInfoGUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI friendName;

	[SerializeField] private Image friendAvatar;
	[SerializeField] private Image friendBorder;

	[SerializeField] private Button acceptButton;
	[SerializeField] private Button rejectButton;

	private string friendId;

	private void Start()
	{
		acceptButton.onClick.AddListener(OnAcceptButtonClicked);
		rejectButton.onClick.AddListener(OnRejectButtonClicked);
	}

	private async void OnRejectButtonClicked()
	{
		if (string.IsNullOrEmpty(friendId))
		{
			Debug.LogError("Friend ID is not set.");
			return;
		}
		await FriendSystemManager.Instance.RejectFriendRequest(friendId);
		Destroy(gameObject);
	}

	private async void OnAcceptButtonClicked()
	{
		if (string.IsNullOrEmpty(friendId))
		{
			Debug.LogError("Friend ID is not set.");
			return;
		}
		await FriendSystemManager.Instance.AcceptFriendRequest(friendId);
		Destroy(gameObject);
	}

	public void SetFriendInfo(string name, string id)
	{
		friendName.text = name;

		Debug.Log($"Loading images for {id}");
		CloudSaveManager.Instance.LoadImageUsePlayerId(id, "PlayerProfileImage", friendAvatar);
		CloudSaveManager.Instance.LoadImageUsePlayerId(id, "PlayerProfileBorderImage", friendBorder);

		friendId = id;
	}
}