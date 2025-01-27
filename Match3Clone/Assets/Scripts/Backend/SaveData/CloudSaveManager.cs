using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

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
            byte[] imageData = ImageUtility.ConvertImageToBytes(texture);

            try
            {
                await CloudSaveService.Instance.Files.Player.SaveAsync(key, imageData);
                await LocalSaveManager.Instance.SaveImageAsync(texture, key);
                
                Debug.Log("Image saved successfully to Unity Cloud Save.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save image: {e.Message}");
            }
        }
        public async void LoadImageAsync(string key, UnityEngine.UI.Image targetImage)
        {
            try
            {
                var imageData = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(key);
                if (imageData != null)
                {
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    await LocalSaveManager.Instance.SaveImageAsync(texture, key);
                    Debug.Log("Image loaded successfully from Unity Cloud Save.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load image: {e.Message}");
            }
        }
    }
}