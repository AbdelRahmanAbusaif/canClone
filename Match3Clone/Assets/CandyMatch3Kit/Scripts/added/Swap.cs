using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swap : MonoBehaviour
{
    public Scrollbar scrollBar;
    private float scroll_pos = 0;
    private float[] pos;

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize pos array based on the number of children
        pos = new float[transform.childCount];

       // Debug.Log($"Number of child elements: {transform.childCount}");
    }

    // Update is called once per frame
    void Update()
    {
        float distance = 1f / (pos.Length - 1f);

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollBar.value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], 0.1f);
                }
            }
        }

        // Adjust the scale of the child elements based on their position
        
        for (int i = 0; i < pos.Length; i++)
        {
            RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
            if (child != null)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                  //  Debug.Log($"Scaling up child at index {i}");
                    child.localScale = Vector2.Lerp(child.localScale, new Vector2(1f, 1f), 0.3f);
                }
                else
                {
                   // Debug.Log($"Scaling down child at index {i}");
                    child.localScale = Vector2.Lerp(child.localScale, new Vector2(0.5f, 0.5f), 0.3f);
                }
            }
            else
            {
                Debug.LogError($"RectTransform component not found on child {i}");
            }
        
        }
        
    }
}
