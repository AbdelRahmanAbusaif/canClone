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
    private async void OnEnable()
    {
        foreach (var item in profileEditItemData)
        {
            foreach (Transform child in item.ItemContainer.transform)
            {
                Destroy(child.gameObject);
            }
            
            item.ItemValues?.Clear();
            item.emptyItem.SetActive(false);

        }
        foreach (var item in profileEditItemData)
        {
            switch (item.ListItemId)
            {
                case ConsumableType.PlayerProfileAvatar:
                    List<ConsumableItem> containerProfileAvatarImages = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileAvatarImages");
                    foreach (var containerProfileAvatarImage in containerProfileAvatarImages)
                    {
                        item.ItemValues.Add(containerProfileAvatarImage);

                        GameObject cloneObject = Instantiate(item.ItemPrefab, item.ItemContainer.transform);
                        cloneObject.GetComponent<ProfileEditUI>().SetItemData(containerProfileAvatarImage.Id, containerProfileAvatarImage.ConsumableName, ConsumableType.PlayerProfileAvatar);

						item.ItemContainer.GetComponent<RectTransform>().sizeDelta =
	                        new Vector2(item.ItemContainer.GetComponent<RectTransform>().sizeDelta.x,
	                        item.ItemContainer.GetComponent<RectTransform>().sizeDelta.y + item.ItemPrefab.GetComponent<RectTransform>().sizeDelta.y + 20f);
					}
                    break;
                case ConsumableType.PlayerProfileCover:
                    List<ConsumableItem> containerProfileCoverImages = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileCoverImages");
                    foreach (var containerProfileCoverImage in containerProfileCoverImages)
                    {
                        item.ItemValues.Add(containerProfileCoverImage);

                        GameObject cloneObject = Instantiate(item.ItemPrefab, item.ItemContainer.transform);
                        cloneObject.GetComponent<ProfileEditUI>().SetItemData(containerProfileCoverImage.Id, containerProfileCoverImage.ConsumableName, ConsumableType.PlayerProfileCover);

						item.ItemContainer.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(item.ItemContainer.GetComponent<RectTransform>().sizeDelta.x,
                            item.ItemContainer.GetComponent<RectTransform>().sizeDelta.y + item.ItemPrefab.GetComponent<RectTransform>().sizeDelta.y + 20f);

					}
                    break;
                case ConsumableType.PlayerProfileBorder:
                    List<ConsumableItem> containerProfileBorders = await LocalSaveManager.Instance.LoadDataAsync<List<ConsumableItem>>("ContainerProfileBorders");
                    foreach (var containerProfileBorder in containerProfileBorders)
                    {
                        item.ItemValues.Add(containerProfileBorder);

                        GameObject cloneObject = Instantiate(item.ItemPrefab, item.ItemContainer.transform);
                        cloneObject.GetComponent<ProfileEditUI>().SetItemData(containerProfileBorder.Id, containerProfileBorder.ConsumableName, ConsumableType.PlayerProfileBorder);

						item.ItemContainer.GetComponent<RectTransform>().sizeDelta =
	                    new Vector2(item.ItemContainer.GetComponent<RectTransform>().sizeDelta.x,
	                    item.ItemContainer.GetComponent<RectTransform>().sizeDelta.y + item.ItemPrefab.GetComponent<RectTransform>().sizeDelta.y + 20f);
					}
                    break;
            }
        }

        foreach (var item in profileEditItemData)
        {
            if(item.ItemValues.Count == 0)
            {
                item.emptyItem.SetActive(true);
            }
        }

    }

}
[Serializable]
public class ProfileEditItemData
{
    public ConsumableType ListItemId;
    public GameObject emptyItem;
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