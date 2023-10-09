using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// Manages the settings menu and user input for changing settings.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Transform sounds, controls, display; 
    [SerializeField] private SettingsManager settingsManager;
    
    #region Current Pressed Key
    private bool 
        _isCheckingForJump,
        _isCheckingForMoveLeft,
        _isCheckingForMoveRight,
        _isCheckingForFire,
        _isCheckingForSpeedBuff,
        _isCheckingForJumpBuff,
        _isCheckingForPause;
    #endregion
    
    #region Sounds
    private Transform
        _generalSoundTransform,
        _musicTransform,
        _sfxTransform;
    #endregion

    #region KeyCodes
    private TextMeshProUGUI
        _jumpKeyText,
        _moveLeftKeyText,
        _moveRightKeyText,
        _fireKeyText,
        _speedBuffKeyText,
        _jumpBuffKeyText,
        _pauseKeyText;
    #endregion

    #region Display
    private TMP_Dropdown 
        _resolution,
        _language;
    private Transform 
        _fullScreenTransform,
        _vSyncTransform;
    #endregion

    private void Awake()
    {
        _generalSoundTransform = sounds.GetChild(0); 
        _musicTransform = sounds.GetChild(1); 
        _sfxTransform = sounds.GetChild(2);

        _jumpKeyText = GetTextComponentForKey(0);
        _moveLeftKeyText = GetTextComponentForKey(1);
        _moveRightKeyText = GetTextComponentForKey(2);
        _fireKeyText = GetTextComponentForKey(3);
        _speedBuffKeyText = GetTextComponentForKey(4);
        _jumpBuffKeyText = GetTextComponentForKey(5);
        _pauseKeyText = GetTextComponentForKey(6);
        
        _resolution = GetDisplayChild(0).GetComponent<TMP_Dropdown>();
        _fullScreenTransform = GetDisplayChild(1);
        _vSyncTransform = GetDisplayChild(2);
        _language = GetDisplayChild(3).GetComponent<TMP_Dropdown>();

        TextMeshProUGUI GetTextComponentForKey(int keyId) =>
            controls.GetChild(keyId).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        Transform GetDisplayChild(int childId) => display.GetChild(childId).GetChild(1);
    }

    private void Start()
    {
        var resolution = Screen.currentResolution;
        Screen.SetResolution(resolution.width, resolution.height, settingsManager.Fullscreen);

        switch (settingsManager.CurrentCategoryTab)
        {
            case SettingsFrames.Sounds:
                ShowSounds();
                break;
            case SettingsFrames.Controls:
                ShowControls();
                break;
            case SettingsFrames.Display:
                ShowDisplay();
                break;
        }
    }

    private void Update()
    {
        if (!_isCheckingForJump && !_isCheckingForMoveLeft && !_isCheckingForMoveRight && !_isCheckingForFire &&
            !_isCheckingForSpeedBuff && !_isCheckingForJumpBuff && !_isCheckingForPause)
            return;

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKeyDown(keyCode) || KeyAlreadyInUse(keyCode))
                continue;

            if (_isCheckingForJump)
            {
                SetUpKeyCodeText(_jumpKeyText, KeyCodeInTextFormat(keyCode));
                settingsManager.JumpKeyCode = keyCode;
                _isCheckingForJump = false;
                return;
            }

            if (_isCheckingForMoveLeft)
            {
                SetUpKeyCodeText(_moveLeftKeyText, KeyCodeInTextFormat(keyCode));
                settingsManager.MoveLeftKeyCode = keyCode;
                _isCheckingForMoveLeft = false;
                return;
            }

            if (_isCheckingForMoveRight)
            {
                SetUpKeyCodeText(_moveRightKeyText, KeyCodeInTextFormat(keyCode));
                settingsManager.MoveRightKeyCode = keyCode;
                _isCheckingForMoveRight = false;
                return;
            }

            if (_isCheckingForFire)
            {
                SetUpKeyCodeText(_fireKeyText, KeyCodeInTextFormat(keyCode));
                settingsManager.FireKeyCode = keyCode;
                _isCheckingForFire = false;
                return;
            }

            if (_isCheckingForSpeedBuff)
            {
                SetUpKeyCodeText(_speedBuffKeyText, KeyCodeInTextFormat(keyCode));
                settingsManager.SpeedBuffKeyCode = keyCode;
                _isCheckingForSpeedBuff = false;
                return;
            }

            if (_isCheckingForJumpBuff)
            {
                SetUpKeyCodeText(_jumpBuffKeyText, KeyCodeInTextFormat(keyCode));
                settingsManager.JumpBuffKeyCode = keyCode;
                _isCheckingForJumpBuff = false;
                return;
            }

            if (_isCheckingForPause)
            {
                SetUpKeyCodeText(_pauseKeyText, KeyCodeInTextFormat(keyCode));
                settingsManager.PauseKeyCode = keyCode;
                _isCheckingForPause = false;
                return;
            }
        }
    }
    
    private static void SetUpKeyCodeText(TMP_Text keyCodeText, string text) => keyCodeText.text = text;

    private static string KeyCodeInTextFormat(KeyCode keyCode) =>
        Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
    
    private static void SetUpSoundText(Transform soundTransform, float volume) =>
        soundTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(volume * 100) + "%";

  
    /// <summary>
    /// Checks if a key code is already in use by another setting.
    /// </summary>
    /// <param name="keyCode">KeyCode to check.</param>
    /// <returns>Bool representing if key was already used.</returns>
    private bool KeyAlreadyInUse(KeyCode keyCode)  // TODO: Create unity animation to show wrong decision.
    {
        var allKeyCodes = new List<KeyCode>
        {
            settingsManager.JumpKeyCode,
            settingsManager.MoveLeftKeyCode,
            settingsManager.MoveRightKeyCode,
            settingsManager.FireKeyCode,
            settingsManager.SpeedBuffKeyCode,
            settingsManager.JumpBuffKeyCode,
            settingsManager.PauseKeyCode
        };

        if (_isCheckingForJump)
            allKeyCodes.RemoveAt(0);
        if (_isCheckingForMoveLeft)
            allKeyCodes.RemoveAt(1);
        if (_isCheckingForMoveRight)
            allKeyCodes.RemoveAt(2);
        if (_isCheckingForFire)
            allKeyCodes.RemoveAt(3);
        if (_isCheckingForSpeedBuff)
            allKeyCodes.RemoveAt(4);
        if (_isCheckingForJumpBuff)
            allKeyCodes.RemoveAt(5);
        if (_isCheckingForPause)
            allKeyCodes.RemoveAt(6);

        return allKeyCodes.Contains(keyCode);
    }
    
    #region Show Category
    
    /// <summary>
    /// Set up the sounds volume.
    /// </summary>
    public void ShowSounds()
    {
        sounds.gameObject.SetActive(true);
        controls.gameObject.SetActive(false);
        display.gameObject.SetActive(false);

        SetUpSound(_generalSoundTransform, settingsManager.GeneralVolume);
        SetUpSound(_musicTransform, settingsManager.MusicVolume);
        SetUpSound(_sfxTransform, settingsManager.SoundEffectVolume);
        
        void SetUpSound(Transform soundTransform, float volume)
        {
            soundTransform.GetChild(1).GetComponent<Slider>().value = volume;
            SetUpSoundText(soundTransform, volume);
        }
    }
    
    /// <summary>
    /// Set up the keybindings.
    /// </summary>
    public void ShowControls()
    {
        controls.gameObject.SetActive(true);
        sounds.gameObject.SetActive(false);
        display.gameObject.SetActive(false);
     
        SetUpKeyCodeText(_jumpKeyText, KeyCodeInTextFormat(settingsManager.JumpKeyCode));
        SetUpKeyCodeText(_moveLeftKeyText, KeyCodeInTextFormat(settingsManager.MoveLeftKeyCode));
        SetUpKeyCodeText(_moveRightKeyText, KeyCodeInTextFormat(settingsManager.MoveRightKeyCode));
        SetUpKeyCodeText(_fireKeyText, KeyCodeInTextFormat(settingsManager.FireKeyCode));
        SetUpKeyCodeText(_speedBuffKeyText, KeyCodeInTextFormat(settingsManager.SpeedBuffKeyCode));
        SetUpKeyCodeText(_jumpBuffKeyText, KeyCodeInTextFormat(settingsManager.JumpBuffKeyCode));
        SetUpKeyCodeText(_pauseKeyText, KeyCodeInTextFormat(settingsManager.PauseKeyCode));
    }

    /// <summary>
    /// Set up the Display Menu.
    /// </summary>
    public void ShowDisplay()
    {
        display.gameObject.SetActive(true);
        controls.gameObject.SetActive(false);
        sounds.gameObject.SetActive(false);

        _resolution.value = settingsManager.Resolution;

        _fullScreenTransform.GetChild(0).gameObject.SetActive(settingsManager.Fullscreen);
        _fullScreenTransform.GetChild(1).gameObject.SetActive(!settingsManager.Fullscreen);

        _vSyncTransform.GetChild(0).gameObject.SetActive(settingsManager.Vsync);
        _vSyncTransform.GetChild(1).gameObject.SetActive(!settingsManager.Vsync);

        ShowLanguageOptions();
    }
    #endregion

    private void DeactivateAllChecking()
    {
        _isCheckingForJump = false;
        _isCheckingForMoveLeft = false;
        _isCheckingForMoveRight = false;
        _isCheckingForFire = false;
        _isCheckingForSpeedBuff = false;
        _isCheckingForJumpBuff = false;
        _isCheckingForPause = false;

        ShowControls();
    }

    #region Save Sound
    /// <summary>
    /// Saves the selected general sounds volume.
    /// </summary>
    public void SaveGeneralSoundsVolume() => SaveSound(_generalSoundTransform);

    /// <summary>
    /// Saves the selected music volume.
    /// </summary>
    public void SaveMusicVolume() => SaveSound(_musicTransform);

    /// <summary>
    /// Saves the selected sfx volume.
    /// </summary>
    public void SaveSfxSound() => SaveSound(_sfxTransform);

    private void SaveSound(Transform soundTransform)
    {
        if (soundTransform == null) return;
        var value = soundTransform.GetChild(1).GetComponent<Slider>().value;
        settingsManager.SoundEffectVolume = value;
        SetUpSoundText(soundTransform, value);
        SettingsManager.SetUpSound(FindObjectOfType<Camera>().transform);
    }
    #endregion

    #region Save Key Code
    /// <summary>
    /// Saves last pressed key for jump.
    /// </summary>
    public void SaveJumpValue()
    {
        DeactivateAllChecking();
        _isCheckingForJump = true;
        SetUpKeyCodeText(_jumpKeyText, "");
    }

    /// <summary>
    /// Saves last pressed key for left movement.
    /// </summary>
    public void SaveMoveLeftValue()
    {
        DeactivateAllChecking();
        _isCheckingForMoveLeft = true;
        SetUpKeyCodeText(_moveLeftKeyText, "");
    }

    /// <summary>
    /// Saves last pressed key for right movement.
    /// </summary>
    public void SaveMoveRightValue()
    {
        DeactivateAllChecking();
        _isCheckingForMoveRight = true;
        SetUpKeyCodeText(_moveRightKeyText, "");
    }

    /// <summary>
    /// Saves last pressed key for fire.
    /// </summary>
    public void SaveFireValue()
    {
        DeactivateAllChecking();
        _isCheckingForFire = true;
        SetUpKeyCodeText(_fireKeyText, "");
    }

    /// <summary>
    /// Saves last pressed key for minAndMaxSpeed buff.
    /// </summary>
    public void SaveSpeedBuffValue()
    {
        DeactivateAllChecking();
        _isCheckingForSpeedBuff = true;
        SetUpKeyCodeText(_speedBuffKeyText, "");
    }

    /// <summary>
    /// Saves last pressed key for jump buff.
    /// </summary>
    public void SaveJumpBuffValue()
    {
        DeactivateAllChecking();
        _isCheckingForJumpBuff = true;
        SetUpKeyCodeText(_jumpBuffKeyText, "");
    }

    /// <summary>
    /// Saves last pressed key for pause.
    /// </summary>
    public void SavePauseValue()
    {
        DeactivateAllChecking();
        _isCheckingForPause = true;
        SetUpKeyCodeText(_pauseKeyText, "");
    }
    #endregion

    #region Save Display
    /// <summary>
    /// Save and apply selected resolution.
    /// </summary>
    public void SaveResolution()
    {

        var resolutionBeforeTransformation = _resolution.value;
        var resolution = Utility.ConvertResolutionIndexToTuple(resolutionBeforeTransformation);

        settingsManager.Resolution = resolutionBeforeTransformation;
        Screen.SetResolution(resolution.Item1, resolution.Item2, settingsManager.Fullscreen);
    }

    /// <summary>
    /// Save and apply selected full screen bool.
    /// </summary>
    public void SaveFullscreen()
    {
        var isFullScreen = _fullScreenTransform.GetChild(0).gameObject.activeSelf;
        var resolution = Utility.ConvertResolutionIndexToTuple(settingsManager.Resolution);

        settingsManager.Fullscreen = isFullScreen;
        Screen.SetResolution(resolution.Item1, resolution.Item2, isFullScreen);
    }

    /// <summary>
    /// Save and apply selected vsync bool.
    /// </summary>
    public void SaveVsyncScreen()
    {
        var isVSyncEnabled = _vSyncTransform.GetChild(0).gameObject.activeSelf;

        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
        settingsManager.Vsync = isVSyncEnabled;
    }

    /// <summary>
    /// Sets the selected language for localization.
    /// </summary>
    public void SaveLanguage()
    {
        var languageID = _language.value;
        settingsManager.Language =
            (Language)languageID;
        ShowLanguageOptions();
        StartCoroutine(SetLanguage(languageID));
    }
    #endregion

    /// <summary>
    /// Display the language options based on active language.
    /// </summary>
    private void ShowLanguageOptions()
    {
        var language = settingsManager.Language;
        _language.options[0].text = language == Language.English ? "English" : "Engleza";
        _language.options[1].text = language == Language.English ? "Romanian" : "Romana";
    }

    /// <summary>
    /// Coroutine that set up the language using localizationSettings.
    /// </summary>
    /// <param name="languageID">Id of the new language.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    private static IEnumerator SetLanguage(int languageID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageID];
    }
}