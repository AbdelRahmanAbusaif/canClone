using System;
[Serializable]
public class StoreItem 
{
    public string Title;
    public string Description;
    public string Price;
    public string Url;
    public ItemType Type;
    public enum ItemType
    {
        ProfileImage,
        BorderImage,
    }
}
