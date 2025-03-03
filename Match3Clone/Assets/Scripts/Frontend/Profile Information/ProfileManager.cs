using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using System;
using Unity.Services.Authentication;

using SaveData;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProfileManager : MonoBehaviour
{
    public static Action OnUpdateSuccess;

    [SerializeField] private List<InputProfilePanel> panels;
    [SerializeField] private Button updateButton;
    [SerializeField] private Button uploadImageButton;

    [SerializeField] private Image profileImage;
    [SerializeField] private Texture2D texture;

    [SerializeField] private GameObject uploadImagePanel;
    
    private CloudSaveManager cloudSaveManager;

    private string filepath = "";
    private string playerName;
    private string email;
    private string phoneNumber;

    private int currentPanelIndex = 0;


    private void OnEnable() {
        updateButton.onClick.AddListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.AddListener(OnUploadImageButtonClicked);

        cloudSaveManager = FindAnyObjectByType<CloudSaveManager>().GetComponent<CloudSaveManager>();

        foreach (var panel in panels)
        {
            panel.gameObject.SetActive(false); // Hide all panels initially
        }

        if (panels.Count > 0)
        {
            ActivatePanel(0); // Start with the first panel
        }
    }

    private void ActivatePanel(int index)
    {
        if (index < 0 || index >= panels.Count)
        {
            Debug.LogWarning("Invalid panel index.");
            return;
        }

        // Deactivate all panels
        foreach (var panel in panels)
        {
            panel.gameObject.SetActive(false);
        }

        // Activate the current panel
        var currentPanel = panels[index];
        currentPanel.gameObject.SetActive(true);

        // Set up the panel-specific logic
        switch (currentPanel.Id)
        {
            case "Username":
                currentPanel.ValidateInput = ValidateUsername;
                playerName = currentPanel.inputField.text;
                Debug.Log(playerName);
                break;
            case "Email":
                currentPanel.ValidateInput = ValidateEmail;
                email = currentPanel.inputField.text;
                Debug.Log(email);
                break;
            case "Phone":
                currentPanel.ValidateInput = ValidatePhoneNumber;
                phoneNumber = currentPanel.inputField.text;
                Debug.Log(phoneNumber);
                break;
        }

        currentPanel.OnNextButtonClickedAction = success =>
        {
            if (success)
            {
                currentPanelIndex++;
                if (currentPanelIndex < panels.Count)
                {
                    Debug.Log("Moving to the next panel...");

                    
                    ActivatePanel(currentPanelIndex);
                }
                else
                {
                    uploadImagePanel.SetActive(true);
                    updateButton.gameObject.SetActive(true);

                    currentPanel.gameObject.SetActive(false);
                    Debug.Log("Sign-up process completed!");
                }
                switch (currentPanel.Id)
                {
                    case "Username":
                        playerName = currentPanel.inputField.text;
                        Debug.Log(playerName);
                        break;
                    case "Email":
                        email = currentPanel.inputField.text;
                        Debug.Log(email);
                        break;
                    case "Phone":
                        phoneNumber = currentPanel.inputField.text;
                        Debug.Log(phoneNumber);
                        break;
                }
            }
            else
            {
                Debug.Log("Validation failed. Please check your data.");
            }
        };
    }
    private bool ValidateUsername(string input)
    {
        return !string.IsNullOrEmpty(input) && input.Length >= 3;
    }

    private bool ValidateEmail(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool ValidatePhoneNumber(string input)
    {
        string pattern = @"^07[8-9][0-9]{7}$";
        return Regex.IsMatch(input, pattern);
    }

    private void OnUploadImageButtonClicked()
    {
        // Here will be the code for uploading the player profile image
        string[] FileType = new string []{"image/*"};

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                Debug.Log("Picked file with path " + path);
                // LoadImage(profileImage, path);

                if(filepath != path)
                {
                    filepath = path;
                    LoadImage(profileImage, path);
                }
                else
                {
                    Debug.Log("File already exists");
                    return;
                }
            }
            else
            {
                Debug.Log("Error picking file");
            } 
        },  FileType);

        LoadImage(profileImage, filepath);
    }

    private void LoadImage(Image profileImage, string path)
    {
        StartCoroutine(LoadImageCoroutine(profileImage, path));
    }

    private IEnumerator LoadImageCoroutine(Image profileImage, string path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            profileImage.sprite = Sprite.Create(((DownloadHandlerTexture)www.downloadHandler).texture, new Rect(0, 0, ((DownloadHandlerTexture)www.downloadHandler).texture.width, ((DownloadHandlerTexture)www.downloadHandler).texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    private async void OnUpdateButtonClicked()
    {
        // // Here will be the code for updating the player profile
        // if(Validation.IsValidName(playerName))
        // {
        //     Debug.Log("Player name is valid");
        // }
        // else
        // {
        //     Debug.Log("Player name is invalid");
        //     playerNameWarning.gameObject.SetActive(true);

        //     return;
        // }

        // if(Validation.IsValidEmail(email))
        // {
        //     Debug.Log("Email is valid");
        // }
        // else
        // {
        //     Debug.Log("Email is invalid");

        //     emailWarning.gameObject.SetActive(true);
        //     return;
        // }

        // if(Validation.IsValidPhoneNumber(phoneNumber))
        // {
        //     Debug.Log("Phone number is valid");
        // }
        // else
        // {
        //     Debug.Log("Phone number is invalid");

        //     phoneNumberWarning.gameObject.SetActive(true);
        //     return;
        // }


        var playerProfile = new PlayerProfile
        {
            PlayerId = AuthenticationService.Instance.PlayerId,
            PlayerName = playerName,
            Email = email,
            PhoneNumber = phoneNumber,
            DataPublicProfileBorder = "",
            DataPublicProfileImage = "",
            Level = 1,
            LastHeartTime = "0",
            IsAcceptedTerms = true,
            DailyBonus = new DailyBonus()
            {
                DateLastPlayed = "0",
                DailyBonusDayKey = "0"
            },
            SpinWheel = new SpinWheel()
            {
                DateLastSpin = "0",
                DailySpinDayKey = "0"
            },
            LevelsComplete = new List<LevelComplete>()
            {
               
            },
            AdManager = new(),
            ContainerProfileAvatarImages = new(),
            ContainerProfileBorders = new(),
            ContainerProfileCoverImages = new(),
            ContainerProfilePrimeSubscriptions = new()
        };  

        
        if(String.IsNullOrEmpty(filepath))
        {
            await SaveImageInCloud(playerProfile, texture);
            // playerProfile.ContainerProfileImages.Add("PlayerImageUploaded");
        }
        else
        {
            await SaveImageInCloud(playerProfile, profileImage.sprite.texture);
        }
        await cloudSaveManager.SaveDataAsync("PlayerProfile", playerProfile);
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

        Debug.Log("Player profile updated successfully");
        OnUpdateSuccess?.Invoke();
    }

    private async Task SaveImageInCloud(PlayerProfile playerProfile , Texture2D texture = null)
    {
        await cloudSaveManager.SaveImageAsync("PlayerProfileImage", texture);

        await cloudSaveManager.SaveDataAsyncString<string>("PlayerImageUploaded", GetImageBase64(texture));
        Debug.Log($"Player Profile Image: {GetImageBase64(texture)}");

        playerProfile.DataPublicProfileImage = "PlayerImageUploaded";
        ConsumableItem item = new ConsumableItem()
        {
            Id = "PlayerImageUploaded",
            ConsumableName = "PlayerImageUploaded",
            DatePurchased = DateTime.MinValue.ToString(),
            DateExpired = DateTime.MaxValue.ToString()
        };
        playerProfile.ContainerProfileAvatarImages.Add(item);
    }

    private string GetImageBase64(Texture2D texture)
    {
        Texture2D resizeTexture = ImageUtility.ResizeTexture(texture, 256, 256);
        byte[] bytes = ImageUtility.CompressTexture(resizeTexture, quality: 50);
        
        return Convert.ToBase64String(bytes);
    }
    private void OnDisable() 
    {
        updateButton.onClick.RemoveListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.RemoveListener(OnUploadImageButtonClicked);
    }
}
