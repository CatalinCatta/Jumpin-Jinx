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
    private bool 
        _isCheckingForJump,
        _isCheckingForMoveLeft,
        _isCheckingForMoveRight,
        _isCheckingForFire,
        _isCheckingForSpeedBuff,
        _isCheckingForJumpBuff,
        _isCheckingForPause;

    private void Start()
    {
        var resolution = Screen.currentResolution;
        Screen.SetResolution(resolution.width, resolution.height, SettingsManager.Instance.Fullscreen);
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
                transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.JumpKeyCode = keyCode;
                _isCheckingForJump = false;
                return;
            }

            if (_isCheckingForMoveLeft)
            {
                transform.GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.MoveLeftKeyCode = keyCode;
                _isCheckingForMoveLeft = false;
                return;
            }

            if (_isCheckingForMoveRight)
            {
                transform.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.MoveRightKeyCode = keyCode;
                _isCheckingForMoveRight = false;
                return;
            }

            if (_isCheckingForFire)
            {
                transform.GetChild(2).GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.FireKeyCode = keyCode;
                _isCheckingForFire = false;
                return;
            }

            if (_isCheckingForSpeedBuff)
            {
                transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.SpeedBuffKeyCode = keyCode;
                _isCheckingForSpeedBuff = false;
                return;
            }

            if (_isCheckingForJumpBuff)
            {
                transform.GetChild(2).GetChild(5).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.JumpBuffKeyCode = keyCode;
                _isCheckingForJumpBuff = false;
                return;
            }

            if (_isCheckingForPause)
            {
                transform.GetChild(2).GetChild(6).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.PauseKeyCode = keyCode;
                _isCheckingForPause = false;
                return;
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(MoveObject(SettingsFrames.Categories, true, false));

        switch (SettingsManager.Instance.CurrentCategoryTab)
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

        StartCoroutine(MoveObject(SettingsManager.Instance.CurrentCategoryTab, true, false));
    }

    private void OnDisable() =>
        DeactivateAllChecking();

    /// <summary>
    /// Close settings widow and reopen menu. 
    /// </summary>
    public void HideSettings()
    {
        StartCoroutine(MoveObject(SettingsFrames.Categories, false, false));
        StartCoroutine(MoveObject(SettingsManager.Instance.CurrentCategoryTab, false, false));
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
            SettingsManager.Instance.JumpKeyCode,
            SettingsManager.Instance.MoveLeftKeyCode,
            SettingsManager.Instance.MoveRightKeyCode,
            SettingsManager.Instance.FireKeyCode,
            SettingsManager.Instance.SpeedBuffKeyCode,
            SettingsManager.Instance.JumpBuffKeyCode,
            SettingsManager.Instance.PauseKeyCode
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

    // ***TO DO*** : remove this and add proper animation and animator
    private IEnumerator MoveObject(SettingsFrames settingsFrames, bool isAppearing, bool verticalRotation)
    {
        var currentTime = 0f;
        var objectToMove = transform.GetChild((int)settingsFrames).GetComponent<RectTransform>();

        objectToMove.gameObject.SetActive(true);

        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;

            if (settingsFrames == SettingsFrames.Categories)
            {
                objectToMove.anchoredPosition =
                    new Vector2(Mathf.Lerp(isAppearing ? -600f : 0f, isAppearing ? 0f : -600f, currentTime / 0.5f),
                        objectToMove.anchoredPosition.y);
            }
            else
            {
                if (verticalRotation)
                {
                    objectToMove.rotation =
                        Quaternion.Euler(Mathf.Lerp(isAppearing ? 90f : 0f, isAppearing ? 0f : 90f, currentTime / 0.5f),
                            0f, 0f);
                }
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

    /// <summary>
    /// Changes the currently displayed settings category.
    /// </summary>
    /// <param name="settingsFrames">New Category id to open it.</param>
    public void ChangeCategory(int settingsFrames)
    {
        if (settingsFrames == 2)
            DeactivateAllChecking();

        StartCoroutine(ChangeCategoryAnimation((SettingsFrames)settingsFrames));
    }

    // ***TO DO*** : remove this and add proper animation and animator
    private IEnumerator ChangeCategoryAnimation(SettingsFrames settingsFrames)
    {
        StartCoroutine(MoveObject(SettingsManager.Instance.CurrentCategoryTab, false, true));

        yield return new WaitForSeconds(0.5f);

        transform.GetChild((int)SettingsManager.Instance.CurrentCategoryTab).gameObject.SetActive(false);

        transform.GetChild((int)settingsFrames).gameObject.SetActive(true);

        SettingsManager.Instance.CurrentCategoryTab = settingsFrames;

        StartCoroutine(MoveObject(settingsFrames, true, true));
    }

    /// <summary>
    /// Set up the sounds volume.
    /// </summary>
    public void ShowSounds()
    {
        var soundsObject = transform.GetChild(1);

        var generalSoundsObject = soundsObject.GetChild(0);
        var generalVolume = SettingsManager.Instance.GeneralVolume;
        generalSoundsObject.GetChild(1).GetComponent<Slider>().value = generalVolume;
        generalSoundsObject.GetChild(2).GetComponent<TextMeshProUGUI>().text =
            Mathf.RoundToInt(generalVolume * 100) + "%";

        var musicSoundsObject = soundsObject.GetChild(1);
        var musicVolume = SettingsManager.Instance.MusicVolume;
        musicSoundsObject.GetChild(1).GetComponent<Slider>().value = musicVolume;
        musicSoundsObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(musicVolume * 100) + "%";

        var sfxSoundsObject = soundsObject.GetChild(2);
        var sfxVolume = SettingsManager.Instance.SoundEffectVolume;
        sfxSoundsObject.GetChild(1).GetComponent<Slider>().value = sfxVolume;
        sfxSoundsObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(sfxVolume * 100) + "%";
    }

    /// <summary>
    /// Set up the keybindings.
    /// </summary>
    public void ShowControls()
    {
        var controlsObject = transform.GetChild(2);

        controlsObject.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.JumpKeyCode)!, @"(\p{Lu})", " $1")
                .Trim();

        controlsObject.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.MoveLeftKeyCode)!, @"(\p{Lu})", " $1")
                .Trim();

        controlsObject.GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.MoveRightKeyCode)!, @"(\p{Lu})", " $1")
                .Trim();

        controlsObject.GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.FireKeyCode)!, @"(\p{Lu})", " $1")
                .Trim();

        controlsObject.GetChild(4).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.SpeedBuffKeyCode)!, @"(\p{Lu})", " $1")
                .Trim();

        controlsObject.GetChild(5).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.JumpBuffKeyCode)!, @"(\p{Lu})", " $1")
                .Trim();

        controlsObject.GetChild(6).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.PauseKeyCode)!, @"(\p{Lu})", " $1")
                .Trim();
    }

    /// <summary>
    /// Set up the Display Menu.
    /// </summary>
    public void ShowDisplay()
    {
        var displayObject = transform.GetChild(3);

        displayObject.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value = SettingsManager.Instance.Resolution;

        var fullscreenToggleObject = displayObject.GetChild(1).GetChild(1);
        fullscreenToggleObject.GetChild(0).gameObject.SetActive(SettingsManager.Instance.Fullscreen);
        fullscreenToggleObject.GetChild(1).gameObject.SetActive(!SettingsManager.Instance.Fullscreen);

        var vSyncToggleObject = displayObject.GetChild(2).GetChild(1);
        vSyncToggleObject.GetChild(0).gameObject.SetActive(SettingsManager.Instance.Vsync);
        vSyncToggleObject.GetChild(1).gameObject.SetActive(!SettingsManager.Instance.Vsync);

        ShowLanguageOptions();
    }

    /// <summary>
    /// Cancel all checking that was activated.
    /// </summary>
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

    /// <summary>
    /// Saves the selected general sounds volume.
    /// </summary>
    public void SaveGeneralSoundsVolume()
    {
        var generalSoundObject = transform.GetChild(1).GetChild(0);
        var value = generalSoundObject.GetChild(1).GetComponent<Slider>().value;
        SettingsManager.Instance.GeneralVolume = value;
        generalSoundObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(value * 100) + "%";
        SettingsManager.SetUpSound(FindObjectOfType<Camera>().transform);
    }

    /// <summary>
    /// Saves the selected music volume.
    /// </summary>
    public void SaveMusicVolume()
    {
        var musicObject = transform.GetChild(1).GetChild(1);
        var value = musicObject.GetChild(1).GetComponent<Slider>().value;
        SettingsManager.Instance.MusicVolume = value;
        musicObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(value * 100) + "%";
        SettingsManager.SetUpSound(FindObjectOfType<Camera>().transform);
    }

    /// <summary>
    /// Saves the selected sfx volume.
    /// </summary>
    public void SaveSfxSound()
    {
        var sfxObject = transform.GetChild(1).GetChild(2);
        var value = sfxObject.GetChild(1).GetComponent<Slider>().value;
        SettingsManager.Instance.SoundEffectVolume = value;
        sfxObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(value * 100) + "%";
    }

    /// <summary>
    /// Saves last pressed key for jump.
    /// </summary>
    public void SaveJumpValue()
    {
        DeactivateAllChecking();
        _isCheckingForJump = true;
        transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Saves last pressed key for left movement.
    /// </summary>
    public void SaveMoveLeftValue()
    {
        DeactivateAllChecking();
        _isCheckingForMoveLeft = true;
        transform.GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Saves last pressed key for right movement.
    /// </summary>
    public void SaveMoveRightValue()
    {
        DeactivateAllChecking();
        _isCheckingForMoveRight = true;
        transform.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Saves last pressed key for fire.
    /// </summary>
    public void SaveFireValue()
    {
        DeactivateAllChecking();
        _isCheckingForFire = true;
        transform.GetChild(2).GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Saves last pressed key for speed buff.
    /// </summary>
    public void SaveSpeedBuffValue()
    {
        DeactivateAllChecking();
        _isCheckingForSpeedBuff = true;
        transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Saves last pressed key for jump buff.
    /// </summary>
    public void SaveJumpBuffValue()
    {
        DeactivateAllChecking();
        _isCheckingForJumpBuff = true;
        transform.GetChild(2).GetChild(5).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Saves last pressed key for pause.
    /// </summary>
    public void SavePauseValue()
    {
        DeactivateAllChecking();
        _isCheckingForPause = true;
        transform.GetChild(2).GetChild(6).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Save and apply selected resolution.
    /// </summary>
    public void SaveResolution()
    {
        var isFullScreen = SettingsManager.Instance.Fullscreen;
        var resolutionBeforeTransformation =
            transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value;
        var resolution = Utils.ResolutionIndexToTuple(resolutionBeforeTransformation);

        SettingsManager.Instance.Resolution = resolutionBeforeTransformation;
        Screen.SetResolution(resolution.Item1, resolution.Item2, isFullScreen);
    }

    /// <summary>
    /// Save and apply selected full screen bool.
    /// </summary>
    public void SaveFullscreen()
    {
        var isFullScreen = transform.GetChild(3).GetChild(1).GetChild(1).GetChild(0).gameObject.activeSelf;
        var resolution = Utils.ResolutionIndexToTuple(SettingsManager.Instance.Resolution);

        SettingsManager.Instance.Fullscreen = isFullScreen;
        Screen.SetResolution(resolution.Item1, resolution.Item2, isFullScreen);
    }

    /// <summary>
    /// Save and apply selected vsync bool.
    /// </summary>
    public void SaveVsyncScreen()
    {
        var isVSyncEnabled = transform.GetChild(3).GetChild(2).GetChild(1).GetChild(0).gameObject.activeSelf;

        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
        SettingsManager.Instance.Vsync = isVSyncEnabled;
    }

    /// <summary>
    /// Sets the selected language for localization.
    /// </summary>
    public void SaveLanguage()
    {
        var languageID = transform.GetChild(3).GetChild(3).GetChild(1).GetComponent<TMP_Dropdown>().value;
        SettingsManager.Instance.Language =
            (Language)languageID;
        ShowLanguageOptions();
        StartCoroutine(SetLanguage(languageID));
    }

    /// <summary>
    /// Display the language options based on active language.
    /// </summary>
    private void ShowLanguageOptions()
    {
        var language = SettingsManager.Instance.Language;
        var dropdown = transform.GetChild(3).GetChild(3).GetChild(1).GetComponent<TMP_Dropdown>();
        dropdown.options[0].text = language == Language.English ? "English" : "Engleza";
        dropdown.options[1].text = language == Language.English ? "Romanian" : "Romana";
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