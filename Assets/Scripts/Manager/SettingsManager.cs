using UnityEngine;

public class SettingsManager: MonoBehaviour
{
    public static SettingsManager Instance;
    
    public SettingsFrames currentCategoryTab = SettingsFrames.Sounds;
    
    public bool darkModeOn;
    
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
    
    public int resolution;
    public bool fullscreen = true;
    public bool vsync = true;
    public Language language = Language.English;

    private void Awake()
    {
        Load();
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public static void SetUpSound(Transform camera) =>
        camera.GetComponent<AudioSource>().volume =
            Instance.musicVolume * Instance.generalVolume / 2;

    private void Load()
    {
        var settings = SaveAndLoadSystem.LoadSettings();

        if (settings == null)
        {
            resolution = Utils.ResolutionTupleToIndex(Screen.currentResolution);
            fullscreen = true;
            vsync = QualitySettings.vSyncCount > 0;
        }
        else
        {
            generalVolume = settings.generalVolume;
            musicVolume = settings.musicVolume;
            soundEffectVolume = settings.soundEffectVolume;
            
            jumpKeyCode = (KeyCode)settings.jumpKeyCode;
            moveLeftKeyCode = (KeyCode)settings.moveLeftKeyCode;
            moveRightKeyCode = (KeyCode)settings.moveRightKeyCode;
            fireKeyCode = (KeyCode)settings.fireKeyCode;
            speedBuffKeyCode = (KeyCode)settings.speedBuffKeyCode;
            jumpBuffKeyCode = (KeyCode)settings.jumpBuffKeyCode;
            pauseKeyCode = (KeyCode)settings.pauseKeyCode;
            
            resolution = settings.resolution;
            fullscreen = settings.fullscreen;
            vsync = settings.vsync;
            language = (Language)settings.language;
        }
        
        var resolutionAsTuple = Utils.ResolutionIndexToTuple(resolution);
        Screen.SetResolution(resolutionAsTuple.Item1, resolutionAsTuple.Item2, fullscreen);
    }

    public void Save() =>
        SaveAndLoadSystem.SaveSettings(this);
}
