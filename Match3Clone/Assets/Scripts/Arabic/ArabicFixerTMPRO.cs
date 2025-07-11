using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using ArabicSupporter;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ArabicFixerTMPRO : MonoBehaviour
{
	[TextArea]
	public string fixedText;
	public bool ShowTashkeel;
	public bool UseHinduNumbers;

	private TextMeshProUGUI tmpTextComponent;
	private RectTransform rectTransform;

	// State for detecting changes
	private string oldText;
	private int oldFontSize;
	private Vector2 oldDeltaSize;
	private bool oldEnabled;
	private List<RectTransform> parentRects = new List<RectTransform>();
	private Vector2 oldScreenSize;

	private bool isInitialized;

	private void Awake()
	{
		tmpTextComponent = GetComponent<TextMeshProUGUI>();
		rectTransform = GetComponent<RectTransform>();
		GetRectTransformParents(parentRects);
		isInitialized = false;
	}

	private void Start()
	{
		fixedText = tmpTextComponent.text;
		oldScreenSize = new Vector2(Screen.width, Screen.height);
		isInitialized = true;
		ApplyFix();
	}

	private void GetRectTransformParents(List<RectTransform> rects)
	{
		rects.Clear();
		for (Transform t = transform.parent; t != null; t = t.parent)
		{
			if (t is RectTransform rt) rects.Add(rt);
		}
	}

	private bool HaveParentsChanged()
	{
		bool changed = false;
		foreach (var rt in parentRects)
		{
			changed |= rt.hasChanged;
			rt.hasChanged = false;
		}
		return changed;
	}

	private void Update()
	{
		if (!isInitialized) return;

		// Check if any relevant property changed
		if (oldText == fixedText &&
			oldFontSize == (int)tmpTextComponent.fontSize &&
			oldDeltaSize == rectTransform.sizeDelta &&
			oldEnabled == tmpTextComponent.enabled &&
			oldScreenSize.x == Screen.width && oldScreenSize.y == Screen.height &&
			!HaveParentsChanged())
		{
			return;
		}

		ApplyFix();

		// Update state
		oldText = fixedText;
		oldFontSize = (int)tmpTextComponent.fontSize;
		oldDeltaSize = rectTransform.sizeDelta;
		oldEnabled = tmpTextComponent.enabled;
		oldScreenSize = new Vector2(Screen.width, Screen.height);
	}

	private void ApplyFix()
	{
		if (string.IsNullOrEmpty(fixedText)) return;

		// Fix Arabic shaping, remove carriage returns
		string rtlText = ArabicSupport.Fix(fixedText, ShowTashkeel, UseHinduNumbers)
			.Replace("\r", string.Empty);

		// Split into paragraphs, preserving empty entries
		string[] paragraphs = rtlText.Split(new[] { '\n' }, System.StringSplitOptions.None);
		List<string> finalLines = new List<string>();

		foreach (var para in paragraphs)
		{
			// Preserve empty paragraphs (blank lines)
			if (string.IsNullOrEmpty(para))
			{
				finalLines.Add(string.Empty);
				continue;
			}

			// Reverse words for layout pass
			string[] words = para.Split(' ');
			System.Array.Reverse(words);
			string reversedPara = string.Join(" ", words);

			// Set and update to get line info
			tmpTextComponent.text = reversedPara;
			Canvas.ForceUpdateCanvases();

			int lineCount = tmpTextComponent.textInfo.lineCount;
			for (int i = 0; i < lineCount; i++)
			{
				int start = tmpTextComponent.textInfo.lineInfo[i].firstCharacterIndex;
				int end = (i == lineCount - 1)
					? tmpTextComponent.text.Length
					: tmpTextComponent.textInfo.lineInfo[i + 1].firstCharacterIndex;

				int length = end - start;
				string lineText = tmpTextComponent.text.Substring(start, length).Trim();

				// Reverse words back for correct Arabic order per line
				string[] lineWords = lineText.Split(' ');
				System.Array.Reverse(lineWords);
				finalLines.Add(string.Join(" ", lineWords));
			}
		}

		// Join all lines with newline
		tmpTextComponent.text = string.Join("\n", finalLines);
	}
}
