using UnityEngine;

public class TermOfServicePage : MonoBehaviour
{
    [SerializeField] private GameObject LoginPanel;    
    public void OnAcceptButtonClicked()
    {
        Debug.Log("Accept button clicked.");
        LoginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnDeclineButtonClicked()
    {
        Debug.Log("Decline button clicked.");
        Application.Quit();
    }
    public void OnOpenWebButtonClicked()
    {
        Debug.Log("Open web button clicked.");
        Application.OpenURL("https://lavender-weasel-165279.hostingersite.com/privacy-policy-2/");
    }
}
