using UnityEngine;
using UnityEngine.UI;

using System;
using TMPro;
using GameVanilla.Game.Common;


public class updatItemColl : MonoBehaviour
{

    private TMP_Text txt;
    [SerializeField] private FxPool fxPool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        txt = GetComponent<TMP_Text>();
        txt.text = PlayerPrefs.GetInt("itemCollected").ToString();
    }
    private void Start() {
        fxPool.OnItemCollected += OnItemCollected;
    }

    // just will be called after collecting an item
    private void OnItemCollected()
    {
        int currentValue = PlayerPrefs.GetInt("itemCollected");
        txt.text = currentValue.ToString();
        Debug.Log(currentValue);
    }
   
}
