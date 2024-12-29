using System;
using System.Collections.Generic;
using GameVanilla.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Friends.Models;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public static Action OnSignIn;
    [SerializeField] private Button signInButton;
    [SerializeField] private LoginController loginController;
    [SerializeField] private GameObject loginPanel;

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

    private async void OnSignInSuccess(PlayerProfile playerData)
    {
        Debug.Log("Sign in success");
        // Here will be the code for get the player info and save it to the database
        
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PlayerProfile" });
        if(data.ContainsKey("PlayerProfile"))
        {
            Debug.Log("Player profile already exists");

            OnSignIn?.Invoke();
            return;
        }
        loginPanel.SetActive(true);
    }

    // Update is called once per frame
    private void OnDisable() {
        signInButton.onClick.RemoveListener(OnSignInButtonClicked);
        loginController.OnSignInSuccess -= OnSignInSuccess;
    }

}