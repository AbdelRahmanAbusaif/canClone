using System.Collections;
using System.IO;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;

public class UpdateTexture : MonoBehaviour
{
    [SerializeField] private string textureKey;
    [SerializeField] private SpriteRenderer sprite;
    

    private void Start()
    {
        // audioSource.Stop();
        LoadTexture();
    }

    private async void LoadTexture()
    {
        if (sprite == null)
        {
            Debug.LogError("Texture object is not assigned.");
            return;
        }
        string texturePath = Path.Combine("DownloadedAssets", textureKey);

        sprite.sprite = await LocalSaveManager.Instance.LoadSpriteAsync(texturePath);        
    }
}
