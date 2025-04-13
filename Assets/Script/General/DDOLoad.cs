using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOLoad : MonoBehaviour
{
    private static DDOLoad instance;

    public bool gameInit;

    public static DDOLoad Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.activeSceneChanged += (s, a) =>
            {
                if (TryGetComponent<Canvas>(out var canvas))
                {
                    canvas.worldCamera = Camera.main;
                }
            };
            DontDestroyOnLoad(gameObject); // Giữ object khi chuyển scene
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy object mới
        }
    }

    private void OnApplicationQuit()
    {
        
    }
}
