using UnityEngine;

public class Menu : MonoBehaviour
{
    private void Start() =>
        Time.timeScale = 1f;

    public void StartEndlessRun() =>
        LvlManager.Instance.StartLevel(0);

    public void Quit() =>
        Application.Quit();

    public void StartLevel(int lvl) =>
        LvlManager.Instance.StartLevel(lvl);
}