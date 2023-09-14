using System;
using UnityEngine;

public abstract class IndestructibleManager<T> : MonoBehaviour where T : IndestructibleManager<T>
{
    [Header("Singleton Instance")] [NonSerialized]
    public static T Instance;
    
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(Instance!.gameObject);
    }
}