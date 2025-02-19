using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalManager : MonoBehaviour
{
    public static LocalManager Instance;

    public Action<int> OnLocalSelected;
    private bool active = false;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start() 
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            StartCoroutine(SelectLocal(PlayerPrefs.GetInt("Language")));
        }
        else
        {
            StartCoroutine(SelectLocal(0));
        }
    }
    public void SelectLocalByIndex(int index)
    {
        if (active)
            return;
        StartCoroutine(SelectLocal(index));
    }
    IEnumerator SelectLocal(int index)
    {
        active = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        Debug.Log("Selected Locale: " + LocalizationSettings.SelectedLocale);
        OnLocalSelected?.Invoke(index);
        active = false;

        PlayerPrefs.SetInt("Language", index);
        PlayerPrefs.Save();
    }
}
