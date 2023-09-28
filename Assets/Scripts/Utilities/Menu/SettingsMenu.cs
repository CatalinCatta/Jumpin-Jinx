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
    private Transform _localTransform;
    private SettingsManager _settingsManager;

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
        _localTransform = transform;
        
        var sounds = _localTransform.GetChild(1);
        _generalSoundTransform = sounds.GetChild(0); 
        _musicTransform = sounds.GetChild(1); 
        _sfxTransform = sounds.GetChild(2);

        var keys = _localTransform.GetChild(2);
        _jumpKeyText = GetTextComponentForKey(0);
        _moveLeftKeyText = GetTextComponentForKey(1);
        _moveRightKeyText = GetTextComponentForKey(2);
        _fireKeyText = GetTextComponentForKey(3);
        _speedBuffKeyText = GetTextComponentForKey(4);
        _jumpBuffKeyText = GetTextComponentForKey(5);
        _pauseKeyText = GetTextComponentForKey(6);
        
        var display = _localTransform.GetChild(3);
        _resolution = GetDisplayChild(0).GetComponent<TMP_Dropdown>();
        _fullScreenTransform = GetDisplayChild(1);
        _vSyncTransform = GetDisplayChild(2);
        _language = GetDisplayChild(3).GetComponent<TMP_Dropdown>();

        _settingsManager = SettingsManager.Instance;

        TextMeshProUGUI GetTextComponentForKey(int keyId) =>
            keys.GetChild(keyId).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        
        Transform GetDisplayChild(int childId) =>
            display.GetChild(childId).GetChild(1);
    }

    private void Start()
    {        
        var resolution = Screen.currentResolution;
        Screen.SetResolution(resolution.width, resolution.height, _settingsManager.Fullscreen);
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
                _settingsManager.JumpKeyCode = keyCode;
                _isCheckingForJump = false;
                return;
            }

            if (_isCheckingForMoveLeft)
            {
                SetUpKeyCodeText(_moveLeftKeyText, KeyCodeInTextFormat(keyCode));
                _settingsManager.MoveLeftKeyCode = keyCode;
                _isCheckingForMoveLeft = false;
                return;
            }

            if (_isCheckingForMoveRight)
            {
                SetUpKeyCodeText(_moveRightKeyText, KeyCodeInTextFormat(keyCode));
                _settingsManager.MoveRightKeyCode = keyCode;
                _isCheckingForMoveRight = false;
                return;
            }

            if (_isCheckingForFire)
            {
                SetUpKeyCodeText(_fireKeyText, KeyCodeInTextFormat(keyCode));
                _settingsManager.FireKeyCode = keyCode;
                _isCheckingForFire = false;
                return;
            }

            if (_isCheckingForSpeedBuff)
            {
                SetUpKeyCodeText(_speedBuffKeyText, KeyCodeInTextFormat(keyCode));
                _settingsManager.SpeedBuffKeyCode = keyCode;
                _isCheckingForSpeedBuff = false;
                return;
            }

            if (_isCheckingForJumpBuff)
            {
                SetUpKeyCodeText(_jumpBuffKeyText, KeyCodeInTextFormat(keyCode));
                _settingsManager.JumpBuffKeyCode = keyCode;
                _isCheckingForJumpBuff = false;
                return;
            }

            if (_isCheckingForPause)
            {
                SetUpKeyCodeText(_pauseKeyText, KeyCodeInTextFormat(keyCode));
                _settingsManager.PauseKeyCode = keyCode;
                _isCheckingForPause = false;
                return;
            }
        }
    }

    private void OnEnable() // TODO: Remove coroutines when create animation in unity.
    {
        StartCoroutine(MoveObject(SettingsFrames.Categories, true, false));

        Debug.Log(_settingsManager.CurrentCategoryTab);
        switch (_settingsManager.CurrentCategoryTab)
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

        StartCoroutine(MoveObject(_settingsManager.CurrentCategoryTab, true, false));
    }

    private void OnDisable() => DeactivateAllChecking();
    
    private static void SetUpKeyCodeText(TMP_Text keyCodeText, string text) => keyCodeText.text = text;

    private static string KeyCodeInTextFormat(KeyCode keyCode) =>
        Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
    
    private static void SetUpSoundText(Transform soundTransform, float volume) =>
        soundTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(volume * 100) + "%";

    /// <summary>
    /// Close settings widow and reopen menu. 
    /// </summary>
    public void HideSettings()
    {
        StartCoroutine(MoveObject(SettingsFrames.Categories, false, false));
        StartCoroutine(MoveObject(_settingsManager.CurrentCategoryTab, false, false));
    }
  
    /// <summary>
    /// Checks if a key code is already in use by another setting.
    /// </summary>
    /// <param name="keyCode">KeyCode to check.</param>
    /// <returns>Bool representing if key was already used.</returns>
    private bool KeyAlreadyInUse(KeyCode keyCode)  // TODO: Create unity animation to show wrong decision.
    {
        var allKeyCodes = new List<KeyCode>
        {
            _settingsManager.JumpKeyCode,
            _settingsManager.MoveLeftKeyCode,
            _settingsManager.MoveRightKeyCode,
            _settingsManager.FireKeyCode,
            _settingsManager.SpeedBuffKeyCode,
            _settingsManager.JumpBuffKeyCode,
            _settingsManager.PauseKeyCode
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

    #region Animations
    private IEnumerator MoveObject(SettingsFrames settingsFrames, bool isAppearing, bool verticalRotation) // TODO: Remove this and create proper animation.
    {
        var currentTime = 0f;
        var objectToMove = _localTransform.GetChild((int)settingsFrames).GetComponent<RectTransform>();

        objectToMove.gameObject.SetActive(true);
        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;

            if (settingsFrames == SettingsFrames.Categories)
                objectToMove.anchoredPosition =
                    new Vector2(Mathf.Lerp(isAppearing ? -600f : 0f, isAppearing ? 0f : -600f, currentTime / 0.5f),
                        objectToMove.anchoredPosition.y);
            else
            {
                if (verticalRotation)
                    objectToMove.rotation =
                        Quaternion.Euler(Mathf.Lerp(isAppearing ? 90f : 0f, isAppearing ? 0f : 90f, currentTime / 0.5f),
                            0f, 0f);
                else
                {
                    objectToMove.rotation =
                        Quaternion.Euler(0f, 0f,
                            Mathf.Lerp(isAppearing ? -90f : 0f, isAppearing ? 0f : -90f, currentTime / 0.5f));
                    objectToMove.anchoredPosition =
                        new Vector2(Mathf.Lerp(isAppearing ? 600f : 0f, isAppearing ? 0f : 600f, currentTime / 0.5f),
                            objectToMove.anchoredPosition.y);
                }
            }

            yield return null;
        }

        objectToMove.anchoredPosition = new Vector2(0f, objectToMove.anchoredPosition.y);
        objectToMove.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (settingsFrames == SettingsFrames.Categories && !isAppearing)
            gameObject.SetActive(false);
    }

    private IEnumerator ChangeCategoryAnimation(SettingsFrames settingsFrames)  // TODO: Remove this and create proper animation.
    {
        StartCoroutine(MoveObject(_settingsManager.CurrentCategoryTab, false, true));

        yield return new WaitForSeconds(0.5f);

        _localTransform.GetChild((int)_settingsManager.CurrentCategoryTab).gameObject.SetActive(false);

        _localTransform.GetChild((int)settingsFrames).gameObject.SetActive(true);

        _settingsManager.CurrentCategoryTab = settingsFrames;

        StartCoroutine(MoveObject(settingsFrames, true, true));
    }
    #endregion

    /// <summary>
    /// Changes the currently displayed settings category.
    /// </summary>
    /// <param name="settingsFrames">New Category id to open it.</param>
    public void ChangeCategory(int settingsFrames)
    {
        if (settingsFrames == (int)SettingsFrames.Controls) DeactivateAllChecking();

        StartCoroutine(ChangeCategoryAnimation((SettingsFrames)settingsFrames));
    }
    
    #region Show Category
    
    /// <summary>
    /// Set up the sounds volume.
    /// </summary>
    public void ShowSounds()
    {
        SetUpSound(_generalSoundTransform, _settingsManager.GeneralVolume);
        SetUpSound(_musicTransform, _settingsManager.MusicVolume);
        SetUpSound(_sfxTransform, _settingsManager.SoundEffectVolume);
        
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
        SetUpKeyCodeText(_jumpKeyText, KeyCodeInTextFormat(_settingsManager.JumpKeyCode));
        SetUpKeyCodeText(_moveLeftKeyText, KeyCodeInTextFormat(_settingsManager.MoveLeftKeyCode));
        SetUpKeyCodeText(_moveRightKeyText, KeyCodeInTextFormat(_settingsManager.MoveRightKeyCode));
        SetUpKeyCodeText(_fireKeyText, KeyCodeInTextFormat(_settingsManager.FireKeyCode));
        SetUpKeyCodeText(_speedBuffKeyText, KeyCodeInTextFormat(_settingsManager.SpeedBuffKeyCode));
        SetUpKeyCodeText(_jumpBuffKeyText, KeyCodeInTextFormat(_settingsManager.JumpBuffKeyCode));
        SetUpKeyCodeText(_pauseKeyText, KeyCodeInTextFormat(_settingsManager.PauseKeyCode));
    }

    /// <summary>
    /// Set up the Display Menu.
    /// </summary>
    public void ShowDisplay()
    {
        _resolution.value = _settingsManager.Resolution;

        _fullScreenTransform.GetChild(0).gameObject.SetActive(_settingsManager.Fullscreen);
        _fullScreenTransform.GetChild(1).gameObject.SetActive(!_settingsManager.Fullscreen);

        _vSyncTransform.GetChild(0).gameObject.SetActive(_settingsManager.Vsync);
        _vSyncTransform.GetChild(1).gameObject.SetActive(!_settingsManager.Vsync);

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
        _settingsManager.SoundEffectVolume = value;
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

        _settingsManager.Resolution = resolutionBeforeTransformation;
        Screen.SetResolution(resolution.Item1, resolution.Item2, _settingsManager.Fullscreen);
    }

    /// <summary>
    /// Save and apply selected full screen bool.
    /// </summary>
    public void SaveFullscreen()
    {
        var isFullScreen = _fullScreenTransform.GetChild(0).gameObject.activeSelf;
        var resolution = Utility.ConvertResolutionIndexToTuple(_settingsManager.Resolution);

        _settingsManager.Fullscreen = isFullScreen;
        Screen.SetResolution(resolution.Item1, resolution.Item2, isFullScreen);
    }

    /// <summary>
    /// Save and apply selected vsync bool.
    /// </summary>
    public void SaveVsyncScreen()
    {
        var isVSyncEnabled = _vSyncTransform.GetChild(0).gameObject.activeSelf;

        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
        _settingsManager.Vsync = isVSyncEnabled;
    }

    /// <summary>
    /// Sets the selected language for localization.
    /// </summary>
    public void SaveLanguage()
    {
        var languageID = _language.value;
        _settingsManager.Language =
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
        var language = _settingsManager.Language;
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