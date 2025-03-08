using System;

[Serializable]
public class ItemInformation
{
    public string Id;
    public string Title;
    public string Url;
    public ItemType Type;
}

[Serializable]
public class StoreItem : ItemInformation
{
    public string Description;
    public string PriceForOneDay;
    public string PriceForSevenDay;
    public string PriceForThirtyDay;
}
[Serializable]
public class PrimeSubscription : ItemInformation
{
    public string Price;
    public Duration DurationType;
}
public enum ItemType
{
    ProfileImage,
    CoverProfileImage,
    BorderImage,
    PrimeSubscription
}
