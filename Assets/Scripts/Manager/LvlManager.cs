using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LvlManager : MonoBehaviour
{
    public static LvlManager Instance;

    public int currentLvl;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLevel(int lvl)
    {
        currentLvl = lvl;
        SettingsManager.Instance.Save();

        StartCoroutine(LoadAsync(lvl == 0 ? "EndlessRun" : "Grass Lvl", lvl==13));
    }

    private static IEnumerator LoadAsync(string scene, bool delay)
    {
        var loadingScreen = GameObject.Find("Loading Screen").transform;
        var progressbar = loadingScreen.GetChild(1); 
        
        loadingScreen.GetChild(0).gameObject.SetActive(true);
        progressbar.gameObject.SetActive(true);
        
        progressbar.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Some inspirational quote";

        if (delay)
            yield return new WaitForSeconds(10);
        
        var operation = SceneManager.LoadSceneAsync(scene);
        var slider = progressbar.GetChild(0).GetComponent<Slider>();
        var percentage = progressbar.GetChild(2).GetComponent<TextMeshProUGUI>();
        
        
        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            percentage.text = Mathf.RoundToInt(progress * 100) + "%";
            
            yield return null;
        }
        
    }
}
