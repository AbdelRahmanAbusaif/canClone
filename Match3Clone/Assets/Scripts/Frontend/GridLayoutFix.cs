using UnityEngine;
using UnityEngine.UI;

public class GridLayoutFix : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private FixType fixType;
    public enum FixType
    {
        Width,
        Height
    }
    void Start()
    {
        FixSize();
    }
    void FixSize()
    {
        if(fixType == FixType.Width)
        {
            rectTransform.sizeDelta = new Vector2(canvasScaler.referenceResolution.x, rectTransform.sizeDelta.y);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, canvasScaler.referenceResolution.y);
        }
    }
}
