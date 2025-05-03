using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

using SaveData;
using NUnit.Framework;

public class ProfileInformationUI : MonoBehaviour
{
    public static Action OnSignOutTransition;
    [SerializeField] private TextMeshProUGUI playerId;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI phoneNumberText;

    [SerializeField] private Image avatarImage;

    [SerializeField] private Button linkMyAccountButton;
    [SerializeField] private Button signOutButton;

    [SerializeField] private LoginController loginController;

    async void Start()
    {
        loginController = FindAnyObjectByType<LoginController>().GetComponent<LoginController>();
        
        var playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        
        Debug.Log("Player Profile: " + playerProfile.PlayerName);
        try
        {
            playerId.text = playerProfile.PlayerId;
            playerNameText.text = playerProfile.PlayerName;
            emailText.text = playerProfile.Email;
            phoneNumberText.text = playerProfile.PhoneNumber;
            playerLevelText.text = playerProfile.Level.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        signOutButton.onClick.AddListener(OnSignOutButtonClicked);
    }

    private void OnSignOutButtonClicked()
    {
        if(AuthenticationService.Instance.IsSignedIn)
        {
            loginController.InitSignOut();
            PlayerPrefs.DeleteAll();
        }
        else
        {
            Debug.Log("User is not signed in");
        }
    }
    private bool ValidateEmail(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
