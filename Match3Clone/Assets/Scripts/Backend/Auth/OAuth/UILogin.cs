using System;
using System.Collections.Generic;
using GameVanilla.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public static Action OnSignIn;
    [SerializeField] private Button signInButton;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private LoginController loginController;
    [SerializeField] private GameObject loginPanel;

    private void OnEnable() {

        if(signInButton !=null)
        {
            signInButton.onClick.AddListener(OnSignInButtonClicked);
        }

        loginController.OnSignInSuccess += OnSignInSuccess;
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
        
        int anonymousValue = PlayerPrefs.GetInt("IsAnonymous");
        if(anonymousValue == 0)
        {
            loginPanel.SetActive(true);
        }
        else
        {
            sceneTransition.PerformTransition();
        }
    }

    // Update is called once per frame
    private void OnDisable() {

        if(signInButton !=null)
        {
            signInButton.onClick.RemoveListener(OnSignInButtonClicked);
        }
        loginController.OnSignInSuccess -= OnSignInSuccess;
    }

}