using UnityEngine;
using UnityEngine.UI;

public class KeyboardHandler : MonoBehaviour
{
    public RectTransform panel; // Reference to the panel's RectTransform
    public float keyboardHeight = 300f; // Adjust this value based on your keyboard height
    public bool active=false;
    void Update()
    {
        if (TouchScreenKeyboard.visible ||active )
        {
            // Move the panel up when the keyboard is visible
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, keyboardHeight);
        }
        else
        {
            // Move the panel back to its original position when the keyboard is hidden
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, 0);
        }
    }
}
