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
using Unity.IO.LowLevel.Unsafe;

public class ProfileManager : MonoBehaviour
{
    public static Action OnUpdateSuccess;

    [SerializeField] private List<InputProfilePanel> panels;
    [SerializeField] private Button updateButton;
    [SerializeField] private Button uploadImageButton;

    [SerializeField] private Image profileImage;
    [SerializeField] private Texture2D avatarProfileImageTexture;
    [SerializeField] private Texture2D coverProfileImageTexture;
    [SerializeField] private Texture2D borderProfileImageTexture;

    [SerializeField] private GameObject uploadImagePanel;
    
    private CloudSaveManager cloudSaveManager;

    private string filepath = "";
    private string playerName;
    private string email;
    private string phoneNumber;

    private int currentPanelIndex = 0;

    private PlayerProfile playerProfile;
    private async void OnEnable() 
    {
        // Here
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

        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
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
        // Debug.Log("Updating player profile...");
        playerProfile.PhoneNumber = phoneNumber;

        await SaveImageInCloud(playerProfile, playerProfile.PlayerImageUrl);
        await cloudSaveManager.SaveDataAsync("PlayerProfile", playerProfile);
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

        Debug.Log("Player profile updated successfully");
        OnUpdateSuccess?.Invoke();
    }

    private async Task SaveImageInCloud(PlayerProfile playerProfile , Texture2D texture = null)
    {
        await cloudSaveManager.SaveImageAsync("PlayerProfileImage", texture);
        await cloudSaveManager.SaveImageAsync("PlayerProfileBorder", borderProfileImageTexture);
        await cloudSaveManager.SaveImageAsync("PlayerCoverProfileImage", coverProfileImageTexture);

        await cloudSaveManager.SaveDataAsyncString<string>("PlayerImageUploaded", GetImageBase64(texture));
        await cloudSaveManager.SaveDataAsyncString<string>("PlayerBorderImageUploaded", GetImageBase64(borderProfileImageTexture));
        await cloudSaveManager.SaveDataAsyncString<string>("PlayerCoverImageUploaded", GetImageBase64(coverProfileImageTexture));
        
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
    private async Task SaveImageInCloud(PlayerProfile playerProfile , string playerProfileUrl)
    {
        StartCoroutine(DownloadImage(playerProfileUrl));

        Texture2D texture = profileImage.sprite.texture;

        await cloudSaveManager.SaveImageAsync("PlayerProfileImage", texture);
        await cloudSaveManager.SaveImageAsync("PlayerProfileBorder", borderProfileImageTexture);
        await cloudSaveManager.SaveImageAsync("PlayerCoverProfileImage", coverProfileImageTexture);

        await cloudSaveManager.SaveDataAsyncString<string>("PlayerImageUploaded", GetImageBase64(texture));
        await cloudSaveManager.SaveDataAsyncString<string>("PlayerBorderImageUploaded", GetImageBase64(borderProfileImageTexture));
        await cloudSaveManager.SaveDataAsyncString<string>("PlayerCoverImageUploaded", GetImageBase64(coverProfileImageTexture));
        
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
    private IEnumerator DownloadImage(string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
    private void OnDisable() 
    {
        updateButton.onClick.RemoveListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.RemoveListener(OnUploadImageButtonClicked);
    }
}
