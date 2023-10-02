using UnityEngine;
using Button = UnityEngine.UI.Button;
using System.Linq;

/// <summary>
/// Manage campaign menu behaviors.
/// </summary>
public class CampaignMenu : MonoBehaviour
{
    [SerializeField] private Transform lvlButtonsParent;

    public void MakeLvlButtonsInteractable()
    {
        for (var i = 0; i < 18; i++) lvlButtonsParent.GetChild(i).GetComponent<Button>().interactable = true;
    }
    public void MakeLvlButtonsNonInteractable() => lvlButtonsParent.GetComponentsInChildren<Button>().ToList()
        .ForEach(button => button.interactable = false);

}