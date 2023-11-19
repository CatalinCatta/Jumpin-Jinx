using UnityEngine;

public class SkinHandlerButton : MonoBehaviour
{
    [SerializeField] private bool isPrevButton;
    [SerializeField] private bool isNextButton;
    [SerializeField] private bool isPurchaseButton;
    [SerializeField] private BodyPartSelector bodyPartSelector;
    
    private void OnMouseDown()
    {
        if (isPrevButton) bodyPartSelector.PrevSkin();
        else if (isNextButton) bodyPartSelector.NextSkin();
        else if (isPurchaseButton) bodyPartSelector.Buy();
    }
}