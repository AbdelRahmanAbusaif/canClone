using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LoopingScroll : MonoBehaviour
{
    public RectTransform content;
    public float MaxThreshold = 7000f;
    public float resetAfterMax = -7800f;

    public float MinThreshold = 7000f;
    public float resetAfterMin = -7800f;

    public int loop;
    public bool newLoop = false;

    private bool isResetting = false;

    private async void Start()
    {
        var playerProfile = await SaveData.LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        var nextLevel = playerProfile.Level;
        loop = nextLevel / 200;
    }

    void Update()
    {
        Debug.Log(loop);
        if (isResetting) return;

        if (content.anchoredPosition.y > MaxThreshold && loop > 0)
        {
            StartCoroutine(ResetScrollAfterDelay(resetAfterMax, -1));
        }
        else if (content.anchoredPosition.y < MinThreshold)
        {
            StartCoroutine(ResetScrollAfterDelay(resetAfterMin, 1));
        }
    }

    IEnumerator ResetScrollAfterDelay(float newYPosition, int loopDelta)
    {
        isResetting = true;

        yield return new WaitForSeconds(3f); // Wait 3 seconds

        Vector2 pos = content.anchoredPosition;
        pos.y = newYPosition;
        content.anchoredPosition = pos;

        loop += loopDelta;
        newLoop = true;

        isResetting = false;
    }
}
