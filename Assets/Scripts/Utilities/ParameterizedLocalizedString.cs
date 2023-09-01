using TMPro;
using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// Displays a localized string with parameterized arguments using TMPro.
/// </summary>
public class ParameterizedLocalizedString : MonoBehaviour
{
    [SerializeField] private LocalizedString localizedString;
    [SerializeField] private TMP_Text textMeshPro;
    private object[] _objects;
    
    private void OnEnable()
    {
        localizedString.Arguments = _objects;
        localizedString.StringChanged += UpdateText;
    }

    private void OnDisable() =>
        localizedString.StringChanged -= UpdateText;

    private void UpdateText(string text) =>
        textMeshPro.text = text;

    /// <summary>
    /// Sets the object array for parameterized arguments and updates the localized string.
    /// </summary>
    /// <param name="obj">Array of objects to be used as arguments.</param>
    public void SetObject(object[] obj)
    {
        _objects = obj;
        localizedString.Arguments = _objects;
        localizedString.RefreshString();
    }
}