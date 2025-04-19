using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixContentPosition : MonoBehaviour
{
    [SerializeField] private List<GameObject> contentList = new();
    [SerializeField] private RectTransform rectTransform;
    void Start()
    {
        Invoke(nameof(InitSize), 1f);
    }

    private void InitSize()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            contentList.Add(transform.GetChild(i).gameObject);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + 850f);

            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, -200000f, rectTransform.localPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
