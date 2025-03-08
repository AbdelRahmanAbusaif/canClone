using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using GameVanilla.Core;

public class AnimationBox : MonoBehaviour
{
    [SerializeField] private GameObject boxForAnimation;
    [SerializeField] private Image blackScreen;
    
    [SerializeField] private AnimationType animationType = AnimationType.Move;
    [SerializeField] private float startPosY = 5000f;
    [SerializeField] private float endPosY = 0f;
    [SerializeField] private float duration = 0.5f;
    private void OnEnable()
    {
        DOTween.Init();

        Debug.Log("AnimationBox OnEnable");
        switch(animationType)
        {
            case AnimationType.Move:
                DOTween.Sequence()
                .Append(boxForAnimation.transform.DOLocalMoveY(endPosY, duration))
                .Append(blackScreen.DOFade(0.5f, 0.5f))
                .OnComplete(() =>
                {
                    blackScreen.raycastTarget = false;
                });
                break;
            case AnimationType.Fade:
                DOTween.Sequence()
                .Append(boxForAnimation.GetComponent<CanvasGroup>().DOFade(1, duration))
                .Append(blackScreen.DOFade(0.5f, 0.5f))
                .OnComplete(() =>
                {
                    blackScreen.raycastTarget = false;
                });
                break;
            case AnimationType.Scale:
                DOTween.Sequence()
                .Append(boxForAnimation.transform.DOScale(Vector3.one, duration))
                .Append(blackScreen.DOFade(0.5f, 0.5f))
                .OnComplete(() =>
                {
                    blackScreen.raycastTarget = false;
                });
                break;
        }
        
    }
    public void OnClose() {
        
        switch(animationType)
        {
            case AnimationType.Move:
                DOTween.Sequence()
                .Append(boxForAnimation.transform.DOLocalMoveY(startPosY, duration))
                .Join(blackScreen.DOFade(0, 0.5f))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
                break;
            case AnimationType.Fade:
                DOTween.Sequence()
                .Append(boxForAnimation.GetComponent<CanvasGroup>().DOFade(0, duration))
                .Join(blackScreen.DOFade(0, 0.5f))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
                break;
            case AnimationType.Scale:
                DOTween.Sequence()
                .Append(boxForAnimation.transform.DOScale(Vector3.zero, duration))
                .Join(blackScreen.DOFade(0, 0.5f))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
                break;
        }
    }
    private void OnDisable() 
    {
        OnClose();
    }
    private void OnDestroy() 
    {
        DOTween.Clear();
    }

    public enum AnimationType
    {
        Move,
        Fade,
        Scale
    }
}
