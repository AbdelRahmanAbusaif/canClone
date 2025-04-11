using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerId { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string PlayerImageUrl { get; set; } = "";
    public string DataPublicProfileImage { get; set; } = "";
    public string DataPublicProfileBorder { get; set; } = "";
    public int Level { get; set; } = 1;
    public bool IsAcceptedTerms { get; set; } = false;

    // public HeartSystem HeartSystem { get; set; } = new();
    // public DailyBonus DailyBonus { get; set; } = new();
    // public SpinWheel SpinWheel { get; set; } = new();
    // public ConsumableItem PrimeSubscriptions { get; set; } = new();
    // public List<AdManager> AdManager { get; set; } = new();
    // public List<LevelComplete> LevelsComplete { get; set; } = new();
    // public List<ConsumableItem> ContainerProfileAvatarImages { get; set; } = new();
    // public List<ConsumableItem> ContainerProfileCoverImages { get; set; } = new();
    // public List<ConsumableItem> ContainerProfileBorders { get; set; } = new();
}

[System.Serializable]
public class HeartSystem
{
    public int Heart { get; set; } = 0;
    public string LastHeartTime { get; set; } = "";
    public string NextHeartTime { get; set; } = "";
}

[System.Serializable]
public class AdManager
{
    public int AdCounter { get; set; } = 0;
    public string AdId { get; set; } = "";
    public string AdCurrentTimer { get; set; } = "";
    public string AdNextTimer { get; set; } = "";
}

[System.Serializable]
public class ConsumableItem
{
    public string Id { get; set; } = "";
    public string ConsumableName { get; set; } = "";
    public string DatePurchased { get; set; } = DateTime.MinValue.ToString();
    public string DateExpired { get; set; } = DateTime.MinValue.ToString();
}

[System.Serializable]
public class DailyBonus
{
    public string DateLastPlayed { get; set; } = "";
    public string DailyBonusDayKey { get; set; } = "";
}

[System.Serializable]
public class SpinWheel
{
    public string DateLastSpin { get; set; } = "";
    public string DailySpinDayKey { get; set; } = "";
}

[System.Serializable]
public class LevelComplete
{
    public int NumberLevel { get; set; } = 0;
    public int Stars { get; set; } = 0;
    public int Score { get; set; } = 0;
}

[System.Serializable]
public class GameAssetsFiles
{
    public string FileName { get; set; } = "";
    public string FileURL { get; set; } = "";
    public string LocalURL { get; set; } = "";
}
