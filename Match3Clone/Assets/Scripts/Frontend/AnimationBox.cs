using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using GameVanilla.Core;

public class AnimationBox : MonoBehaviour
{
    [SerializeField] private GameObject boxForAnimation;
    [SerializeField] private Image blackScreen;
    
    private void OnEnable()
    {
        DOTween.Init();

        DOTween.Sequence()
            .Append(boxForAnimation.transform.DOLocalMoveY(0, 0.5f))
            .Join(blackScreen.DOFade(0.5f, 0.5f))
            .OnComplete(() =>
            {
                blackScreen.raycastTarget = false;
            });
    }
    public void OnClose() {
        DOTween.Sequence()
            .Append(boxForAnimation.transform.DOLocalMoveY(5000, 0.5f))
            .Join(blackScreen.DOFade(0, 0.5f))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
    private void OnDisable() {
        OnClose();
    }
}
