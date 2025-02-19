using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalSelectLanguage : MonoBehaviour
{
    [SerializeField] private Button languageButton;
    [SerializeField] private Image arabicImage;
    [SerializeField] private Image englishImage;
    private void Start()
    {
        languageButton.onClick.AddListener(OnLanguageButtonClicked);
        LocalManager.Instance.OnLocalSelected += OnLocalSelected;
    }

    private void OnLocalSelected(int obj)
    {
        Debug.Log("Selected Locale: " + LocalizationSettings.SelectedLocale);
        if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
        {
            Debug.Log("Arabic");
            arabicImage.gameObject.SetActive(true);
            englishImage.gameObject.SetActive(false);
        }
        else if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1])
        {
            Debug.Log("English");
            englishImage.gameObject.SetActive(true);
            arabicImage.gameObject.SetActive(false);
        }
    }

    private void OnLanguageButtonClicked()
    {
        if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
        {
            LocalManager.Instance.SelectLocalByIndex(1);
        }
        else if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1])
        {
            LocalManager.Instance.SelectLocalByIndex(0);
        }
    }
    private void OnDisable()
    {
        languageButton.onClick.RemoveListener(OnLanguageButtonClicked);
        LocalManager.Instance.OnLocalSelected -= OnLocalSelected;
    }
}