using Unity.Notifications.Android;
using UnityEngine;
#if UNITY_ANDROID
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
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
    public void SendNotification(string title, string text, int minutes)
    {
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = text,
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            FireTime = System.DateTime.Now.AddMinutes(minutes),
        };
        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
    #endif
}
