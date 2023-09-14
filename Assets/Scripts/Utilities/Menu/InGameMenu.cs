using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manage menu in game paths.
/// </summary>
public class InGameMenu : MonoBehaviour
{
    /// <summary>
    /// Return to main menu.
    /// </summary>
    public void GoToMenu() => SceneManager.LoadScene("StartMenu");

    /// <summary>
    /// Restart current level.
    /// </summary>
    public void RestartLevel() => LvlManager.Instance.StartScene(LvlManager.Instance.CurrentLvl);

    /// <summary>
    /// Start next level.
    /// </summary>
    public void NextLevel() => LvlManager.Instance.StartScene(LvlManager.Instance.CurrentLvl + 1);

    /// <summary>
    /// Stop/Resume current level by modifying the time scale.
    /// </summary>
    /// <param name="pause">Whether to pause or resume game.</param>
    public void PauseGame(bool pause) => Time.timeScale = pause ? 0f : 1f;
}