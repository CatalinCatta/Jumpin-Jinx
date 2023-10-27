using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages game settings and options.
/// </summary>
public class SettingsManager : IndestructibleManager<SettingsManager>
{
    [NonSerialized] public float GeneralVolume, MusicVolume, SoundEffectVolume;

    [NonSerialized] public KeyCode JumpKeyCode,
        MoveLeftKeyCode,
        MoveRightKeyCode,
        FireKeyCode,
        SpeedBuffKeyCode,
        JumpBuffKeyCode,
        PauseKeyCode;

    [NonSerialized] public int Resolution;
    [NonSerialized] public bool Fullscreen, Vsync;
    [NonSerialized] public Language Language;

    [NonSerialized] public string LastMenuName;
    [NonSerialized] public SettingsFrames CurrentCategoryTab;
    [NonSerialized] public bool DarkModeOn, IsMenuOpened;
    
    protected override void Awake()
    {
        Load();
        base.Awake();
    }

    private void Update()
    {
        var resolution = Screen.currentResolution;
        Resolution = Utility.ConvertResolutionTupleToIndex(resolution);
    }

    public void SwitchMenuState(Toggle newState) => IsMenuOpened = newState.isOn;
    
    /// <summary>
    /// Sets up sound volume for the specified cameraTransform.
    /// </summary>
    /// <param name="cameraTransform">The cameraTransform to adjust the sound volume for.</param>
    public static void SetUpSound(Transform cameraTransform) => cameraTransform.GetComponent<AudioSource>().volume =
        Instance.MusicVolume * Instance.GeneralVolume / 2;

    private void Load()
    {
        CurrentCategoryTab = SettingsFrames.Sounds;
        var settings = SaveAndLoadSystem.LoadSettings();

        if (settings == null)
            Initialize();
        else
        {
            DarkModeOn = settings.darkModeOn;
            IsMenuOpened = settings.isMenuOpened;
            
            GeneralVolume = settings.generalVolume;
            MusicVolume = settings.musicVolume;
            SoundEffectVolume = settings.soundEffectVolume;

            JumpKeyCode = (KeyCode)settings.jumpKeyCode;
            MoveLeftKeyCode = (KeyCode)settings.moveLeftKeyCode;
            MoveRightKeyCode = (KeyCode)settings.moveRightKeyCode;
            FireKeyCode = (KeyCode)settings.fireKeyCode;
            SpeedBuffKeyCode = (KeyCode)settings.speedBuffKeyCode;
            JumpBuffKeyCode = (KeyCode)settings.jumpBuffKeyCode;
            PauseKeyCode = (KeyCode)settings.pauseKeyCode;

            Resolution = settings.resolution;
            Fullscreen = settings.fullscreen;
            Vsync = settings.vsync;
            Language = (Language)settings.language;
        }

        var resolutionAsTuple = Utility.ConvertResolutionIndexToTuple(Resolution);
        Screen.SetResolution(resolutionAsTuple.Item1, resolutionAsTuple.Item2, Fullscreen);
    }

    public void Save() => SaveAndLoadSystem.SaveSettings(this);

    public void ResetSettings()
    {
        SaveAndLoadSystem.DeleteSettingsSave();
        Initialize();
    }

    private void Initialize()
    {
        DarkModeOn = false;
        IsMenuOpened = true;

        GeneralVolume = 1f;
        MusicVolume = 0.5f;
        SoundEffectVolume = 1f;

        JumpKeyCode = KeyCode.W;
        MoveLeftKeyCode = KeyCode.A;
        MoveRightKeyCode = KeyCode.D;
        FireKeyCode = KeyCode.Space;
        SpeedBuffKeyCode = KeyCode.LeftAlt;
        JumpBuffKeyCode = KeyCode.LeftControl;
        PauseKeyCode = KeyCode.P;

        Resolution = Utility.ConvertResolutionTupleToIndex(Screen.currentResolution);
        Fullscreen = true;
        Vsync = QualitySettings.vSyncCount > 0;
        Language = Language.English;
    }

    public static void ChangeBackground(Transform background)  // TODO: Uncomment when added dark background.
    {
        // background.GetChild(0).gameObject.SetActive(!Instance.DarkModeOn);
        // background.GetChild(1).gameObject.SetActive(!Instance.DarkModeOn);
    }
}