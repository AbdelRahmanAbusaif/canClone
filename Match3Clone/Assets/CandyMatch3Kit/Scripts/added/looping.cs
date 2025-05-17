using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LoopingScroll : MonoBehaviour
{
    public RectTransform content;
    public GameObject page;
    Animator trans;
    public float MaxThreshold = 7000f;
    public float resetAfterMax = -7800f;

    public float MinThreshold = 7000f;
    public float resetAfterMin = -7800f;

    public int loop;
    public int originLoop;

    public bool newLoop = false;

    private bool isResetting = false;

    private async void Start()
    {
        
        var playerProfile = await SaveData.LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        var nextLevel = playerProfile.Level;
       
        loop = nextLevel / 200;
        trans = page.GetComponent<Animator>();
        originLoop=loop;
        
    }

    void Update()
    {
        Debug.Log(originLoop);
        if (isResetting) return;

        if (content.anchoredPosition.y > MaxThreshold && loop > 0)
        {
            trans.Play("transition");
            StartCoroutine(ResetScrollAfterDelay(resetAfterMax, -1));
            
        }
        else if (content.anchoredPosition.y < MinThreshold)
        { 
            trans.Play("transition");
            StartCoroutine(ResetScrollAfterDelay(resetAfterMin, 1));
           
        }
        if (originLoop==loop)
        {
            GameObject target = GameObject.Find("LevelMapAvatar(Clone)");

            if (target != null)
            {
               target.SetActive(true);
            }
            
        }
        else
        {

            GameObject target = GameObject.Find("LevelMapAvatar(Clone)");

            if (target != null)
            {
                target.SetActive(false);
            }
        }
    }

    IEnumerator ResetScrollAfterDelay(float newYPosition, int loopDelta)
    {
        isResetting = true;

        yield return new WaitForSeconds(1f); // Wait 3 seconds

        Vector2 pos = content.anchoredPosition;
        pos.y = newYPosition;
        content.anchoredPosition = pos;

        loop += loopDelta;
        newLoop = true;

        isResetting = false;
    }
}
