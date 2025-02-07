using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InternetCheck : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject errorInternetPanel;

    private static InternetCheck Instance;

    public bool IsConnected { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(CheckInternetPeriodically());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator CheckInternetPeriodically()
    {
        while (true)
        {
            yield return StartCoroutine(CheckInternetConnection(isConnected =>
            {
                IsConnected = isConnected;
                if (isConnected)
                {
                    loadingPanel.SetActive(false);
                }
                else
                {
                    Debug.Log("Internet is not connected");
                    loadingPanel.SetActive(false);
                    errorInternetPanel.SetActive(true);
                }
            }));

            yield return new WaitForSeconds(2f); // Check every 5 seconds
        }
    }
    public void ConnectInternet()
    {
        loadingPanel.SetActive(true);

        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                Debug.Log("Internet is connected");
                loadingPanel.SetActive(false);
                errorInternetPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Internet is not connected");
                loadingPanel.SetActive(false);
            }
        }));
    }

    public IEnumerator CheckInternetConnection(System.Action<bool> callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false);
            yield break;
        }

        UnityWebRequest request = new UnityWebRequest("https://www.google.com");
        yield return request.SendWebRequest();

        StartCoroutine(CheckInternetSpeed());

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            callback(false);
        }
        else
        {
            callback(true);
        }
    }
    
    private IEnumerator CheckInternetSpeed()
    {
        UnityWebRequest request = new UnityWebRequest("https://www.google.com");
        float startTime = Time.time;
        yield return request.SendWebRequest();
        float endTime = Time.time;

        float duration = endTime - startTime;

        if (duration > 2.5f)
        {
            loadingPanel.SetActive(true);
        }
        else
        {
            loadingPanel.SetActive(false);
        }
    }
    
}
