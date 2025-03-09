using System;
using UnityEngine;

public class EventInvoker : MonoBehaviour
{
    private static EventInvoker _instance;

    public static EventInvoker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<EventInvoker>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<EventInvoker>();
                    singletonObject.name = typeof(EventInvoker).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    public event Action<string> OnUpdateText;
    public event Action<Sprite> OnUpdateImage;

    
}