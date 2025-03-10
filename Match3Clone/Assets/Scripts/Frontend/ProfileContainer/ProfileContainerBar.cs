using System;
using System.Threading.Tasks;
using SaveData;
using UnityEngine;
using UnityEngine.UI;

public class ProfileContainerBar : MonoBehaviour
{
    [SerializeField] private Image borderImage;
    [SerializeField] private Image avatarImage;

    private void OnEnable() 
    {        
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileImage", avatarImage);
        LocalSaveManager.Instance.LoadImageAsync("PlayerProfileBorder", borderImage);
    }
    private void Awake() 
    {
        ProfileEditUI.OnImageChanged += OnImageChanged;
    }

    private void OnImageChanged(ConsumableType type, Sprite sprite)
    {
        switch(type)
        {
            case ConsumableType.PlayerProfileAvatar:
                avatarImage.sprite = sprite;
                break;
            case ConsumableType.PlayerProfileBorder:
                borderImage.sprite = sprite;
                break;
        }
    }
}
