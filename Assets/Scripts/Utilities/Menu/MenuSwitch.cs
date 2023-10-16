using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSwitch : MonoBehaviour
{
    [SerializeField] private List<Button> backgroundButtons;
    [SerializeField] private Animator canvasAnimator;
    private Toggle _toggle;
    private bool activatedMenu;
    
    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        transform.GetChild(1).GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { KeyCode.F3 });

        activatedMenu = _toggle.isOn; 
        if (!activatedMenu) canvasAnimator.Play("Afk");
        canvasAnimator.SetBool("MenuOpened", activatedMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3)) _toggle.isOn = !activatedMenu;
    }

    private void LateUpdate()
    {
        var animCurrentState = canvasAnimator.GetCurrentAnimatorStateInfo(0);

        if (!animCurrentState.IsName("OpenMenu") && !animCurrentState.IsName("Afk")) return;
        
        foreach (var backgroundButton in backgroundButtons) backgroundButton.interactable = !activatedMenu;
    }

    public void SwitchMenu() => canvasAnimator.SetBool("MenuOpened", activatedMenu = !activatedMenu);

}
