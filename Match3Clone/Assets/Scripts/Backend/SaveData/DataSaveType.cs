using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerId;
    public string PlayerName;
    public string Email;
    public string PhoneNumber;
    public string LastHeartTime;
    public int Level;
    public DailyBonus DailyBonus;
    public SpinWheel SpinWheel;
    public List<LevelComplete> LevelsComplete;
}
[System.Serializable]
public class DailyBonus
{
    public string DateLastPlayed;
    public string DailyBonusDayKey;
}
[System.Serializable]
public class SpinWheel
{
    public string DateLastSpin;
    public string DailySpinDayKey;
}
[System.Serializable]
public class LevelComplete
{
    public int NumberLevel;
    public int Stars;
    public int Score;
}
[System.Serializable]
public class GameAssetsFiles
{
    public string FileName;
    public string FileURL;
    public string LocalURL;
}