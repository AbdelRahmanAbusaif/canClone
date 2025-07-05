using Unity.Services.Authentication;
using UnityEngine;

public class CheckLogin : MonoBehaviour
{
    [SerializeField] private GameObject uiLogin;
    void Start()
    {
        if(AuthenticationService.Instance.IsSignedIn)
        {
			Debug.Log("User is already signed in.");
			uiLogin.SetActive(true);
		}
		else
		{
			Debug.Log("User is not signed in, showing login UI.");
			uiLogin.SetActive(false);
		}
    }
}
