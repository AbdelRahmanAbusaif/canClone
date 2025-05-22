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
    GameObject target;

    public bool newLoop = false;

    private bool isResetting = false;

    PlayerProfile playerProfile;

    private async void Start()
    {
        trans = page.GetComponent<Animator>();
        playerProfile = await SaveData.LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        var nextLevel = playerProfile.Level;

       
       

       
        loop = nextLevel / 200;
        originLoop =loop;


    }

    void Update()
    {
        

        Debug.Log("Origin" + originLoop);
        Debug.Log("currnt" + loop);
        Debug.Log("next" + playerProfile.Level);


      

        if (target == null)
        {
              target = GameObject.Find("LevelMapAvatar(Clone)");
        }
       
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
        if (originLoop == loop)
        {
           

            if (target != null)
            {
                target.SetActive(true);
            }

        }
        else
        {

           

            if (target != null)
            {
                target.SetActive(false);
            }
        }
    }
}
