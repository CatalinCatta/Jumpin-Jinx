using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class ParameterizedLocalizedString : MonoBehaviour
{
    [SerializeField] private LocalizedString localizedString;
    // [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private TMP_Text textMeshPro;
    
    private object[] _objects;

    private void OnEnable()
    {
        localizedString.Arguments = _objects;
        localizedString.StringChanged += UpdateText;
    }

    private void OnDisable() =>
        localizedString.StringChanged -= UpdateText;

    private void UpdateText(string text) 
    {
        // if (textMeshProUGUI == null)
        textMeshPro.text = text;
        // else
        //     textMeshProUGUI.text = text;
        //
        // GetComponent<TMP_Text>().text = "1";
    }

    public void SetObject(object[] obj)
    {
        _objects = obj;
        localizedString.Arguments = _objects;
        localizedString.RefreshString();
    }
}