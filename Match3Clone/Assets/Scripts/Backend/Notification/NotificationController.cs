#if UNITY_ANDROID
using System;
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
    public void ScheduleNotification(string title, string message, DateTime fireTime)
    {
        
        Debug.Log("Scheduling notification");
#if UNITY_ANDROID
        // Cancel all displayed notifications
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        androidNotificationSettings.SendNotification(title, message, fireTime);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iosNotificationSettings.SendNotification(title, message, fireTime);
#endif
        Debug.Log("Notification scheduled");
    }
}
