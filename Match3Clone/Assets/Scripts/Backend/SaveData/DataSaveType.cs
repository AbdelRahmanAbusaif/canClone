using UnityEngine.UI;

[System.Serializable]
public class PlayerProfile
{
    public string PlayerId;
    public string PlayerName;
    public string Email;
    public string PhoneNumber;
    public int Level;
    public int HighestScore;
}

[System.Serializable]
public class PlayerProgress
{
    
}
[System.Serializable]
public class GameAssetsFiles
{
    public string FileName;
    public string FileURL;
    public string LocalURL;
}