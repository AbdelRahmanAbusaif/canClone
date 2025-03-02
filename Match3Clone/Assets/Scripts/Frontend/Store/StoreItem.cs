using System;
[Serializable]
public class StoreItem 
{
    public string Id;
    public string Title;
    public string Description;
    public string PriceForOneDay;
    public string PriceForSevenDay;
    public string PriceForThirtyDay;
    public string Url;
    public ItemType Type;
    public enum ItemType
    {
        ProfileImage,
        CoverProfileImage,
        BorderImage,
    }
}
