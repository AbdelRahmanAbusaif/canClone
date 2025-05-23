using UnityEngine;
using System;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public class AndroidNotificationSettings : MonoBehaviour
{
#if UNITY_ANDROID
    public void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }
    public void RequestNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Full Lives",
            EnableVibration = true,
            LockScreenVisibility = LockScreenVisibility.Public,
            CanShowBadge = true,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
    public void SendNotification(string title, string text, DateTime fireTime)
    {
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = text,
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            FireTime = fireTime,
        };
        var id = AndroidNotificationCenter.SendNotification(notification, "default_channel");

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            Debug.Log("Notification scheduled");
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
            AndroidNotificationCenter.CancelNotification(id);
            AndroidNotificationCenter.SendNotification(notification, "default_channel");
        }
        else
        {
            Debug.Log("Failed to schedule notification");
        }
    }
    #endif
}
