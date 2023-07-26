using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        SceneManager.LoadScene(lvl== 0 ? "EndlessRun" : "Grass Lvl Design");
    }
}
