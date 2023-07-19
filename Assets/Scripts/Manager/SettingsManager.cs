
using UnityEngine;

public class SettingsManager: MonoBehaviour
{
    public static SettingsManager Instance;
   
    public SettingsFrames currentCategoryTab = SettingsFrames.Sounds;
    
    public float generalVolume = 1f;
    public float musicVolume = 0.5f;
    public float soundEffectVolume = 1f;

    public KeyCode jumpKeyCode = KeyCode.W;
    public KeyCode moveLeftKeyCode = KeyCode.A;
    public KeyCode moveRightKeyCode = KeyCode.D;
    public KeyCode fireKeyCode = KeyCode.Space;
    public KeyCode speedBuffKeyCode = KeyCode.LeftAlt;
    public KeyCode jumpBuffKeyCode = KeyCode.LeftControl;
    public KeyCode pauseKeyCode = KeyCode.Escape;

    public int resolution = 15;
    public bool fullscreen = true;
    public bool vsync = true;
    public float brightness = 0.5f;
    public Language language = Language.English;
    
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
