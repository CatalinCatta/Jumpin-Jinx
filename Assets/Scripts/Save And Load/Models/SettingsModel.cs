using System;

[Serializable]
public class SettingsModel
{
    public bool darkModeOn;
    
    public float generalVolume;
    public float musicVolume;
    public float soundEffectVolume;
    
    public int jumpKeyCode;
    public int moveLeftKeyCode;
    public int moveRightKeyCode;
    public int fireKeyCode;
    public int speedBuffKeyCode;
    public int jumpBuffKeyCode;
    public int pauseKeyCode;
    
    public int resolution;
    public bool fullscreen;
    public bool vsync;
    public int language;

    public SettingsModel(SettingsManager settingsManager)
    {
        darkModeOn = settingsManager.darkModeOn;

        generalVolume = settingsManager.generalVolume;
        musicVolume = settingsManager.musicVolume;
        soundEffectVolume = settingsManager.soundEffectVolume;

        jumpKeyCode = (int)settingsManager.jumpKeyCode;
        moveLeftKeyCode = (int)settingsManager.moveLeftKeyCode;
        moveRightKeyCode = (int)settingsManager.moveRightKeyCode;
        fireKeyCode = (int)settingsManager.fireKeyCode;
        speedBuffKeyCode = (int)settingsManager.speedBuffKeyCode;
        jumpBuffKeyCode = (int)settingsManager.jumpBuffKeyCode;
        pauseKeyCode = (int)settingsManager.pauseKeyCode;
        
        resolution = settingsManager.resolution;
        fullscreen = settingsManager.fullscreen;
        vsync = settingsManager.vsync;
        language = (int)settingsManager.language;
    }
}