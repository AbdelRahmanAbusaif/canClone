using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using Unity.Services.Core;
using System;
using Unity.Services.Authentication;

public class ProfileManager : MonoBehaviour
{
    public static Action OnUpdateSuccess;

    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField phoneNumberInputField;

    [SerializeField] private Button updateButton;
    [SerializeField] private Button uploadImageButton;

    [SerializeField] private Image profileImage;
    [SerializeField] private Texture2D texture;

    [SerializeField] private TextMeshProUGUI playerNameWarning;
    [SerializeField] private TextMeshProUGUI emailWarning;
    [SerializeField] private TextMeshProUGUI phoneNumberWarning;
    
    private CloudSaveManager cloudSaveManager;
    private string filepath = "";
    private void OnEnable() {
        updateButton.onClick.AddListener(OnUpdateButtonClicked);
        uploadImageButton.onClick.AddListener(OnUploadImageButtonClicked);

        cloudSaveManager = FindAnyObjectByType<CloudSaveManager>().GetComponent<CloudSaveManager>();
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
        string playerName = playerNameInputField.text;
        string email = emailInputField.text;
        string phoneNumber = phoneNumberInputField.text;

        // Here will be the code for updating the player profile
        if(Validation.IsValidName(playerName))
        {
            Debug.Log("Player name is valid");
        }
        else
        {
            Debug.Log("Player name is invalid");
            playerNameWarning.gameObject.SetActive(true);

            return;
        }

        if(Validation.IsValidEmail(email))
        {
            Debug.Log("Email is valid");
        }
        else
        {
            Debug.Log("Email is invalid");

            emailWarning.gameObject.SetActive(true);
            return;
        }

        if(Validation.IsValidPhoneNumber(phoneNumber))
        {
            Debug.Log("Phone number is valid");
        }
        else
        {
            Debug.Log("Phone number is invalid");

            phoneNumberWarning.gameObject.SetActive(true);
            return;
        }

        if(String.IsNullOrEmpty(filepath))
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
        };

        await cloudSaveManager.SaveDataAsync("PlayerProfile", playerProfile);

        Debug.Log("Player profile updated successfully");
        OnUpdateSuccess?.Invoke();
    }
    private class Validation{
        public static bool IsValidName(string name)
        {
            Regex regex = new(@"^[a-zA-Z0-9]*$");
            return regex.IsMatch(name) && name.Length > 0;
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^07[8-9][0-9]{7}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
