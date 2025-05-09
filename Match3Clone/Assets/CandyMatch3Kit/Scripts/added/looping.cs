using UnityEngine;
using UnityEngine.UI;

public class LoopingScroll : MonoBehaviour
{
    public RectTransform content; // Reference to the Content object inside the ScrollView
    public float MaxThreshold = 7000f;
    public float resetAfterMax = -7800f;

    public float MinThreshold = 7000f;
    public float resetAfterMin = -7800f;

    public int loop;

    void start()
    {
        loop = 0;
    }

    void Update()
    {
        Debug.Log(loop);
        
        if (content.anchoredPosition.y > MaxThreshold && loop>0)
        {
            Vector2 pos = content.anchoredPosition;
            pos.y = resetAfterMax;
            content.anchoredPosition = pos;

            loop -=1;
           
        }
        if (content.anchoredPosition.y < MinThreshold)
        {
            Vector2 pos = content.anchoredPosition;
            pos.y = resetAfterMin;
            content.anchoredPosition = pos;

            loop += 1;
          
        }
    }
}
