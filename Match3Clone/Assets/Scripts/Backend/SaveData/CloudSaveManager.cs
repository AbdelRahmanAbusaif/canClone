using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SaveData
{
    public class CloudSaveManager : MonoBehaviour
    {
        public static CloudSaveManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public async Task SaveDataAsync<T>(string key, T data)
        {
            string jsonData = JsonUtility.ToJson(data);
            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { key, jsonData }
                });
                await LocalSaveManager.Instance.SaveDataAsync(data, key);
                Debug.Log($"{key} saved successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving {key}: {e.Message}");
            }
        }
        public async Task SaveDataAsyncString<T>(string key, T data)
        {
            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { key, data }
                });
                await LocalSaveManager.Instance.SaveDataAsync(data, key);
                Debug.Log($"{key} saved successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving {key}: {e.Message}");
            }
        }

        public async Task<T> LoadDataAsync<T>(string key) where T : new()
        {
            try
            {
                var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
                if (savedData.TryGetValue(key,out var item))
                {
                    string jsonData = item.Value.GetAs<string>();
                
                    Debug.Log($"{key} loaded successfully.");
                    Debug.Log(jsonData);

                    T profileData = JsonUtility.FromJson<T>(jsonData);
                    await LocalSaveManager.Instance.SaveDataAsync(profileData, key);
                
                    return JsonUtility.FromJson<T>(jsonData);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading {key}: {e.Message}");
            }

            return new T(); // Return default object if no data found
        }
        public async Task SaveImageAsync(string key, Texture2D texture)
        {
            Texture2D resizedTexture = ImageUtility.ResizeTexture(texture, 128, 128);

            byte[] imageData = ImageUtility.CompressTexture(resizedTexture, quality: 50);

            try
            {
                var data = new Dictionary<string, object>
                {
                    { key, Convert.ToBase64String(imageData) }
                };

                await CloudSaveService.Instance.Data.Player.SaveAsync(data, new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions()));
                await LocalSaveManager.Instance.SaveImageAsync(texture, key);
                
                Debug.Log("Image saved successfully to Unity Cloud Save.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save image: {e.Message}");
            }
        }
        public async void LoadImageAsync(string key, Image targetImage, bool publicData = true)
        {
            try
            {
                Dictionary<string, Unity.Services.CloudSave.Models.Item> imageData = null;

                if(publicData)
                {
                    imageData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>{key}, new LoadOptions(new PublicReadAccessClassOptions()));
                }
                else 
                {
                    imageData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>{key});
                }
                if(imageData.TryGetValue(key, out var item))
                {
                    string base64Image = item.Value.GetAs<string>();
                    byte[] imageDataBytes = System.Convert.FromBase64String(base64Image);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageDataBytes); // Convert bytes to texture

                    targetImage.sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    Debug.Log("Image loaded successfully from Unity Cloud Save.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load image: {e.Message}");
            }
        }

        public async void LoadImageUsePlayerId(string playerId, Image targetImage)
        {
            try
            {
                var imageData = await LoadPublicDataByPlayerId(playerId, "PlayerProfileImage");

                byte[] imageDataBytes = System.Convert.FromBase64String(imageData);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageDataBytes); // Convert bytes to texture

                targetImage.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                Debug.Log("Image loaded successfully from Unity Cloud Save.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load image: {e.Message}");
            }
        }
        public async Task<string> LoadPublicDataByPlayerId(string playerId, string key)
        {

            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>{key}, new LoadOptions(new PublicReadAccessClassOptions(playerId)));
            if (playerData.TryGetValue(key, out var keyName)) {
                Debug.Log($"keyName: {keyName.Value.GetAs<string>()}");
                return keyName.Value.GetAs<string>();
            }
            else
            {
                Debug.LogError($"No data found for key: {key}");
                return null;
            }
        }
        // public async void LoadPlayerProfileImage(string playerId, Image avatarImage)
        // {
        //     // Load the profile image from Cloud Save
        //     // var data = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(new HashSet<string> { $"{playerId}_profileImage" });
        //     if (data.TryGetValue($"{playerId}_profileImage", out var base64Image))
        //     {
        //         byte[] avatarBytes = System.Convert.FromBase64String(base64Image.ToString());
        //         Texture2D texture = new Texture2D(2, 2);
        //         texture.LoadImage(avatarBytes); // Convert bytes to texture
                
        //         // Apply the texture to the Image component
        //         avatarImage.sprite = Sprite.Create(
        //             texture,
        //             new Rect(0, 0, texture.width, texture.height),
        //             new Vector2(0.5f, 0.5f)
        //         );
        //     }
        //     else
        //     {
        //         Debug.LogError($"No profile image found for player: {playerId}");
        //     }
        // }
    }
}