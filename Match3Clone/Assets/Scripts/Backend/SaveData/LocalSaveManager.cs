using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace SaveData
{
    public class LocalSaveManager 
    {
        private string saveImageFilePath;
        private string saveDataFilePath;

        public static LocalSaveManager Instance{ get; private set ;} = new LocalSaveManager();

        private LocalSaveManager()
        {  
            saveImageFilePath = Path.Combine(Application.persistentDataPath, "PlayerProfile.jpeg");
            saveDataFilePath = Path.Combine(Application.persistentDataPath, "PlayerProfile.json");
        }
        public async Task SaveDataAsync<T>(T data)
        {
            string jsonData = JsonUtility.ToJson(data);
            try
            {
                await File.WriteAllTextAsync(saveDataFilePath, jsonData);
                Debug.Log("Data saved successfully to local storage.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving data: {e.Message}");
            }
        }

        public async Task<T> LoadDataAsync<T>(string key = "") where T : new()
        {
            try
            {
                if(File.Exists(saveDataFilePath))
                {
                    string jsonData = await File.ReadAllTextAsync(saveDataFilePath);
                    Debug.Log("Data loaded successfully from local storage.");

                    Debug.Log(jsonData);
                    return JsonUtility.FromJson<T>(jsonData);
                }
                else
                {

                    var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
                    if (savedData.TryGetValue(key,out var item))
                    {

                        string jsonData = item.Value.GetAs<string>();
                    
                        Debug.Log($"{key} loaded successfully.");
                        Debug.Log(jsonData);
                    
                        return JsonUtility.FromJson<T>(jsonData);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading {key}: {e.Message}");
            }

            return new T(); // Return default object if no data found
        }

        public async Task SaveImageAsync(Texture2D texture)
        {
            byte[] imageData = ImageUtility.ConvertImageToBytes(texture);

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

                if (File.Exists(saveImageFilePath))
                {
                    imageData = File.ReadAllBytes(saveImageFilePath);

                    Debug.Log("Image loaded successfully from local storage.");
                }
                else
                {
                    imageData = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(key);

                    Debug.Log("Image loaded successfully from Unity Cloud Save.");
                }
                
                if (imageData != null)
                {
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load image: {e.Message}");
            }
        }
    }
}