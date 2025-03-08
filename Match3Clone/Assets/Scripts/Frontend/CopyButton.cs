using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button copyButton;
    void Start()
    {
        copyButton.onClick.AddListener(() =>
        {
            CopyText();
        });
    }

    private void CopyText()
    {
        GUIUtility.systemCopyBuffer = text.text;
    }

    private void OnDestroy()
    {
        copyButton.onClick.RemoveAllListeners();
    }
}
