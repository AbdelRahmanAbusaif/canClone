using System.Collections;
using System.IO;
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

        string clipPath = Path.Combine(Application.persistentDataPath,"DownloadedAssets", clipKey);

        if (!File.Exists(clipPath+".wav"))
        {
            Debug.LogError($"Audio file not found at path: {clipPath}.wav");
            return;
        }

        AudioClip audioClip = await LocalSaveManager.Instance.LoadClipAsync(clipPath);
        audioClip.name = clipKey;

        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
