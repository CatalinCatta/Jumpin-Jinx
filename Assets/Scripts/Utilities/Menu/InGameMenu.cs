using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    
    public void GoToMenu() =>
        SceneManager.LoadScene("StartMenu");

    public void RestartLevel() =>  
        SceneManager.LoadScene(currentLevel == 0 ? "EndlessRun" :  $"Lvl {currentLevel}");

    public void NextLevel() =>
        SceneManager.LoadScene($"Lvl {currentLevel+1}");
    
    public void PauseGame(bool pause) =>        
        Time.timeScale = pause? 0f : 1f;

}
