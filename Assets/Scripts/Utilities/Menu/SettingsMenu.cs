using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class SettingsMenu : MonoBehaviour
{
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
    
    public void HideSettings()
    {
        StartCoroutine(MoveObject(SettingsFrames.Categories, false, false));
        StartCoroutine(MoveObject(SettingsManager.Instance.currentCategoryTab, false, false));
    }

    private IEnumerator MoveObject(SettingsFrames settingsFrames, bool isAppearing, bool verticalRotation)
    {
        var currentTime = 0f;
        var objectToMove = transform.GetChild((int)settingsFrames).GetComponent<RectTransform>();
        
        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;
            
            if (settingsFrames == SettingsFrames.Categories)
                objectToMove.anchoredPosition = new Vector2(Mathf.Lerp(isAppearing? -600f : 0f, isAppearing? 0f : -600f, currentTime / 0.5f), objectToMove.anchoredPosition.y);
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

    public void ChangeCategory(int settingsFrames) =>
        StartCoroutine(ChangeCategoryAnimation((SettingsFrames)settingsFrames));

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
        
        var genericSoundsObject = soundsObject.GetChild(0);
        var genericVolume = SettingsManager.Instance.generalVolume;
        genericSoundsObject.GetChild(1).GetComponent<Slider>().value = genericVolume;
        genericSoundsObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(genericVolume * 100) + "%";
        
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
        vSyncToggleObject.GetChild(0).gameObject.SetActive(SettingsManager.Instance.fullscreen);
        vSyncToggleObject.GetChild(1).gameObject.SetActive(!SettingsManager.Instance.fullscreen);
        
        var brightnessObject = displayObject.GetChild(3);
        var brightnessVolume = SettingsManager.Instance.brightness;
        brightnessObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Brightness" : "Luminozitate";
        brightnessObject.GetChild(1).GetComponent<Slider>().value = brightnessVolume;
        brightnessObject.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(brightnessVolume * 100) + "%";
        
        
        var languageObject = displayObject.GetChild(4);
        var dropdown = languageObject.GetChild(1).GetComponent<TMP_Dropdown>();
        languageObject.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Language" : "Limba";
        dropdown.value = (int)SettingsManager.Instance.language;
        dropdown.options[0].text = "Engleza";
        dropdown.options[1].text = "Romana";
    }
}
