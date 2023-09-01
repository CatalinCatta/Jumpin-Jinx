using System;

/// <summary>
/// Represents the settings model for the game.
/// </summary>
[Serializable]
public class SettingsModel
{
    public bool darkModeOn;

    #region Volume
    public float generalVolume;
    public float musicVolume;
    public float soundEffectVolume;
    #endregion

    #region KeyCode
    public int jumpKeyCode;
    public int moveLeftKeyCode;
    public int moveRightKeyCode;
    public int fireKeyCode;
    public int speedBuffKeyCode;
    public int jumpBuffKeyCode;
    public int pauseKeyCode;
    #endregion

    #region Display
    public int resolution;
    public bool fullscreen;
    public bool vsync;
    public int language;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsModel"/> class.
    /// </summary>
    /// <param name="settingsManager">The settings manager to get the values from.</param>
    public SettingsModel(SettingsManager settingsManager)
    {
        darkModeOn = settingsManager.DarkModeOn;

        generalVolume = settingsManager.GeneralVolume;
        musicVolume = settingsManager.MusicVolume;
        soundEffectVolume = settingsManager.SoundEffectVolume;

        jumpKeyCode = (int)settingsManager.JumpKeyCode;
        moveLeftKeyCode = (int)settingsManager.MoveLeftKeyCode;
        moveRightKeyCode = (int)settingsManager.MoveRightKeyCode;
        fireKeyCode = (int)settingsManager.FireKeyCode;
        speedBuffKeyCode = (int)settingsManager.SpeedBuffKeyCode;
        jumpBuffKeyCode = (int)settingsManager.JumpBuffKeyCode;
        pauseKeyCode = (int)settingsManager.PauseKeyCode;

        resolution = settingsManager.Resolution;
        fullscreen = settingsManager.Fullscreen;
        vsync = settingsManager.Vsync;
        language = (int)settingsManager.Language;
    }
}