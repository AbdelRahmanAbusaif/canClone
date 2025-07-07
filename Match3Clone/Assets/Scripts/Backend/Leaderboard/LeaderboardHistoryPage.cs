using ArabicSupporter;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class LeaderboardHistoryPage : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI contentText;

	[SerializeField] private Button closeButton;

	private void Start()
	{
		closeButton.onClick.AddListener(() => Destroy(gameObject));
	}
	public void SetData(string title, string contentAr, string contentEn)
	{
		titleText.text = title;

		Debug.Log($"Setting content for {LocalizationSettings.SelectedLocale} locale");
		// Set content based on the selected locale
		if (LocalizationSettings.SelectedLocale.Identifier.Code == "ar")
		{
			Debug.Log("Setting Arabic content");
			contentText.text = contentAr;

			contentText.text = ArabicSupporter.ArabicSupport.Fix(contentText.text);
		}
		else
		{
			Debug.Log("Setting English content");
			contentText.text = contentEn;
		}

	}
}