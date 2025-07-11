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

	[SerializeField] private ArabicFixerTMPRO ArabicFixerTMPRO; // Assuming you have a script to handle Arabic text fixing

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

			ArabicFixerTMPRO.enabled = true; // Enable Arabic fixer script if needed

			// Get the current width of the text box
			float currentWidth = contentText.rectTransform.rect.width;

			// Get preferred size using current width to calculate proper wrapping
			Vector2 preferredSize = contentText.GetPreferredValues(contentAr, currentWidth, 0f);

			// Apply new height only
			contentText.rectTransform.sizeDelta = new Vector2(contentText.rectTransform.sizeDelta.x, preferredSize.y );
			//contentText.text = ArabicSupporter.ArabicSupport.Fix(contentText.text);
		}
		else
		{
			Debug.Log("Setting English content");
			contentText.text = contentEn;
			
			ArabicFixerTMPRO.enabled = false; // Disable Arabic fixer script for English content

			// Get the current width of the text box
			float currentWidth = contentText.rectTransform.rect.width;

			// Get preferred size using current width to calculate proper wrapping
			Vector2 preferredSize = contentText.GetPreferredValues(contentEn, currentWidth, 0f);

			// Apply new height only
			contentText.rectTransform.sizeDelta = new Vector2(contentText.rectTransform.sizeDelta.x, preferredSize.y);
		}

	}
}