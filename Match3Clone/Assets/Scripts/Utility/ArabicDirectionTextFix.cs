using UnityEngine;
using ArabicSupporter;
using TMPro;

public class ArabicDirectionTextFix : MonoBehaviour
{
    [SerializeField] private Vector2 textArabicPosition;
    [SerializeField] private Vector2 valueTextArabicPosition;
    [SerializeField] private Vector2 textEnglishPosition;
    [SerializeField] private Vector2 valueTextEnglishPosition;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI valueText;
    public void UpdateDirection(string text)
    {
        if (text != null)
        {
            if(ArabicSupport.IsArabicString(text))
            {
                Debug.Log("Arabic");
                this.text.rectTransform.localPosition = textArabicPosition;
                this.valueText.rectTransform.localPosition = valueTextArabicPosition;
            }
            else 
            {
                Debug.Log("English");
                this.text.rectTransform.localPosition = textEnglishPosition;
                this.valueText.rectTransform.localPosition = valueTextEnglishPosition;
            }
        }
    }
}
