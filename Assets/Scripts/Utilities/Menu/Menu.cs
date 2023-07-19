using UnityEngine;
using UnityEngine.SceneManagement;  
using TMPro;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;
        SetUpLanguage();
    }

    public void SetUpLanguage()
    {
        var language = SettingsManager.Instance.language;
  
        var menu = transform.GetChild(1);
        menu.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Play" : "Joaca";
        menu.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Endless" : "Nelimitat";
        menu.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Settings" : "Setari";
        menu.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Quit" : "Iesi";
        
        var settings = transform.GetChild(3).GetChild(0);
        settings.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Settings" : "Setari";
        settings.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Sounds" : "Sunete";
        settings.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Controls" : "Comenzi";
        settings.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Display" : "Afisaj";
        settings.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Back" : "Inapoi";
    }

    public void StartEndlessRun() =>
        SceneManager.LoadScene("EndlessRun");
    
    public void Quit() =>
        Application.Quit();  
    
    public void StartLevel(int lvl) =>  
        SceneManager.LoadScene($"Lvl {lvl}");  

}
