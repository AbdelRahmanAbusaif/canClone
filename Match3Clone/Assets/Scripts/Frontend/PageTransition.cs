using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PageTransition : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages; 
    [SerializeField] private List<Image> imagesBorder;

    [SerializeField] private Sprite ActiveBorder;
    [SerializeField] private Sprite InactiveBorder;

    [SerializeField] private GameObject rightRespawn;  
    [SerializeField] private GameObject leftRespawn; 
    [SerializeField] private float transitionDuration = 0.1f;

    //because first page index is 1(Story page)
    private int currentPageIndex = 1;
    private bool isTransitioning = false;

    public void TransitionToPage(int targetPageIndex)
    {
        if (targetPageIndex < 0 || targetPageIndex >= pages.Count || isTransitioning || currentPageIndex == targetPageIndex)
        {
            return;
        }

        isTransitioning = true;
        
        Vector3 targetPosition = targetPageIndex > currentPageIndex ? leftRespawn.transform.position : rightRespawn.transform.position;
        Vector3 entryPosition = targetPageIndex > currentPageIndex ? rightRespawn.transform.position : leftRespawn.transform.position;

        // Activate the target page at the correct entry position
        pages[targetPageIndex].transform.position = entryPosition;
        pages[targetPageIndex].SetActive(true);

        UpdateBorder(targetPageIndex);

        // Perform the transition animation
        DOTween.Sequence()
            .Append(pages[currentPageIndex].transform.DOMove(targetPosition, transitionDuration))
            .Join(pages[targetPageIndex].transform.DOMove(Vector3.zero, transitionDuration))
            .OnComplete(() =>
            {
                FinalizeTransition(targetPageIndex);
            });
    }

    private void UpdateBorder(int targetPageIndex)
    {
        for (int i = 0; i < imagesBorder.Count; i++)
        {
            imagesBorder[i].sprite = InactiveBorder;
        }
        imagesBorder[targetPageIndex].sprite = ActiveBorder;
    }

    private void FinalizeTransition(int targetPageIndex)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == targetPageIndex);
        }

        currentPageIndex = targetPageIndex;
        isTransitioning = false;
    }
}
