using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

using System;
using TMPro;

public class updatItem_level : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TMP_Text txt;
    void Awake()
    {
        txt = GetComponent<TMP_Text>();
        int currentValue = PlayerPrefs.GetInt("itemCollected", 0);
        txt.text = currentValue.ToString();
        Debug.Log(currentValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
