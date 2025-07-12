using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCycler : MonoBehaviour
{
    public List<GameObject> leaderBoards; 
    public Image targetImage; 

    private List<Sprite> activeSprites = new List<Sprite>();
    private int currentIndex = 0;
    private Coroutine cycleCoroutine;

    void Start()
    {
        UpdateSprites();
    }

    void UpdateSprites()
    {
        activeSprites.Clear();

        foreach (var comp in leaderBoards)
        {
            if (comp.active)
            {
                Transform child = comp.transform.Find("CandyType");
                if (child != null)
                {
                    Image img = child.GetComponent<Image>();
                    if (img != null)
                    {
                        activeSprites.Add(img.sprite);
                    }
                }
            }
        }

        if (activeSprites.Count == 1)
        {
            targetImage.sprite = activeSprites[0];

           
            if (cycleCoroutine != null)
            {
                StopCoroutine(cycleCoroutine);
                cycleCoroutine = null;
            }
        }
        else if (activeSprites.Count > 1)
        {
           
            if (cycleCoroutine == null)
            {
                cycleCoroutine = StartCoroutine(CycleSprites());
            }
        }
        else
        {
            // No active sprites

            if (cycleCoroutine != null)
            {
                StopCoroutine(cycleCoroutine);
                cycleCoroutine = null;
            }
        }
    }

    IEnumerator CycleSprites()
    {
        while (true)
        {
            GetComponent<Animator>().Play(0);
            yield return new WaitForSeconds(1f);
            targetImage.sprite = activeSprites[currentIndex];
            currentIndex = (currentIndex + 1) % activeSprites.Count;
            yield return new WaitForSeconds(10f);
        }
    }

    void Update()
    {
        // Optional: check every frame if components enabled state changes
        UpdateSprites();
    }
}
