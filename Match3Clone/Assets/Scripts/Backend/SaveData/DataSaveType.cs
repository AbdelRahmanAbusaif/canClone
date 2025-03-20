using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerId = "";
    public string PlayerName = "";
    public string Email = "";
    public string PhoneNumber = "";
    public string DataPublicProfileImage = "";
    public string DataPublicProfileBorder = "";
    public int Level = 1;
    public bool IsAcceptedTerms = false;
    public HeartSystem HeartSystem = new();
    public DailyBonus DailyBonus = new();
    public SpinWheel SpinWheel = new();
    public ConsumableItem PrimeSubscriptions = new();
    public List<AdManager> AdManager = new();
    public List<LevelComplete> LevelsComplete = new();
    public List<ConsumableItem> ContainerProfileAvatarImages = new();
    public List<ConsumableItem> ContainerProfileCoverImages = new();
    public List<ConsumableItem> ContainerProfileBorders = new();
}
[Serializable]
public class HeartSystem
{
    public int Heart = 0;
    public string LastHeartTime = "";
    public string NextHeartTime = "";
}
[System.Serializable]
public class AdManager
{
    public int AdCounter = 0;
    public string AdId = "";
    public string AdCurrentTimer = "";  
    public string AdNextTimer = "";
}
[System.Serializable]
public class ConsumableItem
{
    public string Id = "";
    public string ConsumableName = "";
    public string DatePurchased = DateTime.MinValue.ToString();
    public string DateExpired = DateTime.MinValue.ToString();
}

[System.Serializable]
public class DailyBonus
{
    public string DateLastPlayed = "";
    public string DailyBonusDayKey = "";
}
[System.Serializable]
public class SpinWheel
{
    public string DateLastSpin = "";
    public string DailySpinDayKey = "";
}
[System.Serializable]
public class LevelComplete
{
    public int NumberLevel = 0;
    public int Stars = 0;
    public int Score = 0;
}
[System.Serializable]
public class GameAssetsFiles
{
    public string FileName = "";
    public string FileURL = "";
    public string LocalURL = "";
}