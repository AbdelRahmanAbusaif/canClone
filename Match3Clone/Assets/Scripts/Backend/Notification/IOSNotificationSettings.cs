using System;
using System.Collections;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

public class IOSNotificationSettings : MonoBehaviour
{
    #if UNITY_IOS
    public IEnumerator RequestAuthorization()
    {
        var request = new Unity.Notifications.iOS.AuthorizationRequest(
            Unity.Notifications.iOS.AuthorizationOption.Alert | Unity.Notifications.iOS.AuthorizationOption.Badge | Unity.Notifications.iOS.AuthorizationOption.Sound,
            true);
        yield return request;
        if (request.IsFinished)
        {
            if (request.Granted)
            {
                Debug.Log("Notification permission granted");
            }
            else
            {
                Debug.Log("Notification permission denied");
            }
        }
        else
        {
            while (!request.IsFinished)
            {
                yield return null;
            }
            Debug.Log("Notification permission request failed");
        }
    }
    public void SendNotification(string title, string text, int seconds)
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger
        {
            TimeInterval = TimeSpan.FromSeconds(seconds),
            Repeats = false
        };

        var notification = new iOSNotification
        {
            Identifier = "default_notification",
            Title = title,
            Body = text,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound | PresentationOption.Badge),
            CategoryIdentifier = "default_category",
            ThreadIdentifier = "default_thread",
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }
    #endif
}
