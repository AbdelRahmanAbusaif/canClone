using UnityEngine.UI;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerName;
    public string Email;
    public string PhoneNumber;
}

[System.Serializable]
public class PlayerProgress
{
    public int Level;
    public int Coin;
    public int ExperiencePoints;
    public int HighestScore;
}