using UnityEngine;
using UnityEngine.SceneManagement;  

public class Menu : MonoBehaviour
{
    public void StartEndlessRun() =>
        SceneManager.LoadScene("EndlessRun");
    
    public void Quit() =>
        Application.Quit();  
    
    public void StartLevel(int lvl) =>  
        SceneManager.LoadScene($"Lvl {lvl}");  

}
