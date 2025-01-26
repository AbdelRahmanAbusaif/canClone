using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

namespace SaveData
{
    public class LocalSaveManager : MonoBehaviour
    {
        private static readonly object padlock = new object();
        private static LocalSaveManager instance = null;

        public static LocalSaveManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = FindFirstObjectByType<LocalSaveManager>();
                        if (instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            instance = singletonObject.AddComponent<LocalSaveManager>();
                            singletonObject.name = typeof(LocalSaveManager).ToString() + " (Singleton)";
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                    return instance;
                }
            }
        }

        public async Task SaveDataAsync<T>(T data, string key)
        {
            string saveDataFilePath = Path.Combine(Application.persistentDataPath, key + ".json");

            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            try
            {
                await File.WriteAllTextAsync(saveDataFilePath, jsonData);

                EncryptionHelper.EncryptAndSaveFile(saveDataFilePath, Encoding.UTF8.GetBytes(jsonData));
                Debug.Log("Data saved successfully to local storage.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving data: {e.Message}");
            }
        }

        public async Task<T> LoadDataAsync<T>(string key) where T : new()
        {
            try
            {
                string saveDataFilePath = Path.Combine(Application.persistentDataPath, key + ".json");

                if (File.Exists(saveDataFilePath))
                {
                    var bytes = EncryptionHelper.LoadAndDecryptFile(saveDataFilePath);
                    string jsonData = Encoding.UTF8.GetString(bytes);

                    Debug.Log("Data loaded successfully from local storage.");
                    Debug.Log(jsonData);
                    
                    return JsonConvert.DeserializeObject<T>(jsonData);
                }
                else
                {
                    var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
                    if (savedData.TryGetValue(key, out var item))
                    {
                        string jsonData = item.Value.GetAs<string>();

                        Debug.Log($"{key} loaded successfully.");
                        Debug.Log(jsonData);

                        return JsonConvert.DeserializeObject<T>(jsonData);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading {key}: {e.Message}");
            }

            return new T(); // Return default object if no data found
        }

        public async Task SaveImageAsync(Texture2D texture, string key)
        {
            byte[] imageData = ImageUtility.ConvertImageToBytes(texture);

            string saveImageFilePath = Path.Combine(Application.persistentDataPath, key + ".png");
            Debug.Log(saveImageFilePath);

            try
            {
                await File.WriteAllBytesAsync(saveImageFilePath, imageData);
                Debug.Log("Image saved successfully to local storage.");
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
                byte[] imageData = null;
                string saveImageFilePath = Path.Combine(Application.persistentDataPath, key + ".png");
                Debug.Log(saveImageFilePath);

                if (File.Exists(saveImageFilePath))
                {
                    imageData = File.ReadAllBytes(saveImageFilePath);

                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    Debug.Log("Image loaded successfully from local storage.");
                }
                else
                {
                    imageData = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(key);

                    Texture2D texture2D = new Texture2D(2, 2);
                    texture2D.LoadImage(imageData);
                    targetImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));

                    await SaveImageAsync(texture2D, key);

                    Debug.Log("Image loaded successfully from Unity Cloud Save.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load image: {e.Message}");
            }
        }

        public void DeleteData(string key)
        {
            string saveDataFilePath = Path.Combine(Application.persistentDataPath, key + ".json");

            if (File.Exists(saveDataFilePath))
            {
                File.Delete(saveDataFilePath);
                Debug.Log("Data deleted successfully from local storage.");
            }
        }

        public void DeleteImage(string key)
        {
            string saveImageFilePath = Path.Combine(Application.persistentDataPath, key + ".jpeg");

            if (File.Exists(saveImageFilePath))
            {
                File.Delete(saveImageFilePath);
                Debug.Log("Image deleted successfully from local storage.");
            }
        }
    }

}