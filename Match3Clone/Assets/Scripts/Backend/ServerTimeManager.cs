using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ServerTimeManager : MonoBehaviour
{
    private static ServerTimeManager _instance;

    public static ServerTimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("ServerTimeManager");
                _instance = go.AddComponent<ServerTimeManager>();
                DontDestroyOnLoad(go); // Persist across scenes
            }
            return _instance;
        }
    }

    private const string TimeApiUrl = "https://www.timeapi.io/api/time/current/zone?timeZone=Asia%2FAmman";
    private DateTime? _serverTime;
    private bool _isInitialized = false;

    public DateTime CurrentTime
    {
        get
        {
            if (!_serverTime.HasValue)
            {
                Debug.LogWarning("Server time has not been initialized yet. Returning DateTime.MinValue.");
                return DateTime.MinValue; // Fallback value
            }

            // Calculate and return the current server time with the elapsed duration
            return _serverTime.Value.AddSeconds(Time.realtimeSinceStartup);
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize(); // Automatically initialize when the singleton is created
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    private async void Initialize()
    {
        if (_isInitialized) return;

        try
        {
            _serverTime = await FetchServerTimeAsync();
            _isInitialized = true;
            Debug.Log($"Server time initialized: {_serverTime}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize server time: {ex.Message}");
        }
    }

    private async Task<DateTime> FetchServerTimeAsync()
    {
        using UnityWebRequest request = UnityWebRequest.Get(TimeApiUrl);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log($"Server response JSON: {json}");

            ServerTimeResponse response = JsonUtility.FromJson<ServerTimeResponse>(json);
            return DateTime.Parse(response.dateTime);
        }
        else
        {
            throw new Exception($"Request failed: {request.error}");
        }
    }
}

[Serializable]
public class ServerTimeResponse
{
    public string dateTime;
}
