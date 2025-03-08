using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaveData;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class ProfileEditItem : MonoBehaviour
{
    [SerializeField] private List<ProfileEditItemData> profileEditItemData;
    private PlayerProfile playerProfile;
    private async void Awake() 
    {
        
        await UnityServices.InitializeAsync();
        if(AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Signed in");
        }
        else
        {
            Debug.Log("Not signed in");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously");
        }
    }
    private async void Start()
    {
        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        foreach (var item in profileEditItemData)
        {
            switch (item.ListItemId)
            {
                case ConsumableType.PlayerProfileAvatar:
                    List<ConsumableItem> containerProfileAvatarImages = playerProfile.ContainerProfileAvatarImages;
                    foreach (var containerProfileAvatarImage in containerProfileAvatarImages)
                    {
                        item.ItemValues.Add(containerProfileAvatarImage);

                        GameObject cloneObject = Instantiate(item.ItemPrefab, item.ItemContainer.transform);
                        cloneObject.GetComponent<ProfileEditUI>().SetItemData(containerProfileAvatarImage.Id, containerProfileAvatarImage.ConsumableName);
                    }
                    break;
                case ConsumableType.PlayerProfileCover:
                    List<ConsumableItem> containerProfileCoverImages = playerProfile.ContainerProfileCoverImages;
                    foreach (var containerProfileCoverImage in containerProfileCoverImages)
                    {
                        item.ItemValues.Add(containerProfileCoverImage);

                        GameObject cloneObject = Instantiate(item.ItemPrefab, item.ItemContainer.transform);
                        cloneObject.GetComponent<ProfileEditUI>().SetItemData(containerProfileCoverImage.Id, containerProfileCoverImage.ConsumableName);
                    }
                    break;
                case ConsumableType.PlayerProfileBorder:
                    List<ConsumableItem> containerProfileBorders = playerProfile.ContainerProfileBorders;
                    foreach (var containerProfileBorder in containerProfileBorders)
                    {
                        item.ItemValues.Add(containerProfileBorder);

                        GameObject cloneObject = Instantiate(item.ItemPrefab, item.ItemContainer.transform);
                        cloneObject.GetComponent<ProfileEditUI>().SetItemData(containerProfileBorder.Id, containerProfileBorder.ConsumableName);
                    }
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update code here
    }
}
[Serializable]
public class ProfileEditItemData
{
    public ConsumableType ListItemId;
    public GameObject ItemPrefab;
    public GameObject ItemContainer;
    public List<ConsumableItem> ItemValues;
}
public enum ConsumableType
{
    PlayerProfileAvatar,
    PlayerProfileCover,
    PlayerProfileBorder,
}