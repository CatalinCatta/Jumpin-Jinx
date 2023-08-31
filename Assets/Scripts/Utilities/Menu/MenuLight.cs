using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage game light theme.
/// </summary>
public class MenuLight : MonoBehaviour
{
    [Header("Sun Icons")] [SerializeField] private Sprite
        darkSun,
        lightSun;
 
    private bool _darkMode;

    private void Start()
    {
        _darkMode = SettingsManager.Instance.DarkModeOn;
        SetUpTheme();
    }

    /// <summary>
    /// Switch game theme (light/dark).
    /// </summary>
    public void ChangeTheme()
    {
        _darkMode = !_darkMode;
        SettingsManager.Instance.DarkModeOn = _darkMode;

        SetUpTheme();
    }

    private void SetUpTheme()
    {
        transform.parent.parent.GetComponent<Image>().color = _darkMode ? new Color(0.1f, 0.1f, 0.3f) : Color.cyan;
        transform.parent.GetComponent<Image>().color =
            _darkMode ? new Color(0.5f, 1f, 0.8f, 0.5f) : new Color(0f, 1f, 0.5f, 1f);

        var selfImage = transform.GetComponent<Image>();

        selfImage.color = _darkMode ? new Color(0.4f, 0.4f, 1) : Color.white;
        selfImage.sprite = _darkMode ? darkSun : lightSun;

        transform.GetChild(0).gameObject.SetActive(_darkMode);
    }
}