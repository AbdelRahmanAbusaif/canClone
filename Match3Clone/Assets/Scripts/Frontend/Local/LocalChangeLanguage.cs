using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalChangeLanguage : MonoBehaviour
{
    [SerializeField] private Button arabicButton;
    [SerializeField] private Button englishButton;

    [SerializeField] private Image arabicImage;
    [SerializeField] private Image englishImage;

    [SerializeField] private Sprite inactiveImage;
    [SerializeField] private Sprite activeImage;


    private void Awake() 
    {
        LocalManager.Instance.OnLocalSelected += OnLocalSelected;

        CheckSelectedLocale();

        arabicButton.onClick.AddListener(() => LocalManager.Instance.SelectLocalByIndex(0));
        englishButton.onClick.AddListener(() => LocalManager.Instance.SelectLocalByIndex(1));
    }

    private void CheckSelectedLocale()
    {
        if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
        {
            arabicImage.sprite = activeImage;
            englishImage.sprite = inactiveImage;
        }
        else if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1])
        {
            englishImage.sprite = activeImage;
            arabicImage.sprite = inactiveImage;
        }
    }


    private void OnLocalSelected(int obj)
    {
        Debug.Log("Selected Locale: " + LocalizationSettings.SelectedLocale);
        CheckSelectedLocale();
    }
    private void OnDestroy() 
    {
        LocalManager.Instance.OnLocalSelected -= OnLocalSelected;    
    }

}
