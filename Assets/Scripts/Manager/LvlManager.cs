using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

        StartCoroutine(LoadAsync(lvl == 0 ? "EndlessRun" : "Grass Lvl Design"));
    }

    private IEnumerator LoadAsync(string scene)
    {
        var operation = SceneManager.LoadSceneAsync(scene);
        var loadingScreen = GameObject.Find("Loading Screen").transform;
        var progressbar = loadingScreen.GetChild(1); 
        var slider = progressbar.GetChild(0).GetComponent<Slider>();
        var percentage = progressbar.GetChild(2).GetComponent<TextMeshProUGUI>();
        
        loadingScreen.GetChild(0).gameObject.SetActive(true);
        progressbar.gameObject.SetActive(true);
        
        progressbar.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Some inspirational quote";
        
        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            percentage.text = Mathf.RoundToInt(progress * 100) + "%";
            
            yield return null;
        }
        
    }
}
