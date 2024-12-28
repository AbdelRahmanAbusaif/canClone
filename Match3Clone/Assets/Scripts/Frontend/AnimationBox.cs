using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class AnimationBox : MonoBehaviour
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
    private void OnDisable() {
        blackScreen.DOFade(0, 0.5f);
        boxForAnimation.transform.DOMoveY(-1000, 0.5f);

        blackScreen.raycastTarget = true;
    }
}
