// using UnityEngine;

// using Facebook.Unity;
// using System.Collections.Generic;

// public class FacebookManager : MonoBehaviour
// {
//     public string Token;
//     public string Error;
//     public static FacebookManager Instance;

//     // Awake function from Unity's MonoBehaviour
//     public void Awake()
//     {
//         if (!FB.IsInitialized)
//         {
//             // Initialize the Facebook SDK
//             FB.Init(InitCallback, OnHideUnity);
//         }
//         else
//         {
//             // Already initialized, signal an app activation App Event
//             FB.ActivateApp();
//         }
//     }

//     void InitCallback()
//     {
//         if (FB.IsInitialized)
//         {
//             // Signal an app activation App Event
//             FB.ActivateApp();
//             // Continue with Facebook SDK
//         }
//         else
//         {
//             Debug.Log("Failed to Initialize the Facebook SDK");
//         }
//     }

//     void OnHideUnity(bool isGameShown)
//     {
//         if (!isGameShown)
//         {
//             // Pause the game - we will need to hide
//             Time.timeScale = 0;
//         }
//         else
//         {
//             // Resume the game - we're getting focus again
//             Time.timeScale = 1;
//         }
//     }

//     public void Login()
//     {
//         // Define the permissions
//         var perms = new List<string>() { "public_profile", "email" };

//         FB.LogInWithReadPermissions(perms, result =>
//         {
//             if (FB.IsLoggedIn)
//             {
//                 Token = AccessToken.CurrentAccessToken.TokenString;
//                 Debug.Log($"Facebook Login token: {Token}");
//             }
//             else
//             {
//                 Error = "User cancelled login";
//                 Debug.Log("[Facebook Login] User cancelled login");
//             }
//         });
//     }
// }
