using UnityEngine.UI;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerName;
    public string Email;
    public string PhoneNumber;
    public string ImageUrl;
}

[System.Serializable]
public class PlayerProgress
{
    public int level;
    public int coins;
    public int experiencePoints;
    public int highestScore;
}