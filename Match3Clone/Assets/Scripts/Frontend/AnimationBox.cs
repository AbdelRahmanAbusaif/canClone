using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using GameVanilla.Core;

public class AnimationBox : Popup
{
    [SerializeField] private GameObject boxForAnimation;
    [SerializeField] private Image blackScreen;
    
    private void OnEnable()
    {
        DOTween.Init();

        DOTween.Sequence()
            .Append(boxForAnimation.transform.DOMoveY(0, 0.5f))
            .AppendInterval(1f)
            .Append(blackScreen.DOFade(0.5f, 0.5f))
            .OnComplete(() =>
            {
                blackScreen.raycastTarget = false;
            });
    }
    public void OnClose() {
        DOTween.Sequence()
            .Append(blackScreen.DOFade(0, 0.5f))
            .Append(boxForAnimation.transform.DOScale(0,0.3f))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
