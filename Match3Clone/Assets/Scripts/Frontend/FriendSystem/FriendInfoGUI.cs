using SaveData;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FriendInfoGUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI friendName;
	[SerializeField] private TextMeshProUGUI friendLevel;
	[SerializeField] private TextMeshProUGUI friendRank;

	[SerializeField] private Image friendAvatar;
	[SerializeField] private Image friendBorder;

	private string friendId;

	public void SetFriendInfo(string name, string id, string level, string rank)
	{
		friendName.text = name;
		friendLevel.text = level;
		friendRank.text = rank;

		CloudSaveManager.Instance.LoadImageUsePlayerId(id, "PlayerProfileImage", friendAvatar);
		CloudSaveManager.Instance.LoadImageUsePlayerId(id, "PlayerProfileBorderImage", friendBorder);

		friendId = id;
	}
}