using UnityEngine;

public class AdCoordinator : MonoBehaviour
{
    public static AdCoordinator Instance;

    private bool isAdShowing = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanShowAd()
    {
        return !isAdShowing;
    }

    public void NotifyAdStarted()
    {
        isAdShowing = true;
    }

    public void NotifyAdEnded()
    {
        isAdShowing = false;
    }
}
