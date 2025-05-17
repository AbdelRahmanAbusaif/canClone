using UnityEngine;
using Unity.Services.PushNotifications;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
public class PushNotificationHandler : MonoBehaviour
{
    async void Start()
    {
        await Unity.Services.Core.UnityServices.InitializeAsync();
      
        PushNotificationsService.Instance.OnRemoteNotificationReceived += OnRemoteNotificationReceived;

        var registrationResult = await PushNotificationsService.Instance.RegisterForPushNotificationsAsync();
        Debug.Log("UGS device token: " + registrationResult);
    }

    private void OnRemoteNotificationReceived(Dictionary<string, object> dictionary)
    {
        Debug.Log("Notification received: " + dictionary);
        // Handle the notification data here
        // For example, you can extract the title and message from the dictionary
        if (dictionary.TryGetValue("title", out object title))
        {
            Debug.Log("Title: " + title);
        }
        if (dictionary.TryGetValue("message", out object message))
        {
            Debug.Log("Message: " + message);
        }
    }
}
