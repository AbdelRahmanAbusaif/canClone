using System.Collections;
using System.IO;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;

public class UpdateClip : MonoBehaviour
{
    [SerializeField] private string clipKey;
    [SerializeField] private AudioSource audioSource;

    [System.Obsolete]
    private void Start()
    {
        audioSource.Stop();
        LoadClip();
    }

    [System.Obsolete]
    private void LoadClip()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource object is not assigned.");
            return;
        }
        string clipPath = Path.Combine("DownloadedAssets", clipKey);

        StartCoroutine(LocalSaveManager.Instance.LoadClipAsync(clipPath, (loadedClip) =>
        {
            loadedClip.name = clipKey;
            audioSource.clip = loadedClip;
            audioSource.Play();
        }));
   }
}
