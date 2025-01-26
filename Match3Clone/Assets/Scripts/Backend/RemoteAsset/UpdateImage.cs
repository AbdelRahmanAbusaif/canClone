using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;
using UnityEngine.UI;

public class UpdateImage : MonoBehaviour
{
    [SerializeField] private string imageKey;
    [SerializeField] private Image image;

    void Start()
    {
        LoadImage();
    }

    private void LoadImage()
    {
        if(image == null)
        {
            Debug.LogError("Image object is not assigned.");
            return;
        }
        string imagePath = Path.Combine("DownloadedAssets", imageKey); 
    
        LocalSaveManager.Instance.LoadImageAsync(imagePath,image);    
    }
}
