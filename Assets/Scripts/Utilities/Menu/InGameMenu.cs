using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public void GoToMenu() =>
        SceneManager.LoadScene("StartMenu");

    public void RestartLevel() =>
        LvlManager.Instance.StartLevel(LvlManager.Instance.currentLvl);  

    public void NextLevel() =>
        LvlManager.Instance.StartLevel(LvlManager.Instance.currentLvl + 1);  

    public void PauseGame(bool pause) =>        
        Time.timeScale = pause? 0f : 1f;

}
