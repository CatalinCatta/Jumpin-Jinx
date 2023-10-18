using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class MenuSwitch : MonoBehaviour
{
    [SerializeField] private List<Button> backgroundButtons;
    [SerializeField] private Animator canvasAnimator;
    private Toggle _toggle;
    
    private void Start()
    {
        
        transform.GetChild(1).GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { KeyCode.F3 });
        canvasAnimator.SetBool("MenuOpened",
            (_toggle = GetComponent<Toggle>()).isOn = SettingsManager.Instance.IsMenuOpened);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3)) _toggle.isOn = !_toggle.isOn;
    }

    private void LateUpdate()
    {
        var animCurrentState = canvasAnimator.GetCurrentAnimatorStateInfo(0);
        if (!animCurrentState.IsName("AfkOpenedMenu") && !animCurrentState.IsName("AfkClosedMenu")) return;
        
        foreach (var backgroundButton in backgroundButtons) backgroundButton.interactable = !_toggle.isOn;
    }

    public void SwitchMenu() => canvasAnimator.SetBool("MenuOpened", _toggle.isOn);

}
