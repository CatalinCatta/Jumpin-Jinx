using System;
using UnityEngine;

public abstract class IndestructibleManager : MonoBehaviour
{
    [Header("Singleton Instance")] [NonSerialized]
    public static IndestructibleManager Instance;
    
    private void Awake()
    {
        DoSomethingAtAwakeBeginning();
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Override it if u need to call something at the beginning of <see cref="Awake"/>. 
    /// </summary>
    protected virtual void DoSomethingAtAwakeBeginning(){}
}