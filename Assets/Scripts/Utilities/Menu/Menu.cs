using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Start() =>
        Time.timeScale = 1f;

    public void StartEndlessRun()
    {
        PlayerManager.Instance.Save();
        LvlManager.Instance.StartLevel(0);
    }

    public void Restart() =>
        SceneManager.LoadScene("StartMenu");
    
    public void Quit() =>
        Application.Quit();

    public void StartLevel(int lvl) =>
        LvlManager.Instance.StartLevel(lvl);
}