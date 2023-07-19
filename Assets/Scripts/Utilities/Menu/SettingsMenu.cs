using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class SettingsMenu : MonoBehaviour
{
    private bool _isCheckingForJump;
    private bool _isCheckingForMoveLeft;
    private bool _isCheckingForMoveRight;
    private bool _isCheckingForFire;
    private bool _isCheckingForSpeedBuff;
    private bool _isCheckingForJumpBuff;
    private bool _isCheckingForPause;
    
    private void OnEnable()
    {
        StartCoroutine(MoveObject(SettingsFrames.Categories, true, false));

        switch (SettingsManager.Instance.currentCategoryTab)
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
        
        StartCoroutine(MoveObject(SettingsManager.Instance.currentCategoryTab, true, false));
    }

    private void OnDisable() => DeactivateAllChecking();
    
    private void Update()
    {
        if (!_isCheckingForJump && !_isCheckingForMoveLeft && !_isCheckingForMoveRight && !_isCheckingForFire && !_isCheckingForSpeedBuff && !_isCheckingForJumpBuff && !_isCheckingForPause)
            return;
        
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKeyDown(keyCode) || KeyAllreadyInUse(keyCode))
                continue;
            
            if(_isCheckingForJump)
            {
                transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.jumpKeyCode = keyCode;
                _isCheckingForJump = false;
                return;
            }
            if(_isCheckingForMoveLeft)
            {
                transform.GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.moveLeftKeyCode = keyCode;
                _isCheckingForMoveLeft = false;
                return;
            }
            if(_isCheckingForMoveRight)
            {
                transform.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.moveRightKeyCode = keyCode;
                _isCheckingForMoveRight = false;
                return;
            }
            if(_isCheckingForFire)
            {
                transform.GetChild(2).GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.fireKeyCode = keyCode;
                _isCheckingForFire = false;
                return;
            }
            if(_isCheckingForSpeedBuff)
            {
                transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.speedBuffKeyCode = keyCode;
                _isCheckingForSpeedBuff = false;
                return;
            }
            if(_isCheckingForJumpBuff)
            {
                transform.GetChild(2).GetChild(5).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.jumpBuffKeyCode = keyCode;
                _isCheckingForJumpBuff = false;
                return;
            }
            if(_isCheckingForPause)
            {
                transform.GetChild(2).GetChild(6).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), keyCode)!, @"(\p{Lu})", " $1").Trim();
                SettingsManager.Instance.pauseKeyCode = keyCode;
                _isCheckingForPause = false;
                return;
            }
        }
    }

    public void HideSettings()
    {
        StartCoroutine(MoveObject(SettingsFrames.Categories, false, false));
        StartCoroutine(MoveObject(SettingsManager.Instance.currentCategoryTab, false, false));
    }

    private bool KeyAllreadyInUse(KeyCode keyCode)
    {
        var allKeyCodes = new List<KeyCode>
        {
            SettingsManager.Instance.jumpKeyCode,
            SettingsManager.Instance.moveLeftKeyCode,
            SettingsManager.Instance.moveRightKeyCode,
            SettingsManager.Instance.fireKeyCode,
            SettingsManager.Instance.speedBuffKeyCode,
            SettingsManager.Instance.jumpBuffKeyCode,
            SettingsManager.Instance.pauseKeyCode
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
    
    
    private IEnumerator MoveObject(SettingsFrames settingsFrames, bool isAppearing, bool verticalRotation)
    {
        var currentTime = 0f;
        var objectToMove = transform.GetChild((int)settingsFrames).GetComponent<RectTransform>();
        
        objectToMove.gameObject.SetActive(true);
        
        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;
            
            if (settingsFrames == SettingsFrames.Categories)
                objectToMove.anchoredPosition = 
                    new Vector2(Mathf.Lerp(isAppearing? -600f : 0f, isAppearing? 0f : -600f, currentTime / 0.5f), objectToMove.anchoredPosition.y);
            else
            {
                if (verticalRotation)
                    objectToMove.rotation = 
                        Quaternion.Euler(Mathf.Lerp(isAppearing? 90f : 0f, isAppearing? 0f : 90f, currentTime / 0.5f), 0f, 0f);
                else
                {
                    objectToMove.rotation = 
                        Quaternion.Euler(0f, 0f, Mathf.Lerp(isAppearing? -90f : 0f, isAppearing? 0f : -90f, currentTime / 0.5f));
                    objectToMove.anchoredPosition = 
                        new Vector2(Mathf.Lerp(isAppearing? 600f : 0f , isAppearing? 0f : 600f, currentTime / 0.5f), objectToMove.anchoredPosition.y);
                }
            }

            yield return null;
        }

        objectToMove.anchoredPosition = new Vector2(0f, objectToMove.anchoredPosition.y);
        objectToMove.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (settingsFrames == SettingsFrames.Categories && !isAppearing)
            gameObject.SetActive(false);
    }

    public void ChangeCategory(int settingsFrames)
    {
        if (settingsFrames == 2)
            DeactivateAllChecking();
        
        StartCoroutine(ChangeCategoryAnimation((SettingsFrames)settingsFrames));
    }
    
    private IEnumerator ChangeCategoryAnimation(SettingsFrames settingsFrames)
    {
        StartCoroutine(MoveObject(SettingsManager.Instance.currentCategoryTab, false, true));

        yield return new WaitForSeconds(0.5f);
        
        transform.GetChild((int)SettingsManager.Instance.currentCategoryTab).gameObject.SetActive(false);
        
        transform.GetChild((int)settingsFrames).gameObject.SetActive(true);
        
        SettingsManager.Instance.currentCategoryTab = settingsFrames;

        StartCoroutine(MoveObject(settingsFrames, true, true));
    }

    public void ShowSounds()
    {
        var soundsObject = transform.GetChild(1);
        
        var generalSoundsObject = soundsObject.GetChild(0);
        var generalVolume = SettingsManager.Instance.generalVolume;
        generalSoundsObject.GetChild(1).GetComponent<Slider>().value = generalVolume;
        generalSoundsObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(generalVolume * 100) + "%";
        
        var musicSoundsObject = soundsObject.GetChild(1);
        var musicVolume = SettingsManager.Instance.musicVolume;
        musicSoundsObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            SettingsManager.Instance.language == Language.English ? "Music" : "Muzica";
        musicSoundsObject.GetChild(1).GetComponent<Slider>().value = musicVolume;
        musicSoundsObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(musicVolume * 100) + "%";
        
        var sfxSoundsObject = soundsObject.GetChild(2);
        var sfxVolume = SettingsManager.Instance.soundEffectVolume;
        sfxSoundsObject.GetChild(1).GetComponent<Slider>().value = sfxVolume;
        sfxSoundsObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(sfxVolume * 100) + "%";
    }

    public void ShowControls()
    {
        var controlsObject = transform.GetChild(2);
        var language = SettingsManager.Instance.language;
        
        var jumpObject = controlsObject.GetChild(0);
        jumpObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Jump" : "Salt";
        jumpObject.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.jumpKeyCode)!, @"(\p{Lu})", " $1").Trim();

        var moveLeftObject = controlsObject.GetChild(1);
        moveLeftObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Move Left" : "Miscare La Stanga";
        moveLeftObject.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.moveLeftKeyCode)!, @"(\p{Lu})", " $1").Trim();

        var moveRightObject = controlsObject.GetChild(2);
        moveRightObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Move Right" : "Miscare La Dreapta";
        moveRightObject.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.moveRightKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        var fireObject = controlsObject.GetChild(3);
        fireObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Fire" : "Trage";
        fireObject.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.fireKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        var speedBuffObject = controlsObject.GetChild(4);
        speedBuffObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Speed Buff" : "Bonus De Viteza";
        speedBuffObject.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.speedBuffKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        var jumpBuffObject = controlsObject.GetChild(5);
        jumpBuffObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Jump Buff" : "Bonus De Saritura";
        jumpBuffObject.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.jumpBuffKeyCode)!, @"(\p{Lu})", " $1").Trim();

        var pauseObject = controlsObject.GetChild(6);
        pauseObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Pause" : "Pauza";
        pauseObject.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.pauseKeyCode)!, @"(\p{Lu})", " $1").Trim();
    }

    public void ShowDisplay()
    {
        SettingsManager.Instance.resolution = Utils.ResolutionTupleToIndex(Screen.currentResolution);
        SettingsManager.Instance.fullscreen = Screen.fullScreen;
        SettingsManager.Instance.vsync = QualitySettings.vSyncCount > 0;
        
        var displayObject = transform.GetChild(3);
        var language = SettingsManager.Instance.language;

        var resolutionObject = displayObject.GetChild(0);
        resolutionObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Resolution" : "Rezolutie";
        resolutionObject.GetChild(1).GetComponent<TMP_Dropdown>().value = SettingsManager.Instance.resolution;

        var fullscreenObject = displayObject.GetChild(1);
        var fullscreenToggleObject = fullscreenObject.GetChild(1);
        fullscreenObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Fullscreen" : "Ecran Complet";
        fullscreenToggleObject.GetChild(0).gameObject.SetActive(SettingsManager.Instance.fullscreen);
        fullscreenToggleObject.GetChild(1).gameObject.SetActive(!SettingsManager.Instance.fullscreen);
        
        var vSyncToggleObject = displayObject.GetChild(2).GetChild(1);
        vSyncToggleObject.GetChild(0).gameObject.SetActive(SettingsManager.Instance.vsync);
        vSyncToggleObject.GetChild(1).gameObject.SetActive(!SettingsManager.Instance.vsync);
        
        var languageObject = displayObject.GetChild(3);
        var dropdownObject = languageObject.GetChild(1);
        var dropdown = dropdownObject.GetComponent<TMP_Dropdown>();
        languageObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Language" : "Limba";
        dropdown.value = (int)SettingsManager.Instance.language;
        dropdown.options[0].text = language == Language.English ? "English" : "Engleza";
        dropdown.options[1].text = language == Language.English ? "Romanian" : "Romana";
        dropdownObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "English" : "Romana";
    }

    private void DeactivateAllChecking()
    {
        _isCheckingForJump = false;
        transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.jumpKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        _isCheckingForMoveLeft = false;
        transform.GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.moveLeftKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        _isCheckingForMoveRight = false;
        transform.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.moveRightKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        _isCheckingForFire = false;
        transform.GetChild(2).GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.fireKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        _isCheckingForSpeedBuff = false;
        transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.speedBuffKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        _isCheckingForJumpBuff = false;
        transform.GetChild(2).GetChild(5).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.jumpBuffKeyCode)!, @"(\p{Lu})", " $1").Trim();
        
        _isCheckingForPause = false;
        transform.GetChild(2).GetChild(6).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Regex.Replace(Enum.GetName(typeof(KeyCode), SettingsManager.Instance.pauseKeyCode)!, @"(\p{Lu})", " $1").Trim();
    }
    
    public void SaveGeneralSoundsVolume()
    {
        var generalSoundObject = transform.GetChild(1).GetChild(0);
        var value = generalSoundObject.GetChild(1).GetComponent<Slider>().value;
        SettingsManager.Instance.generalVolume = value;
        generalSoundObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(value * 100) + "%";
        SettingsManager.SetUpSound(FindObjectOfType<Camera>().transform);
    }
    
    public void SaveMusicVolume()
    {
        var musicObject = transform.GetChild(1).GetChild(1);
        var value = musicObject.GetChild(1).GetComponent<Slider>().value;
        SettingsManager.Instance.musicVolume = value;
        musicObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(value * 100) + "%";
        SettingsManager.SetUpSound(FindObjectOfType<Camera>().transform);
    }
    
    public void SaveSfxSound()
    {
        var sfxObject = transform.GetChild(1).GetChild(2);
        var value = sfxObject.GetChild(1).GetComponent<Slider>().value;
        SettingsManager.Instance.soundEffectVolume = value;
        sfxObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void SaveJumpValue()
    {
        DeactivateAllChecking();
        _isCheckingForJump = true;
        transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    
    public void SaveMoveLeftValue()
    {
        DeactivateAllChecking();
        _isCheckingForMoveLeft = true;
        transform.GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    
    public void SaveMoveRightValue()
    {
        DeactivateAllChecking();
        _isCheckingForMoveRight = true;
        transform.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    
    public void SaveFireValue()
    {
        DeactivateAllChecking();
        _isCheckingForFire = true;
        transform.GetChild(2).GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    
    public void SaveSpeedBuffValue()
    {
        DeactivateAllChecking();
        _isCheckingForSpeedBuff = true;
        transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    
    public void SaveJumpBuffValue()
    {
        DeactivateAllChecking();
        _isCheckingForJumpBuff = true;
        transform.GetChild(2).GetChild(5).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
    
    public void SavePauseValue()
    {
        DeactivateAllChecking();
        _isCheckingForPause = true;
        transform.GetChild(2).GetChild(6).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    public void SaveResolution()
    {
        var isFullScreen = SettingsManager.Instance.fullscreen;
        var resolutionBeforeTransformation =
            transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value;
        var resolution = Utils.ResolutionIndexToTuple(resolutionBeforeTransformation);
     
        SettingsManager.Instance.resolution = resolutionBeforeTransformation;
        Screen.SetResolution(resolution.Item1, resolution.Item2, isFullScreen);
    }
    
    public void SaveFullscreen()
    {
        var isFullScreen = transform.GetChild(3).GetChild(1).GetChild(1).GetChild(0).gameObject.activeSelf;
        var resolution = Utils.ResolutionIndexToTuple(SettingsManager.Instance.resolution);
    
        SettingsManager.Instance.fullscreen = isFullScreen;
        Screen.SetResolution(resolution.Item1, resolution.Item2, isFullScreen);
    }

    
    public void SaveVsyncScreen()
    {
        var isVSyncEnabled = transform.GetChild(3).GetChild(2).GetChild(1).GetChild(0).gameObject.activeSelf;

        QualitySettings.vSyncCount = isVSyncEnabled? 1 : 0;
        SettingsManager.Instance.vsync = isVSyncEnabled;
    }
    
    public void SaveLanguage()
    {
        SettingsManager.Instance.language =
            (Language)transform.GetChild(3).GetChild(3).GetChild(1).GetComponent<TMP_Dropdown>().value;
        ShowDisplay();
        FindObjectOfType<Menu>().SetUpLanguage();
    }
}
