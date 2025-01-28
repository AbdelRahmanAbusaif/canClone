using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ServerTimeManager : MonoBehaviour
{
    private const string TimeApiUrl = "https://www.timeapi.io/api/time/current/zone?timeZone=Asia%2FAmman";
    private DateTime serverTime;

    // Public method to fetch server time
    public async Task FetchServerTime(Action<DateTime> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetServerTime(onSuccess, onError));
    }

    private IEnumerator GetServerTime(Action<DateTime> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get(TimeApiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                string json = request.downloadHandler.text;
                Debug.Log($"Server response JSON: {json}");
                ServerTimeResponse response = JsonUtility.FromJson<ServerTimeResponse>(json);
                serverTime = DateTime.Parse(response.dateTime);

                Debug.Log($"Server Time: {serverTime}");
                onSuccess?.Invoke(serverTime); // Pass the server time to the callback
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse server response: {e.Message}");
                onError?.Invoke($"Parsing error: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"Failed to fetch server time: {request.error}");
            onError?.Invoke($"Request error: {request.error}");
        }
    }
}

[Serializable]
public class ServerTimeResponse
{
    public string dateTime;
}
