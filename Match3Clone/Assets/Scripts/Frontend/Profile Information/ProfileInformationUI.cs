using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class ProfileInformationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerId;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI phoneNumberText;

    [SerializeField] private Image avatarImage;

    [SerializeField] private Button linkMyAccountButton;
    [SerializeField] private Button signOutButton;

    private CloudSaveManager cloudSaveManager;

    async void Start()
    {
        cloudSaveManager = FindAnyObjectByType<CloudSaveManager>().GetComponent<CloudSaveManager>();
        
        var playerProfile = await cloudSaveManager.LoadDataAsync<PlayerProfile>("PlayerProfile");

        try
        {
            playerId.text = playerProfile.PlayerId;
            playerNameText.text = playerProfile.PlayerName;
            emailText.text = playerProfile.Email;
            phoneNumberText.text = playerProfile.PhoneNumber;            
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }


        if(PlayerPrefs.GetInt("IsAnonymous") == 1)
        {
            linkMyAccountButton.gameObject.SetActive(true);
        }
        else
        {
            linkMyAccountButton.gameObject.SetActive(false);
            cloudSaveManager.LoadImageAsync("PlayerProfileImage", avatarImage);
        }

        LoginController.OnSignedOutSuccess += OnSignedOutSuccess;

        signOutButton.onClick.AddListener(OnSignOutButtonClicked);
    }

    private void OnSignOutButtonClicked()
    {
        if(AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();

            AuthenticationService.Instance.ClearSessionToken();
        }
        else
        {
            Debug.Log("User is not signed in");
        }
    }


    private void OnSignedOutSuccess()
    {
        PlayerPrefs.SetInt("IsAnonymous", 1);
        PlayerPrefs.Save();
    }

    // Update is called once per frame

    void Update()
    {
        
    }
}
