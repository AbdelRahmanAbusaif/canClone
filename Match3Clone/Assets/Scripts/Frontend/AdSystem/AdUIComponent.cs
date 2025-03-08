using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdUIComponent : MonoBehaviour 
{
    public TextMeshProUGUI goalClicksText;
    public TextMeshProUGUI currentClicksText;
    public TextMeshProUGUI rewardText;
    public Button adButton;

    private void Start()
    {
        adButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log("Open Ad");
        OpenAd();
    }

    private void OpenAd()
    {
        Debug.Log("Ad Opened");
    }

    public void SetAdData(string goalClicks, string currentClicks, string reward)
    {
        goalClicksText.text = goalClicks;
        currentClicksText.text = currentClicks;
        rewardText.text = reward;
    }
}
