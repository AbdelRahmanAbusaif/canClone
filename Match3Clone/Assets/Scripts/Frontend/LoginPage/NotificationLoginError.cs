using System;
using TMPro;
using UnityEngine;

public class NotificationLoginError : MonoBehaviour
{
    [SerializeField] private FirebaseManager firebaseManager;
    [SerializeField] private GameObject notificationPage;

    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    private void Awake()
    {
        firebaseManager.onErrorShow += GetShowErrorNotification;
    }

    private void GetShowErrorNotification(string message)
    {
        CancelInvoke(nameof(DiableTheNotification));

        notificationPage.SetActive(true);
        textMeshProUGUI.text = message;

        Invoke(nameof(DiableTheNotification), 2);
    }

    private void DiableTheNotification()
    {
        notificationPage.GetComponent<AnimationBox>().OnClose();
    }
}
