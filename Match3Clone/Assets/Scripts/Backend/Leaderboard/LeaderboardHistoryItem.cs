using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class LeaderboardHistoryItem : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private Button showMessage;
	[SerializeField] private Image image;

	[SerializeField] private GameObject historyPagePrefabs; // Reference to the history page GameObject

	string descriptionEnText = string.Empty; // Placeholder for description, if needed later
	string descriptionArText = string.Empty; // Placeholder for description, if needed later

	string imageURL = string.Empty; // Placeholder for image URL, if needed later

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(imageURL))
		{
			StartCoroutine(LoadImage(imageURL));
		}
	}
	private void Start()
	{
		showMessage.onClick.AddListener(() =>
		{
			var historyPage = Instantiate(historyPagePrefabs, transform.parent.parent.parent.parent.parent);

			historyPage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset position to center
			historyPage.GetComponent<RectTransform>().localScale = Vector3.one; // Reset scale to default

			var historyPageComponent = historyPage.GetComponent<LeaderboardHistoryPage>();
			if (historyPageComponent != null)
			{
				historyPageComponent.SetData(titleText.text, descriptionArText, descriptionEnText);
			}
			else
			{
				Debug.LogError("LeaderboardHistoryPage component not found on the instantiated prefab.");
			}
		});
	}
	public void SetData(LeaderboardHistoryData data)
	{
		titleText.text = data.title;
		//descriptionText.text = data.description;

		descriptionEnText = data.contentEn ?? string.Empty; // Ensure description is not null
		descriptionArText = data.contentAr ?? string.Empty; // Ensure description is not null

		imageURL = data.imageURL ?? string.Empty; // Ensure imageURL is not null
	}
	private IEnumerator LoadImage(string url)
	{
		using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			Texture2D texture = DownloadHandlerTexture.GetContent(request);
			image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		}
		else
		{
			Debug.LogError($"Failed to load image from {url}: {request.error}");
		}
	}
}