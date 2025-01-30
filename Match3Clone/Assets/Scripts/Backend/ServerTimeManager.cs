using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerTimeManager : MonoBehaviour
{
    private static ServerTimeManager _instance;
    private DateTime? dateTime;

    public Action<bool> OnServerInitialized;
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

    public DateTime CurrentTime
    {
        get
        {
            if (!dateTime.HasValue)
            {
                Debug.LogWarning("Server time has not been initialized yet. Returning DateTime.MinValue.");
                return DateTime.MinValue; // Fallback value
            }

            // Calculate and return the current server time with the elapsed duration
            return dateTime.Value.AddSeconds(Time.realtimeSinceStartup);
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // Initialize(); // Automatically initialize when the singleton is created
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }


    public IEnumerator FetchServerTimeAsync()
    {
        using UnityWebRequest request = UnityWebRequest.Get(TimeApiUrl);
        request.timeout = 10; // Timeout in seconds

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log($"Server response JSON: {json}");

            ServerTimeResponse response = JsonUtility.FromJson<ServerTimeResponse>(json);
            
            OnServerInitialized?.Invoke(true);
            dateTime = DateTime.Parse(response.dateTime);
        }
        else
        {
            OnServerInitialized?.Invoke(false);

            Debug.LogError($"Failed to fetch server time: {request.error}");
        }
    }

    private void OnDestroy() {
        if(_instance == null)
        {
            _instance = null;
        }

        GC.Collect();
    }
}

[Serializable]
public class ServerTimeResponse
{
    public string dateTime;
}
