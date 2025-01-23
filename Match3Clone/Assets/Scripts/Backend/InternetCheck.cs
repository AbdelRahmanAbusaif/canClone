using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InternetCheck : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject errorInternetPanel;
    
    private  void Start()
    {
        ConnectInternet();
    }

    public void ConnectInternet()
    {
        loadingPanel.SetActive(true);
        errorInternetPanel.SetActive(false);

        StartCoroutine(CheckInternetConnection((isConnected) =>
        {

            if (isConnected)
            {
                Debug.Log("Internet is connected");

                loadingPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Internet is not connected");

                loadingPanel.SetActive(false);
                errorInternetPanel.SetActive(true);
            }
        }));
    }

    public IEnumerator CheckInternetConnection(System.Action<bool> callback)
    {
        UnityWebRequest request = new UnityWebRequest("https://www.google.com");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            callback(false);
        }
        else
        {
            callback(true);
        }
    }
}
