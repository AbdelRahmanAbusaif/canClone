using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AirshipAdUI : MonoBehaviour
{
    public Image adImage;
    public Image airshipImage;

    public void SetData(string adUrl , string airshipImageUrl)
    {
        LoadImage(adUrl, ref adImage);
        LoadImage(airshipImageUrl, ref airshipImage);
    }

    private void LoadImage(string URL, ref Image image)
    {
        StartCoroutine(LoadImageCoroutine(URL, image));
    }

    private IEnumerator LoadImageCoroutine(string uRL, Image image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uRL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
