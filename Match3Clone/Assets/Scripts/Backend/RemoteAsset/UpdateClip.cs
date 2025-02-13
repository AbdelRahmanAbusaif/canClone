using System.Collections;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;

public class UpdateClip : MonoBehaviour
{
    [SerializeField] private string clipKey;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        audioSource.Stop();
        LoadClip();
    }

    private async void LoadClip()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource object is not assigned.");
            return;
        }
        string clipPath = "DownloadedAssets/" + clipKey;

        AudioClip audioClip = await LocalSaveManager.Instance.LoadClipAsync(clipPath);
        audioClip.name = clipKey;

        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
