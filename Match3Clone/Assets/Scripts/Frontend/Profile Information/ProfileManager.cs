using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using System;
using Unity.Services.Authentication;
using System.Linq;

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
    [SerializeField] private Texture2D avatarProfileImageTexture;
    [SerializeField] private Texture2D coverProfileImageTexture;
    [SerializeField] private Texture2D borderProfileImageTexture;

    [SerializeField] private GameObject uploadImagePanel;
    [SerializeField] private GameObject loadingPanel;
    
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

        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");

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
        loadingPanel.SetActive(true);

        try
        {
            await ProcessProfileUpdateAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating profile: {e.Message}");
        }
        finally
        {
            // Ensure loading panel is always disabled and success event is invoked
            loadingPanel.SetActive(false);
            OnUpdateSuccess?.Invoke();
        }
    }

    private async Task ProcessProfileUpdateAsync()
    {
        phoneNumber = panels[currentPanelIndex].inputField.text;

        // Validate phone number first
        if (!ValidatePhoneNumber(phoneNumber))
        {
            Debug.LogError("Invalid phone number format. Please enter a valid phone number.");
            return;
        }

        // Load player profile
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        if (playerProfile == null)
        {
            Debug.LogError("Failed to load player profile.");
            return;
        }

        // Update profile data
        playerProfile.IsAcceptedTerms = true;
        playerProfile.PhoneNumber = phoneNumber;

        Debug.Log($"Player phone number: {playerProfile.PhoneNumber}");

        // Create tasks for parallel execution
        var tasks = new List<Task>();

        // Task 1: Update player name if exists
        if (!String.IsNullOrEmpty(playerProfile.PlayerName))
        {
            tasks.Add(UpdatePlayerNameAsync(playerProfile.PlayerName));
        }

        // Task 2: Save profile data
        tasks.Add(SaveProfileDataAsync());

        // Task 3: Save image in cloud
        tasks.Add(SaveImageInCloudAsync());

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        Debug.Log("Player profile updated successfully");
    }

    private async Task UpdatePlayerNameAsync(string playerName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName.Replace("+", "").Replace(" ", ""));
            playerProfile.PlayerName = AuthenticationService.Instance.PlayerName;
            Debug.Log($"Player name updated: {playerProfile.PlayerName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating player name: {e.Message}");
        }
    }

    private async Task SaveProfileDataAsync()
    {
        try
        {
            await CloudSaveManager.Instance.SavePublicDataAsync("PlayerProfile", playerProfile);
            Debug.Log("Player profile data saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving profile data: {e.Message}");
        }
    }
    private async Task SaveImageInCloudAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(playerProfile?.PlayerImageUrl))
            {
                Debug.Log("Player profile URL is null or empty, using default avatar.");
                Sprite sprite = Sprite.Create(avatarProfileImageTexture, new Rect(0, 0, avatarProfileImageTexture.width, avatarProfileImageTexture.height), new Vector2(0.5f, 0.5f));
                await UploadImageToCloudAsync(sprite);
                return;
            }

            // Download image from URL and upload to cloud
            await DownloadAndUploadImageAsync(playerProfile.PlayerImageUrl);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving image in cloud: {e.Message}");
            // Fallback to default avatar
            Sprite sprite = Sprite.Create(avatarProfileImageTexture, new Rect(0, 0, avatarProfileImageTexture.width, avatarProfileImageTexture.height), new Vector2(0.5f, 0.5f));
            await UploadImageToCloudAsync(sprite);
        }
    }

    private async Task DownloadAndUploadImageAsync(string url)
    {
        Debug.Log("Downloading image from URL: " + url);

        using var request = UnityWebRequestTexture.GetTexture(url);
        var operation = request.SendWebRequest();

        // Wait for the request to complete asynchronously
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading image: " + request.error);
            throw new Exception($"Failed to download image: {request.error}");
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image downloaded successfully.");
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite playerImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            await UploadImageToCloudAsync(playerImage);
        }
        else
        {
            throw new Exception("Unknown error when downloading image.");
        }
    }

    private async Task UploadImageToCloudAsync(Sprite playerImage)
    {
        try
        {
            // Step 1: Process ALL Unity operations on main thread first
            Texture2D resizedPlayerTexture = ImageUtility.ResizeTexture(playerImage.texture, 256, 256);
            Texture2D resizedBorderTexture = ImageUtility.ResizeTexture(borderProfileImageTexture, 256, 256);
            Texture2D resizedCoverTexture = ImageUtility.ResizeTexture(coverProfileImageTexture, 256, 256);

            // Compress textures on main thread (Unity requirement)
            byte[] playerImageBytes = ImageUtility.CompressTexture(resizedPlayerTexture, quality: 30);
            byte[] borderImageBytes = ImageUtility.CompressTexture(resizedBorderTexture, quality: 30);
            byte[] coverImageBytes = ImageUtility.CompressTexture(resizedCoverTexture, quality: 30);

            // Step 2: Create tasks for parallel image uploads (original textures)
            var uploadTasks = new List<Task>
            {
                CloudSaveManager.Instance.SaveImageAsync("PlayerProfileImage", playerImage.texture),
                CloudSaveManager.Instance.SaveImageAsync("PlayerProfileBorderImage", borderProfileImageTexture),
                CloudSaveManager.Instance.SaveImageAsync("PlayerProfileCoverImage", coverProfileImageTexture)
            };

            // Step 3: Convert to Base64 on main thread, then upload in parallel
            string playerImageBase64 = Convert.ToBase64String(playerImageBytes);
            string borderImageBase64 = Convert.ToBase64String(borderImageBytes);
            string coverImageBase64 = Convert.ToBase64String(coverImageBytes);

            var stringUploadTasks = new List<Task>
            {
                CloudSaveManager.Instance.SaveDataAsyncString<string>("PlayerImageUploaded", playerImageBase64),
                CloudSaveManager.Instance.SaveDataAsyncString<string>("PlayerBorderImageUploaded", borderImageBase64),
                CloudSaveManager.Instance.SaveDataAsyncString<string>("PlayerCoverImageUploaded", coverImageBase64)
            };

            // Wait for all uploads to complete
            await Task.WhenAll(uploadTasks);
            await Task.WhenAll(stringUploadTasks);

            // Update profile data
            playerProfile.DataPublicProfileImage = "PlayerImageUploaded";
            ConsumableItem item = new ConsumableItem()
            {
                Id = "PlayerImageUploaded",
                ConsumableName = "PlayerImageUploaded",
                DatePurchased = DateTime.MinValue.ToString(),
                DateExpired = DateTime.MaxValue.ToString()
            };

            // Update container profile avatar images
            var containerProfileAvatarImages = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileAvatarImages");
            if (containerProfileAvatarImages == null)
            {
                containerProfileAvatarImages = new List<ConsumableItem>();
            }
            containerProfileAvatarImages.Add(item);
            await CloudSaveManager.Instance.SaveDataAsync("ContainerProfileAvatarImages", containerProfileAvatarImages);

            Debug.Log("Player profile image uploaded successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error uploading image to cloud: {e.Message}");
            throw;
        }
    }
    private string GetImageBase64(Texture2D texture)
    {
        Texture2D resizeTexture = ImageUtility.ResizeTexture(texture, 256, 256);
        byte[] bytes = ImageUtility.CompressTexture(resizeTexture, quality: 30);

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

            // Note: This coroutine is now deprecated in favor of the async version
            StartCoroutine(UploadImageToCloudCoroutine(playerImage));
		}
		else
        {
            Debug.LogError("Unknown error when downloading image from Facebook.");
        }
    }

    private IEnumerator UploadImageToCloudCoroutine(Sprite playerImage)
    {
        var task = UploadImageToCloudAsync(playerImage);
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.IsFaulted)
        {
            Debug.LogError($"Error uploading image: {task.Exception?.GetBaseException().Message}");
        }
    }
    private void OnDisable() 
    {
        updateButton.onClick.RemoveListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.RemoveListener(OnUploadImageButtonClicked);
    }

    private void OnApplicationQuit()
    {
        AuthenticationService.Instance.SignOut();
    }
}

/*
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


    private void OnEnable()
    {
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
        string[] FileType = new string[] { "image/*" };

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                Debug.Log("Picked file with path " + path);
                // LoadImage(profileImage, path);

                if (filepath != path)
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
        }, FileType);

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

        if (www.result == UnityWebRequest.Result.ConnectionError)
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

        if (String.IsNullOrEmpty(filepath))
        {
            await cloudSaveManager.SaveImageAsync("PlayerProfileImage", texture);
        }
        else
        {
            await cloudSaveManager.SaveImageAsync("PlayerProfileImage", profileImage.sprite.texture);
        }

        var playerProfile = new PlayerProfile
        {
            PlayerId = AuthenticationService.Instance.PlayerId,
            PlayerName = playerName,
            Email = email,
            PhoneNumber = phoneNumber,
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

            }

        };

        await cloudSaveManager.SaveDataAsync("PlayerProfile", playerProfile);
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

        Debug.Log("Player profile updated successfully");
        OnUpdateSuccess?.Invoke();
    }

    private void OnDisable()
    {
        updateButton.onClick.RemoveListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.RemoveListener(OnUploadImageButtonClicked);
    }
}
*/