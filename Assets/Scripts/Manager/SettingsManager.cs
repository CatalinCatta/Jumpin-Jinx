using UnityEngine;

public class SettingsManager: MonoBehaviour
{
    public static SettingsManager Instance;
    
    public SettingsFrames currentCategoryTab = SettingsFrames.Sounds;
    
    public bool darkModeOn;
    
    public float generalVolume;
    public float musicVolume;
    public float soundEffectVolume;
    
    public KeyCode jumpKeyCode;
    public KeyCode moveLeftKeyCode;
    public KeyCode moveRightKeyCode;
    public KeyCode fireKeyCode;
    public KeyCode speedBuffKeyCode;
    public KeyCode jumpBuffKeyCode;
    public KeyCode pauseKeyCode;
    
    public int resolution;
    public bool fullscreen;
    public bool vsync;
    public Language language;

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
            Initialize();
        else
        {
            darkModeOn = settings.darkModeOn;
                
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
    
    public void ResetSettings() 
    {
        SaveAndLoadSystem.DeleteSettingsSave();
        Initialize();
    }

    private void Initialize()
    {
        darkModeOn = false;
        generalVolume = 1f;
        musicVolume = 0.5f;
        soundEffectVolume = 1f;
            
        jumpKeyCode = KeyCode.W;
        moveLeftKeyCode = KeyCode.A;
        moveRightKeyCode = KeyCode.D;
        fireKeyCode = KeyCode.Space;
        speedBuffKeyCode = KeyCode.LeftAlt;
        jumpBuffKeyCode = KeyCode.LeftControl;
        pauseKeyCode = KeyCode.Escape;
        
        resolution = Utils.ResolutionTupleToIndex(Screen.currentResolution);
        fullscreen = true;
        vsync = QualitySettings.vSyncCount > 0;
        language = Language.English;
    }
    
    public static void ChangeBackground(Transform background)
    {
        if (!Instance.darkModeOn) return;
      
        foreach (Transform backgroundImg in background)
            backgroundImg.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0.5f);
    }
}
