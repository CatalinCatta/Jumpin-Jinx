using UnityEngine;
using UnityEngine.UI;

public class MenuLight : MonoBehaviour
{
    private bool _darkMode;

    [SerializeField] private Sprite darkSun;
    [SerializeField] private Sprite lightSun;

    private void Start()
    {
        _darkMode = SettingsManager.Instance.darkModeOn;
        SetUpTheme();
    }
    
    public void ChangeTheme()
    {
        _darkMode = !_darkMode;
        SettingsManager.Instance.darkModeOn = _darkMode;
        
        SetUpTheme();
    }

    private void SetUpTheme()
    {
        transform.parent.parent.GetComponent<Image>().color = _darkMode ? new Color(0.1f, 0.1f, 0.3f) : Color.cyan;
        transform.parent.GetComponent<Image>().color = _darkMode ? new Color(0.5f, 1f, 0.8f, 0.5f) : new Color(0f, 1f, 0.5f, 1f);

        var selfImage = transform.GetComponent<Image>();

        selfImage.color = _darkMode ? new Color(0.4f, 0.4f, 1) : Color.white;
        selfImage.sprite = _darkMode ? darkSun : lightSun;

        transform.GetChild(0).gameObject.SetActive(_darkMode);
    }
}
