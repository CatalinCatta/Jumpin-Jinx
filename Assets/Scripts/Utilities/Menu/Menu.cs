using UnityEngine;

/// <summary>
/// Manage main menu paths. 
/// </summary>
public class Menu : MonoBehaviour
{
    private void Start() => Time.timeScale = 1f;

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
}