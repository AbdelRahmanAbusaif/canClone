using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;

public class FriendSystemManager : MonoBehaviour
{
	// Singleton instance
	public static FriendSystemManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	async void Start()
	{
		await UnityServices.InitializeAsync();
		await FriendsService.Instance.InitializeAsync();

		if (!AuthenticationService.Instance.IsSignedIn)
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
			Debug.Log("Signed in anonymously with Player ID: " + AuthenticationService.Instance.PlayerId);
		}
	}

	// ✅ Send Friend Request
	public async Task SendFriendRequest(string targetPlayerId)
	{
		try
		{
			await FriendsService.Instance.AddFriendAsync(targetPlayerId);
			Debug.Log($"Sent friend request to {targetPlayerId}");
		}
		catch (System.Exception e)
		{
			Debug.LogError("SendFriendRequest failed: " + e.Message);
		}
	}

	// ✅ Get My Friends
	public IReadOnlyList<Relationship> GetFriends()
	{
		try
		{
			var friends = FriendsService.Instance.Friends;
			Debug.Log($"You have {friends.Count} friend(s)");
			return friends;
		}
		catch (System.Exception e)
		{
			Debug.LogError("GetFriends failed: " + e.Message);
			return new List<Relationship>();
		}
	}

	// ✅ Remove a Friend
	public async Task RemoveFriend(string friendId)
	{
		try
		{
			await FriendsService.Instance.DeleteFriendAsync(friendId);
			Debug.Log($"Removed friend: {friendId}");
		}
		catch (System.Exception e)
		{
			Debug.LogError("RemoveFriend failed: " + e.Message);
		}
	}

	// ✅ Get Incoming Friend Requests
	public IReadOnlyList<Relationship> GetIncomingRequests()
	{
		try
		{
			var requests = FriendsService.Instance.IncomingFriendRequests;
			Debug.Log($"You have {requests.Count} incoming request(s)");
			return requests;
		}
		catch (System.Exception e)
		{
			Debug.LogError("GetIncomingRequests failed: " + e.Message);
			return new List<Relationship>();
		}
	}

	// ✅ Accept Friend Request
	public async Task AcceptFriendRequest(string senderPlayerId)
	{
		try
		{
			await FriendsService.Instance.AddFriendAsync(senderPlayerId);
			Debug.Log($"Accepted friend request from: {senderPlayerId}");
		}
		catch (System.Exception e)
		{
			Debug.LogError("AcceptFriendRequest failed: " + e.Message);
		}
	}

	// ✅ Reject Friend Request
	public async Task RejectFriendRequest(string senderPlayerId)
	{
		try
		{
			await FriendsService.Instance.DeleteIncomingFriendRequestAsync(senderPlayerId);
			Debug.Log($"Rejected friend request from: {senderPlayerId}");
		}
		catch (System.Exception e)
		{
			Debug.LogError("RejectFriendRequest failed: " + e.Message);
		}
	}
}
