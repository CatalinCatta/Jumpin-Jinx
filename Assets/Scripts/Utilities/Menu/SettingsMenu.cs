using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// Manages the settings menu and user input for changing settings.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Transform background, sounds, controls, display; 
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

    private (Transform objTransform, TextMeshProUGUI tmp)
        _jumpKey,
        _moveLeftKey,
        _moveRightKey,
        _fireKey,
        _speedBuffKey,
        _jumpBuffKey,
        _pauseKey;
    private Transform _alertTransform;
    
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

        _jumpKey = (controls.GetChild(0),GetTextComponentForKey(0));
        _moveLeftKey = (controls.GetChild(1),GetTextComponentForKey(1));
        _moveRightKey = (controls.GetChild(2),GetTextComponentForKey(2));
        _fireKey = (controls.GetChild(3),GetTextComponentForKey(3));
        _speedBuffKey = (controls.GetChild(4),GetTextComponentForKey(4));
        _jumpBuffKey = (controls.GetChild(5),GetTextComponentForKey(5));
        _pauseKey = (controls.GetChild(6),GetTextComponentForKey(6));
        _alertTransform = controls.GetChild(7);
        
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

    private void Update() => ChangeKeyValue();
    
    private static void SetUpSoundText(Transform soundTransform, float volume) =>
        soundTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(volume * 100) + "%";

    #region Show Category
    
    /// <summary>
    /// Set up the sounds volume.
    /// </summary>
    public void ShowSounds()
    {
        DeactivateAllChecking();
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
        DeactivateAllChecking();
        controls.gameObject.SetActive(true);
        sounds.gameObject.SetActive(false);
        display.gameObject.SetActive(false);
        SetUpControlsDisplayedValue();
    }

    /// <summary>
    /// Set up the Display Menu.
    /// </summary>
    public void ShowDisplay()
    {
        DeactivateAllChecking();
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

    #region Controls Private Settings

    private void ChangeKeyValue()
    {
        if (!_isCheckingForJump && !_isCheckingForMoveLeft && !_isCheckingForMoveRight && !_isCheckingForFire &&
            !_isCheckingForSpeedBuff && !_isCheckingForJumpBuff && !_isCheckingForPause) return;

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKeyDown(keyCode)) continue;
            if (KeyAlreadyInUse(keyCode))
            {
                StartCoroutine(AlertKeyAlreadyInUse(
                    keyCode == settingsManager.JumpKeyCode ? _jumpKey.objTransform :
                    keyCode == settingsManager.MoveLeftKeyCode ? _moveLeftKey.objTransform :
                    keyCode == settingsManager.MoveRightKeyCode ? _moveRightKey.objTransform :
                    keyCode == settingsManager.FireKeyCode ? _fireKey.objTransform :
                    keyCode == settingsManager.SpeedBuffKeyCode ? _speedBuffKey.objTransform :
                    keyCode == settingsManager.JumpBuffKeyCode ? _jumpBuffKey.objTransform :
                    keyCode == settingsManager.PauseKeyCode ? _pauseKey.objTransform :
                    _alertTransform, keyCode == KeyCode.Escape ? "ESC" : ""));
                return;
            }
            if (_isCheckingForJump)
            {
                SetUpKeyCodeText(_jumpKey.tmp, KeyCodeInTextFormat(keyCode));
                settingsManager.JumpKeyCode = keyCode;
                _isCheckingForJump = false;
                return;
            }

            if (_isCheckingForMoveLeft)
            {
                SetUpKeyCodeText(_moveLeftKey.tmp, KeyCodeInTextFormat(keyCode));
                settingsManager.MoveLeftKeyCode = keyCode;
                _isCheckingForMoveLeft = false;
                return;
            }

            if (_isCheckingForMoveRight)
            {
                SetUpKeyCodeText(_moveRightKey.tmp, KeyCodeInTextFormat(keyCode));
                settingsManager.MoveRightKeyCode = keyCode;
                _isCheckingForMoveRight = false;
                return;
            }

            if (_isCheckingForFire)
            {
                SetUpKeyCodeText(_fireKey.tmp, KeyCodeInTextFormat(keyCode));
                settingsManager.FireKeyCode = keyCode;
                _isCheckingForFire = false;
                return;
            }

            if (_isCheckingForSpeedBuff)
            {
                SetUpKeyCodeText(_speedBuffKey.tmp, KeyCodeInTextFormat(keyCode));
                settingsManager.SpeedBuffKeyCode = keyCode;
                _isCheckingForSpeedBuff = false;
                return;
            }

            if (_isCheckingForJumpBuff)
            {
                SetUpKeyCodeText(_jumpBuffKey.tmp, KeyCodeInTextFormat(keyCode));
                settingsManager.JumpBuffKeyCode = keyCode;
                _isCheckingForJumpBuff = false;
                return;
            }

            if (_isCheckingForPause)
            {
                SetUpKeyCodeText(_pauseKey.tmp, KeyCodeInTextFormat(keyCode));
                settingsManager.PauseKeyCode = keyCode;
                _isCheckingForPause = false;
                return;
            }
        }
    }

    /// <summary>
    /// Checks if a key code is already in use by another setting.
    /// </summary>
    /// <param name="keyCode">KeyCode to check.</param>
    /// <returns>Bool representing if key was already used.</returns>
    private bool KeyAlreadyInUse(KeyCode keyCode)
    {
        var allKeyCodes = new List<KeyCode>
        {
            settingsManager.JumpKeyCode,
            settingsManager.MoveLeftKeyCode,
            settingsManager.MoveRightKeyCode,
            settingsManager.FireKeyCode,
            settingsManager.SpeedBuffKeyCode,
            settingsManager.JumpBuffKeyCode,
            settingsManager.PauseKeyCode,
            KeyCode.Escape
        };

        if (_isCheckingForJump) allKeyCodes.Remove(settingsManager.JumpKeyCode);
        else if (_isCheckingForMoveLeft) allKeyCodes.Remove(settingsManager.MoveLeftKeyCode);
        else if (_isCheckingForMoveRight) allKeyCodes.Remove(settingsManager.MoveRightKeyCode);
        else if (_isCheckingForFire) allKeyCodes.Remove(settingsManager.FireKeyCode);
        else if (_isCheckingForSpeedBuff) allKeyCodes.Remove(settingsManager.SpeedBuffKeyCode);
        else if (_isCheckingForJumpBuff) allKeyCodes.Remove(settingsManager.JumpBuffKeyCode);
        else if (_isCheckingForPause) allKeyCodes.Remove(settingsManager.PauseKeyCode);

        return allKeyCodes.Contains(keyCode);
    }
    
    private void DeactivateAllChecking()
    {
        _isCheckingForJump = false;
        _isCheckingForMoveLeft = false;
        _isCheckingForMoveRight = false;
        _isCheckingForFire = false;
        _isCheckingForSpeedBuff = false;
        _isCheckingForJumpBuff = false;
        _isCheckingForPause = false;
    }

    private void SetUpControlsDisplayedValue()
    {
        SetUpKeyCodeText(_jumpKey.tmp, KeyCodeInTextFormat(settingsManager.JumpKeyCode));
        SetUpKeyCodeText(_moveLeftKey.tmp, KeyCodeInTextFormat(settingsManager.MoveLeftKeyCode));
        SetUpKeyCodeText(_moveRightKey.tmp, KeyCodeInTextFormat(settingsManager.MoveRightKeyCode));
        SetUpKeyCodeText(_fireKey.tmp, KeyCodeInTextFormat(settingsManager.FireKeyCode));
        SetUpKeyCodeText(_speedBuffKey.tmp, KeyCodeInTextFormat(settingsManager.SpeedBuffKeyCode));
        SetUpKeyCodeText(_jumpBuffKey.tmp, KeyCodeInTextFormat(settingsManager.JumpBuffKeyCode));
        SetUpKeyCodeText(_pauseKey.tmp, KeyCodeInTextFormat(settingsManager.PauseKeyCode));
    }
    
    public float colorTimeOnRed = 0.5f;
    public float changeColorBackToNormal = 0.5f;


    private IEnumerator AlertKeyAlreadyInUse(Component currentKeyUserFrame, [CanBeNull] string keyUsed = "")
    {
        Image currentKeyUserFrameImage = null;
        var originalColor = new Color();


        if (keyUsed != "")
        {
            _alertTransform.GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { keyUsed });
            currentKeyUserFrame.gameObject.SetActive(true);
        }
        else
        {
            currentKeyUserFrameImage = currentKeyUserFrame.GetComponent<Image>();
            originalColor = currentKeyUserFrameImage.color;
            currentKeyUserFrameImage.color = Color.red;
        }

        background.parent.GetComponent<Animator>().Play("ShakeSettings");
        yield return new WaitForSeconds(colorTimeOnRed);
        var startTime = Time.time;

        while (Time.time - startTime < changeColorBackToNormal)
        {
            Debug.Log(currentKeyUserFrame);
            Debug.Log(currentKeyUserFrame.GetComponent<TextMeshProUGUI>());
            Debug.Log(keyUsed);
            if (keyUsed != "")
                currentKeyUserFrame.GetComponent<TextMeshProUGUI>().color = Color.Lerp(new Color(1, 0, 0, 0), Color.red,
                    (Time.time - startTime) / changeColorBackToNormal);
            else
                currentKeyUserFrameImage!.color = Color.Lerp(Color.red, originalColor,
                    (Time.time - startTime) / changeColorBackToNormal);

            yield return null;
        }

        if (keyUsed != "") currentKeyUserFrame.gameObject.SetActive(false);
        else currentKeyUserFrameImage!.color = originalColor;
    }

    private static void SetUpKeyCodeText(TMP_Text keyCodeText, string text) => keyCodeText.text = text;
    
    private static string KeyCodeInTextFormat(KeyCode keyCode) =>
        Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();

    #endregion

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
        SetUpControlsDisplayedValue();
        _isCheckingForJump = true;
        SetUpKeyCodeText(_jumpKey.tmp, "");
    }

    /// <summary>
    /// Saves last pressed key for left movement.
    /// </summary>
    public void SaveMoveLeftValue()
    {
        DeactivateAllChecking();
        SetUpControlsDisplayedValue();
        _isCheckingForMoveLeft = true;
        SetUpKeyCodeText(_moveLeftKey.tmp, "");
    }

    /// <summary>
    /// Saves last pressed key for right movement.
    /// </summary>
    public void SaveMoveRightValue()
    {
        DeactivateAllChecking();
        SetUpControlsDisplayedValue();
        _isCheckingForMoveRight = true;
        SetUpKeyCodeText(_moveRightKey.tmp, "");
    }

    /// <summary>
    /// Saves last pressed key for fire.
    /// </summary>
    public void SaveFireValue()
    {
        DeactivateAllChecking();
        SetUpControlsDisplayedValue();
        _isCheckingForFire = true;
        SetUpKeyCodeText(_fireKey.tmp, "");
    }

    /// <summary>
    /// Saves last pressed key for minAndMaxSpeed buff.
    /// </summary>
    public void SaveSpeedBuffValue()
    {
        DeactivateAllChecking();
        SetUpControlsDisplayedValue();
        _isCheckingForSpeedBuff = true;
        SetUpKeyCodeText(_speedBuffKey.tmp, "");
    }

    /// <summary>
    /// Saves last pressed key for jump buff.
    /// </summary>
    public void SaveJumpBuffValue()
    {
        DeactivateAllChecking();
        SetUpControlsDisplayedValue();
        _isCheckingForJumpBuff = true;
        SetUpKeyCodeText(_jumpBuffKey.tmp, "");
    }

    /// <summary>
    /// Saves last pressed key for pause.
    /// </summary>
    public void SavePauseValue()
    {
        DeactivateAllChecking();
        SetUpControlsDisplayedValue();
        _isCheckingForPause = true;
        SetUpKeyCodeText(_pauseKey.tmp, "");
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