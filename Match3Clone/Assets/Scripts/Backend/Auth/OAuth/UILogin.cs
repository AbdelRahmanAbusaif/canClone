using System;
using GameVanilla.Core;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button signInButton;
    [SerializeField] private LoginController loginController;

    private SceneTransition sceneTransition;
    private void OnEnable() {
        signInButton.onClick.AddListener(OnSignInButtonClicked);
        loginController.OnSignInSuccess += OnSignInSuccess;

        sceneTransition = GetComponent<SceneTransition>();
    }

    private async void OnSignInButtonClicked()
    {
        Debug.Log("Sign in button clicked");
        await loginController.InitSign();
    }

    private void OnSignInSuccess(PlayerInfo info, string arg2)
    {
        Debug.Log("Sign in success");

        // Here will be the code for get the player info and save it to the database
    }

    // Update is called once per frame
    private void OnDisable() {
        signInButton.onClick.RemoveListener(OnSignInButtonClicked);
        loginController.OnSignInSuccess -= OnSignInSuccess;
    }
}