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

    public void SwitchMenu()
    {
        activatedMenu = !activatedMenu;
        if (activatedMenu) ShowMenu();
        else CloseMenu();
    }
    
    private void ShowMenu() => canvasAnimator.SetBool("MenuOpened", true);
    
    private void CloseMenu()
    {
        if (canvasAnimator.GetCurrentAnimatorStateInfo(0).IsName("OpenMenu"))
        {
            canvasAnimator.Play("CloseMenu");
            canvasAnimator.SetTrigger("Afk");
        }
        canvasAnimator.SetBool("MenuOpened", false);
    }
}
