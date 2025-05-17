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

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ScheduleNotification();
        }
    }

    private void OnApplicationQuit()
    {
        ScheduleNotification();
    }

    private void ScheduleNotification()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
        androidNotificationSettings.SendNotification("Full Lives", "You have full lives now!", 1);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iosNotificationSettings.SendNotification("Full Lives", "You have full lives now!", 1);
#endif
        Debug.Log("Notification scheduled");
    }
}
