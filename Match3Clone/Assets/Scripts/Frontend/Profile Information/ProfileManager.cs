using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using System;
using Unity.Services.Authentication;

using SaveData;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    public static Action OnUpdateSuccess;

    [SerializeField] private InputProfilePanel phonePanel;
    [SerializeField] private Button updateButton;
    [SerializeField] private Button uploadImageButton;

    [SerializeField] private Image profileImage;
    [SerializeField] private Texture2D avatarProfileImageTexture;
    [SerializeField] private Texture2D coverProfileImageTexture;
    [SerializeField] private Texture2D borderProfileImageTexture;

    [SerializeField] private GameObject uploadImagePanel;
    
    private string filepath = "";
    private string phoneNumber;

    private int currentPanelIndex = 0;

    private PlayerProfile playerProfile;
    private async void OnEnable() 
    {
        // Here
        updateButton.onClick.AddListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.AddListener(OnUploadImageButtonClicked);

        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
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
        string pattern = @"^07[7-9][0-9]{7}$";
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
        try
        {
            playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error get player profile: {e.Message}");
        }
        
        phoneNumber = phonePanel.inputField.text.Trim();

        if(!ValidatePhoneNumber(phoneNumber))
        {
            Debug.LogError("Invalid phone number format. Please enter a valid phone number.");
            return;
        }
        playerProfile.PhoneNumber = phoneNumber;
        Debug.Log($"Player phone number: {playerProfile.PhoneNumber}");

        SaveImageInCloud();
        Debug.Log($"Player profile image: {playerProfile.PlayerImageUrl}");

        await CloudSaveManager.Instance.SavePublicDataAsync("PlayerProfile", playerProfile);
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerProfile.PlayerName.Replace("+", "").Replace(" ", ""));

        Debug.Log("Player profile updated successfully");

        if(playerProfile.PhoneNumber == null)
        {
            playerProfile.PhoneNumber = phoneNumber;
            Debug.LogError("Phone number is null, please enter a valid phone number");
            return;
        }
        OnUpdateSuccess?.Invoke();
    }
    private async void SaveImageInCloud()
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        if (playerProfile == null)
        {
            Debug.LogError("Player profile is null.");
            return;
        }
        if (string.IsNullOrEmpty(playerProfile.PlayerImageUrl))
        {
            Debug.LogError("Player profile URL is null or empty.");
            return;
        }
        if (playerProfile == null)
        {
            Debug.LogError("Player profile is null.");
            return;
        }

        StartCoroutine(DownloadImage(playerProfile.PlayerImageUrl));
    }

    private async void UpdloadedInCloud(Sprite playerImage)
    {
        await CloudSaveManager.Instance.SaveImageAsync("PlayerProfileImage", playerImage.texture);
        await CloudSaveManager.Instance.SaveImageAsync("PlayerProfileBorderImage", borderProfileImageTexture);
        await CloudSaveManager.Instance.SaveImageAsync("PlayerProfileCoverImage", coverProfileImageTexture);

        await CloudSaveManager.Instance.SaveDataAsyncString<string>("PlayerImageUploaded", GetImageBase64(playerImage.texture));
        await CloudSaveManager.Instance.SaveDataAsyncString<string>("PlayerBorderImageUploaded", GetImageBase64(borderProfileImageTexture));
        await CloudSaveManager.Instance.SaveDataAsyncString<string>("PlayerCoverImageUploaded", GetImageBase64(coverProfileImageTexture));

        playerProfile.DataPublicProfileImage = "PlayerImageUploaded";
        ConsumableItem item = new ConsumableItem()
        {
            Id = "PlayerImageUploaded",
            ConsumableName = "PlayerImageUploaded",
            DatePurchased = DateTime.MinValue.ToString(),
            DateExpired = DateTime.MaxValue.ToString()
        };

        var containerProfileAvatarImages = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileAvatarImages");
        containerProfileAvatarImages.Add(item);
        await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileAvatarImages", containerProfileAvatarImages);
    }

    private string GetImageBase64(Texture2D texture)
    {
        Texture2D resizeTexture = ImageUtility.ResizeTexture(texture, 256, 256);
        byte[] bytes = ImageUtility.CompressTexture(resizeTexture, quality: 50);
        
        return Convert.ToBase64String(bytes);
    }
    private IEnumerator DownloadImage(string url)
    {
        Debug.Log("Downloading image from Facebook: " + url);

        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error when download Image from facebook :" + request.error);
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image downloaded successfully from Facebook.");

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite playerImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            UpdloadedInCloud(playerImage);
        }
        else
        {
            Debug.LogError("Unknown error when downloading image from Facebook.");
        }
    }
    private void OnDisable() 
    {
        updateButton.onClick.RemoveListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.RemoveListener(OnUploadImageButtonClicked);
    }
}
