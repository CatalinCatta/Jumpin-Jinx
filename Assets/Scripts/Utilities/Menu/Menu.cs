using UnityEngine;

/// <summary>
/// Manage main menu paths. 
/// </summary>
[RequireComponent(typeof(Animator))]
public class Menu : MonoBehaviour
{
    private Animator _canvasAnimator;
    private SettingsManager _settingsManager;
    
    private void Start()
    {
        _settingsManager = SettingsManager.Instance;
        _canvasAnimator = GetComponent<Animator>();
        Time.timeScale = 1f;
        if (_settingsManager.LastMenuName != "") _canvasAnimator.Play(_settingsManager.LastMenuName);
    }

    /// <summary>
    /// Start endless game mode.
    /// </summary>
    public void StartEndlessRun()
    {
        PlayerManager.Instance.Save();
        LvlManager.Instance.StartScene((int)Scene.Endless);
    }

    /// <summary>
    /// Reopen main menu.
    /// </summary>
    public void Restart() => LvlManager.Instance.StartScene((int)Scene.Menu);

    /// <summary>
    /// Close app. 
    /// </summary>
    public void Quit() => Application.Quit();

    /// <summary>
    /// Start specific level from champaign game mode.
    /// </summary>
    /// <param name="lvl">Level to run.</param>
    /// <remarks>Levels starts form 1.</remarks>
    public void StartLevel(int lvl) => LvlManager.Instance.StartScene(lvl);

    public void OpenSettings()
    {
        _canvasAnimator.Play("CloseMenu");
        _canvasAnimator.SetTrigger("Settings");
    }
    
    public void OpenEndless()
    {
        _canvasAnimator.Play("CloseMenu");
        _canvasAnimator.SetTrigger("Endless");
    }
    
    public void OpenCustom()
    {
        _canvasAnimator.Play("CloseMenu");
        _canvasAnimator.SetTrigger("Custom");
    }
    
    public void OpenCampaign()
    {
        _canvasAnimator.Play("CloseMenu");
        _canvasAnimator.SetTrigger("Campaign");
    }

    public void SetCampaignLast() => _settingsManager.LastMenuName = "CampaignAfk";
    public void SetSettingsLast() => _settingsManager.LastMenuName = "SettingsAfk";
    public void SetEndlessLast() => _settingsManager.LastMenuName = "EndlessAfk";
    public void SetCustomLast() => _settingsManager.LastMenuName = "CustomLvlAfk";
}