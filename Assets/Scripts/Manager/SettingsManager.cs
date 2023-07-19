
using UnityEngine;

public class SettingsManager: MonoBehaviour
{
    public static SettingsManager Instance;
    
    public float generalVolume = 1f;
    public float musicVolume = 0.5f;
    public float soundEffectVolume = 1f;
    public bool soundMuted;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
