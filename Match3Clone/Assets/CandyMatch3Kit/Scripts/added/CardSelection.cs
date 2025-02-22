using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardSelection : MonoBehaviour
{
    public List<RectTransform> cards; 
    public float expandedSize = 300f; // 
    public float normalSize=200;
    public float high = 250;
    public float animationSpeed = 5f; // 

    private RectTransform selectedCard = null;

    private void Start()
    {
        SelectCard(cards[1]);
        
    }
    public void SelectCard(RectTransform card)
    {

        selectedCard = card;
        foreach (RectTransform c in cards)
        {
            float targetSize = (c == selectedCard) ? expandedSize : normalSize;
            StartCoroutine(AnimateSize(c, targetSize));
        }
    }

    private IEnumerator<YieldInstruction> AnimateSize(RectTransform card, float targetSize)
    {
        Vector2 originalSize = card.sizeDelta;
        Vector2 target = new Vector2(targetSize,high );
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * animationSpeed;
            card.sizeDelta = Vector2.Lerp(originalSize, target, time);
            yield return null;
        }

        card.sizeDelta = target;
    }
}