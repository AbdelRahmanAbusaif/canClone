using UnityEngine;
using UnityEngine.UI;

public class NavigationBarButton : MonoBehaviour
{
    [SerializeField] private GameObject targetObjectPage;
    [SerializeField] private Image imageButton;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(targetObjectPage.activeInHierarchy)
        {
            GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
            imageButton.color = Color.gray;
        }
        else
        {
            GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            imageButton.color = Color.black;
        }
    }
}
