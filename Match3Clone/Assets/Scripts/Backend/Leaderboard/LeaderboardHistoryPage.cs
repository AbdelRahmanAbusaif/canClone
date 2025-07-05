using ArabicSupporter;
using TMPro;
using UnityEngine;
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
		if(ArabicSupport.IsArabicString(title))
		{
			titleText.text = ArabicSupporter.ArabicSupport.Fix(title);
		}
		else
		{
			titleText.text = title;
		}

		if (PlayerPrefs.GetInt("Language") == 0)
		{
			contentText.text = contentAr;

			contentText.text = ArabicSupporter.ArabicSupport.Fix(contentText.text);
		}
		else
		{
			contentText.text = contentEn;
		}

	}
}