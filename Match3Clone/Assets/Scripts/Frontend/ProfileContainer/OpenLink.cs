using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public void OpenUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("Attempted to open an empty URL.");
        }
    }
}
