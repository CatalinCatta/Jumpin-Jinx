using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private Transform menu;
    [SerializeField] private Transform settingsCategories;
    
    private void Start()
    {
        Time.timeScale = 1f;
        SetUpLanguage();
    }

    public void SetUpLanguage()
    {
        var language = SettingsManager.Instance.language;
  
        menu.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Play" : "Joaca";
        menu.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Endless" : "Nelimitat";
        menu.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Settings" : "Setari";
        menu.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            language == Language.English ? "Quit" : "Iesi";
        
        settingsCategories.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Settings" : "Setari";
        settingsCategories.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Sounds" : "Sunete";
        settingsCategories.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Controls" : "Comenzi";
        settingsCategories.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Display" : "Afisaj";
        settingsCategories.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = 
            language == Language.English ? "Back" : "Inapoi";
    }

    public void StartEndlessRun() =>
        LvlManager.Instance.StartLevel(0);
    
    public void Quit() =>
        Application.Quit();

    public void StartLevel(int lvl) =>
        LvlManager.Instance.StartLevel(lvl);

}
