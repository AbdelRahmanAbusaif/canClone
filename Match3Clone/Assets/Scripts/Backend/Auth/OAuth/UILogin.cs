using System;
using GameVanilla.Core;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button signInButton;
    [SerializeField] private LoginController loginController;
    [SerializeField] private GameObject loginPanel;

    private SceneTransition sceneTransition;
    private PlayerInfo playerInfo;
    private string playerName;
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

    private void OnSignInSuccess(PlayerProfile playerData)
    {
        Debug.Log("Sign in success");
        // Here will be the code for get the player info and save it to the database
        
        loginPanel.SetActive(true);
    }

    // Update is called once per frame
    private void OnDisable() {
        signInButton.onClick.RemoveListener(OnSignInButtonClicked);
        loginController.OnSignInSuccess -= OnSignInSuccess;
    }

}