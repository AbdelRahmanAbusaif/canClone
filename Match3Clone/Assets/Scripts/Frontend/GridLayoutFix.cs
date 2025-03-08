using UnityEngine;
using UnityEngine.UI;

public class GridLayoutFix : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform canvasScaler;
    [SerializeField] private FixType fixType;
    public enum FixType
    {
        Width,
        Height
    }
    void Start()
    {
        canvasScaler = GameObject.Find("Canvas").GetComponent<RectTransform>();
        FixSize();
    }
    void FixSize()
    {
        if(fixType == FixType.Width)
        {
            rectTransform.sizeDelta = new Vector2(canvasScaler.sizeDelta.x, rectTransform.sizeDelta.y);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, canvasScaler.sizeDelta.y);
        }
    }
}
