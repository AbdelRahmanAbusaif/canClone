// using System;
// using Firebase.Auth;
// using Firebase.Extensions;
// using UnityEngine;
// using UnityEngine.UI;

// public class FirebaseVerificationCode : MonoBehaviour
// {
//     [SerializeField] private GameObject verificationPanel;
//     [SerializeField] private AnimationBox animationBox;
//     [SerializeField] private Button verifyButton;
//     [SerializeField] private Button resendButton;
//     private FirebaseAuth auth;
//     private void Start()
//     {
//         auth = FirebaseAuth.DefaultInstance;

//         if (auth == null)
//         {
//             Debug.LogError("FirebaseAuth is not initialized.");
//         }
//         else
//         {
//             Debug.Log("FirebaseAuth initialized successfully.");
//         }

//         var user = auth.CurrentUser;
//         if (user != null)
//         {
//             Debug.Log($"Current user: {user.DisplayName} ({user.Email})");
//         }
//         else
//         {
//             Debug.Log("No user is currently signed in.");
//             return;
//         }

//         if (user.IsEmailVerified)
//         {
//             Debug.Log("User's email is verified.");
//         }
//         else
//         {
//             Debug.Log("User's email is not verified.");
//             verificationPanel.SetActive(true);

//             SendAuthMessage();

//             verifyButton.onClick.AddListener(VerifyEmail);
//             resendButton.onClick.AddListener(SendAuthMessage);
//         }
//     }

//     private void SendAuthMessage()
//     {
//         var user = auth.CurrentUser;
//         if (user != null)
//         {
//             user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
//             {
//                 if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
//                 {
//                     Debug.Log("Verification email sent successfully.");
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to send verification email: " + task.Exception);
//                 }
//             });
//         }
//         else
//         {
//             Debug.LogError("No user is currently signed in to resend verification email.");
//         }
//     }


//     private void VerifyEmail()
//     {
//         var user = auth.CurrentUser;
//         if (user != null)
//         {
//             user.ReloadAsync().ContinueWithOnMainThread(task =>
//             {
//                 if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
//                 {
//                     if (user.IsEmailVerified)
//                     {
//                         Debug.Log("Email is verified.");
//                         animationBox.OnClose();
//                     }
//                     else
//                     {
//                         Debug.Log("Email is still not verified.");
//                     }
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to reload user: " + task.Exception);
//                 }
//             });
//         }
//         else
//         {
//             Debug.LogError("No user is currently signed in to verify email.");
//         }
//     }

// }
