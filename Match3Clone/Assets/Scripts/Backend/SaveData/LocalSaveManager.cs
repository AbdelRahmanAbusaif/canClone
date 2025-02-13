using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine.Networking;
using System.Collections;

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

                // EncryptionHelper.EncryptAndSaveFile(saveDataFilePath, Encoding.UTF8.GetBytes(jsonData));
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
                    // var bytes = EncryptionHelper.LoadAndDecryptFile(saveDataFilePath);
                    var bytes = await File.ReadAllBytesAsync(saveDataFilePath);
                    string jsonData = Encoding.UTF8.GetString(bytes);

                    Debug.Log("Data loaded successfully from local storage.");
                    Debug.Log(jsonData);
                    
                    return JsonConvert.DeserializeObject<T>(jsonData);
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
            Texture2D resizedTexture = ImageUtility.ResizeTexture(texture, 128, 128);

            byte[] imageData = ImageUtility.CompressTexture(resizedTexture, quality: 50);
            
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
                    var image = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key }, new Unity.Services.CloudSave.Models.Data.Player.LoadOptions(new PublicReadAccessClassOptions()));

                    if(image.TryGetValue(key, out var item))
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
                        await SaveImageAsync(texture, key);
                    }

                    Debug.Log("Image loaded successfully from Unity Cloud Save.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load image: {e.Message}");
            }
        }
        public async Task<AudioClip> LoadClipAsync(string key)
        {
            string saveClipFilePath = Path.Combine(Application.persistentDataPath, key + ".wav");
            Debug.Log(saveClipFilePath);

            if (File.Exists(saveClipFilePath))
            {
                UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(saveClipFilePath, AudioType.WAV);
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                    Debug.Log("Clip loaded successfully from local storage.");
                    return clip;
                }
                else
                {
                    Debug.LogError($"Failed to load clip: {request.error}");
                    return null;
                }
            }
            else
            {
                Debug.Log("Clip Not Found in Local Storage.");
                return null;
            }
        }        
        public async Task<Sprite> LoadSpriteAsync(string key)
        {
            string saveClipFilePath = Path.Combine(Application.persistentDataPath, key + ".png");
            Debug.Log(saveClipFilePath);

            if (File.Exists(saveClipFilePath))
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(saveClipFilePath);
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);

                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    sprite.name = key;
                    Debug.Log("texture loaded successfully from local storage.");
                    return sprite;
                }
                else
                {
                    Debug.LogError($"Failed to load texture: {request.error}");
                    return null;
                }
            }
            else
            {
                Debug.Log("Texture Not Found in Local Storage.");
                return null;
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
            string saveImageFilePath = Path.Combine(Application.persistentDataPath, key + ".png");

            if (File.Exists(saveImageFilePath))
            {
                File.Delete(saveImageFilePath);
                Debug.Log("Image deleted successfully from local storage.");
            }
        }
    }

}