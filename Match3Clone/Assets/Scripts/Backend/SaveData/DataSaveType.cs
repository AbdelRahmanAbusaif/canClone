using UnityEngine.UI;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerId;
    public string PlayerName;
    public string Email;
    public string PhoneNumber;
}

[System.Serializable]
public class PlayerProgress
{
    public int Level;
    public int Heart;
    public int Token;
    public int ExperiencePoints;
    public int HighestScore;
}