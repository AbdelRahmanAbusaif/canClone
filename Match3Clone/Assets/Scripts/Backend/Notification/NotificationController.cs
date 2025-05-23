#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using Unity.VisualScripting;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [SerializeField] private AndroidNotificationSettings androidNotificationSettings;
    [SerializeField] private IOSNotificationSettings iosNotificationSettings;
    private void Start()
    {
#if UNITY_ANDROID
        androidNotificationSettings.RequestAuthorization();
        androidNotificationSettings.RequestNotificationChannel();
#elif UNITY_IOS
        StartCoroutine(iosNotificationSettings.RequestAuthorization());
#endif
    }
    public void ScheduleNotification(string title, string message, int seconds)
    {
#if UNITY_ANDROID
        // Cancel all displayed notifications
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        androidNotificationSettings.SendNotification(title, message, seconds);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iosNotificationSettings.SendNotification(title, message, seconds);
#endif
        Debug.Log("Notification scheduled");
    }
}
