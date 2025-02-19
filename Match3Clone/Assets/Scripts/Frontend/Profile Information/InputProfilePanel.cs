using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputProfilePanel : MonoBehaviour
{
    public string Id; // Panel identifier
    public Action<bool> OnNextButtonClickedAction; // Callback for success or failure

    [SerializeField] public TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Button nextButton;

    public Func<string, bool> ValidateInput; // Custom validation logic for this panel

    private void OnEnable()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }

    private void OnNextButtonClicked()
    {
        if (ValidateInput != null && ValidateInput(inputField.text))
        {
            warningText.gameObject.SetActive(false);
            OnNextButtonClickedAction?.Invoke(true);
        }
        else
        {
            warningText.gameObject.SetActive(true);
            OnNextButtonClickedAction?.Invoke(false);
        }
    }
}