// using UnityEngine;
// using Firebase.Auth;
// using Firebase.Extensions;
// using System;
// using TMPro;
// using UnityEngine.UI;
// using Unity.Services.Authentication;


// public class FirebaseManager : MonoBehaviour
// {
//     public Action<string> onErrorShow;
//     Firebase.Auth.FirebaseAuth auth;

//     [Header("UI Elements")]
//     [SerializeField] private UIElemensPanel signUpPanel;
//     [SerializeField] private UIElemensPanel signInPanel;

//     [SerializeField] private GameObject loadingSpinner;

//     [SerializeField] private GameObject signUpPage;
//     [SerializeField] private GameObject verifyCodePage;
//     [SerializeField] private GameObject signInPage;
//     [SerializeField] private GameObject resetPasswordPage;

//     [SerializeField] private Button verifyEmail;
//     [SerializeField] private Button resendVerifyEmail;
//     [SerializeField] private Button signOut;
//     [SerializeField] private Button resetPassword;

//     [SerializeField] private TMP_InputField emailText;

//     [SerializeField] private AnimationBox animationBox;

//     [Header("Login Auth")]
//     [SerializeField] private LoginController loginController;

//     private string Email{ get; set; }
//     private string Password { get; set; }

//     void Start()
//     {
//         auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
//         Debug.Log("Firebase Auth initialized.");
//         signUpPanel.signUpButton.onClick.AddListener(() => SignUp(signUpPanel.emailField.text, signUpPanel.passwordField.text, signUpPanel.confirmPasswordField.text));

//         verifyEmail.onClick.AddListener(() =>
//         {
//             CheckEmailVerification();
//         });

//         resendVerifyEmail.onClick.AddListener(() =>
//         {
//             FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
//             user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
//             {
//                 if (task.IsCompleted)
//                 {
//                     Debug.Log("Verification email resent.");
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to resend verification email.");
//                 }
//             });
//         });

//         signOut.onClick.AddListener(() =>
//         {
//             auth.SignOut();
//             verifyCodePage.SetActive(false);
//             signUpPage.SetActive(true);
//             Debug.Log("User signed out.");
//         });

//         if (signInPanel.signInButton == null)
//         {
//             Debug.LogError("SignIn button is not assigned in the inspector.");
//             return;
//         }

//         signInPanel.signInButton.onClick.AddListener(() => SignIn(signInPanel.emailField.text, signInPanel.passwordField.text));

//         resetPassword.onClick.AddListener(() =>
//         {
//             string email = emailText.text;
//             ForgetPassword(email);
//         });
//     }
//     public void SignUp(string mail,string password,string confirmPassword)
//     {
//         Debug.Log("SignUp called with email: " + mail);
//         loadingSpinner.SetActive(true);

//         if (string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
//         {
//             Debug.Log("Email or password is empty");
//             onErrorShow?.Invoke("Email or password is empty");

//             loadingSpinner.SetActive(false);
//             return;
//         }
//         if (!ValidateEmail(mail))
//         {
//             Debug.Log("Invalid email format");
//             onErrorShow?.Invoke("Invalid email format");

//             loadingSpinner.SetActive(false);
//             return;
//         }
//         if (password != confirmPassword)
//         {
//             Debug.Log("Passwords do not match");
//             onErrorShow?.Invoke("Passwords do not match");

//             loadingSpinner.SetActive(false);
//             return;
//         }

//         if(password.Length < 8)
//         {
//             Debug.Log("Password must be at least 8 characters long");
//             onErrorShow?.Invoke("Password must be at least 8 characters long");
//             loadingSpinner.SetActive(false);
//             return;
//         }

//         if(!ValidatePassword(password))
//         {
//             Debug.Log("Password must contain at least one uppercase letter, one lowercase letter, and one number");
//             onErrorShow?.Invoke("Password must contain at least one uppercase letter, one lowercase letter, and one number");
//             loadingSpinner.SetActive(false);
//             return;
//         }

//         auth.CreateUserWithEmailAndPasswordAsync(mail, password).ContinueWithOnMainThread(task =>
//         {
//             if (task.IsFaulted)
//             {
//                 Debug.Log("Error creating user: " + task.Exception);
//                 onErrorShow?.Invoke("Error creating user: " + task.Exception.Message);

//                 loadingSpinner.SetActive(false);
//             }
//             else if (task.IsCompleted)
//             {
//                 try
//                 {
//                     Firebase.Auth.AuthResult authResult = task.Result;
//                     FirebaseUser newUser = authResult.User; // Correctly access the FirebaseUser from AuthResult
//                     Debug.Log($"User created: {newUser.Email}");

//                     Email = mail;
//                     Password = password;

//                     loadingSpinner.SetActive(false);

//                     loginController.InitSignUpWithUsernameAndPassword(mail, password);
//                     animationBox.OnClose();
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError("Error: " + e.Message);
//                     onErrorShow?.Invoke("Error: " + e.Message);
//                 }
//             }
//         });
//     }
//     public void SignIn(string main,string password)
//     {
//         loadingSpinner.SetActive(true);

//         if (string.IsNullOrEmpty(main) || string.IsNullOrEmpty(password))
//         {
//             Debug.Log("Email or password is empty");
//             onErrorShow?.Invoke("Email or password is empty");
//             loadingSpinner.SetActive(false);
//             return;
//         }
//         if(!ValidateEmail(main))
//         {
//             Debug.Log("Invalid email format");
//             onErrorShow?.Invoke("Invalid email format");
//             loadingSpinner.SetActive(false);
//             return;
//         }

//         auth.SignInWithEmailAndPasswordAsync(main, password).ContinueWithOnMainThread(task =>
//         {
//             if (task.IsFaulted)
//             {
//                 Debug.Log("Error signing in: " + task.Exception);
//                 onErrorShow?.Invoke("Error has happened, please try again");
//             }
//             else
//             {
//                 Debug.Log("User signed in successfully");

//                 // Handle successful sign-in here

//                 animationBox.OnClose();
//                 loginController.InitSignInWithUsernameAndPassword(main, password);
                
//             }
//         });
//         loadingSpinner.SetActive(false);

//     }
//     public void CheckEmailVerification()
//     {
//         FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

//         user.ReloadAsync().ContinueWithOnMainThread(task =>
//         {
//             if (task.IsFaulted || task.IsCanceled)
//             {
//                 Debug.LogError("Reload failed.");
//                 return;
//             }

//             if (user.IsEmailVerified)
//             {
//                 Debug.Log("Email verified! Proceeding...");

//                 // Link Account 
//                 if (AuthenticationService.Instance.IsSignedIn)
//                 {
//                     Debug.Log("User is already signed in with a different provider. Linking account...");
//                     loginController.InitLinkAccountWithUsernamePassword(Email, Password);
//                 }
//                 else
//                 {
//                     loginController.InitSignUpWithUsernameAndPassword(Email, Password);
//                 }
//                 animationBox.OnClose();
//             }
//             else
//             {
//                 Debug.Log("Email is not verified yet.");
//             }
//         });
//     }
//     public void ForgetPassword(string email)
//     {
//         if (string.IsNullOrEmpty(email))
//         {
//             object error = "Email is empty";
//             onErrorShow?.Invoke(error.ToString());
//             return;
//         }

//         if (!ValidateEmail(email))
//         {
//             object error = "Invalid email format";
//             onErrorShow?.Invoke(error.ToString());
//             return;
//         }
//         auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
//         {
//             if (task.IsCanceled || task.IsFaulted)
//             {
//                 Debug.LogError("Error sending password reset email: " + task.Exception);
//                 onErrorShow?.Invoke("Error sending password reset email: " + task.Exception);
//                 return;
//             }
//             else
//             {
//                 onErrorShow?.Invoke("Password reset email sent successfully. Please check your inbox.");
//                 resetPasswordPage.SetActive(false);
//                 signInPage.SetActive(true);
//             }

//         });
//     }
//     private void OnApplicationQuit()
//     {
//         if(auth != null)
//         {
//             Debug.Log("Application is quitting, signing out user.");
//             auth.SignOut();
//         }
//         loginController.InitSignOut();
//     }
//     private void OnDestroy()
//     {
//         signUpPanel.signUpButton.onClick.RemoveListener(() => SignUp(signUpPanel.emailField.text, signUpPanel.passwordField.text, signUpPanel.confirmPasswordField.text));
//         signInPanel.signInButton.onClick.RemoveListener(() => SignIn(signInPanel.emailField.text, signInPanel.passwordField.text));

//         verifyEmail.onClick.RemoveListener(() =>
//         {
//             CheckEmailVerification();
//         });

//         resendVerifyEmail.onClick.RemoveListener(() =>
//         {
//             FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
//             user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
//             {
//                 if (task.IsCompleted)
//                 {
//                     Debug.Log("Verification email resent.");
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to resend verification email.");
//                 }
//             });
//         });

//         signOut.onClick.RemoveListener(() =>
//         {
//             auth.SignOut();
//             verifyCodePage.SetActive(false);
//             signUpPage.SetActive(true);
//             Debug.Log("User signed out.");
//         });

//         resetPassword.onClick.RemoveAllListeners();
//     }
//     private bool ValidateEmail(string input)
//     {
//         return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
//     }
//     private bool ValidatePassword(string input)
//     {
//         return System.Text.RegularExpressions.Regex.IsMatch(input, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[*$%@&#])[a-zA-Z\d*$%@&#]{8,30}$");
//     }
// }
// [Serializable]
// public class  UIElemensPanel
// {
//     public TMP_InputField emailField;
//     public TMP_InputField passwordField;
//     public TMP_InputField confirmPasswordField;

//     public Button signUpButton;
//     public Button signInButton;
// }
