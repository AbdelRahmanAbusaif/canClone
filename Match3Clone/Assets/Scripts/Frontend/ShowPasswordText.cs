using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowPasswordText : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private GameObject showPasswordButton;

    [SerializeField] private Sprite showPasswordIcon;
    [SerializeField] private Sprite hidePasswordIcon;

    [SerializeField] private Image showPasswordImage;

    private bool isPasswordVisible = false;
    private void Start()
    {
        showPasswordButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(TogglePasswordVisibility);
    }

    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;
        if (isPasswordVisible)
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;
            passwordField.ForceLabelUpdate();

            showPasswordImage.sprite = showPasswordIcon;
        }
        else
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
            passwordField.ForceLabelUpdate();

            showPasswordImage.sprite = hidePasswordIcon;
        }
    }
}
