using TMPro;
using UnityEngine;
using UnityEngine.Localization;

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

    public void SetObject(object[] obj)
    {
        _objects = obj;
        localizedString.Arguments = _objects;
        localizedString.RefreshString();
    }
}