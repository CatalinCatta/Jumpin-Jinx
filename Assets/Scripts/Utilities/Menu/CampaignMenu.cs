using UnityEngine;
using Button = UnityEngine.UI.Button;
using System.Linq;
using TMPro;

/// <summary>
/// Manage campaign menu behaviors.
/// </summary>
public class CampaignMenu : MonoBehaviour
{
    [SerializeField] private Transform lvlButtonsParent;
    private int _levelsAvailable;

    private void Start() => Initialize();

    public void MakeLvlButtonsInteractable()
    {
        for (var i = 0; i < _levelsAvailable; i++)
            lvlButtonsParent.GetChild(i).GetComponent<Button>().interactable = true;
    }
    
    public void MakeLvlButtonsNonInteractable() => lvlButtonsParent.GetComponentsInChildren<Button>().ToList()
        .ForEach(button => button.interactable = false);
    
    public void ResetPlayer()
    {
        SaveAndLoadSystem.DeleteCampaignSave();
        Initialize();
    }

    private void Initialize()
    {
        _levelsAvailable = 1;
        var data = SaveAndLoadSystem.LoadCampaign();
        if (data == null) SetUpLvl(lvlButtonsParent.GetChild(0), 0, 0);
        else
        {
            var dataLength = data.Count;
            _levelsAvailable = dataLength + 1;
            for (var i = 0; i < dataLength; i++)
            {
                var dataI = data[i];
                if (dataI.completed == false)
                {
                    SetUpLvl(lvlButtonsParent.GetChild(0), 0, 0);
                    return;
                }
                SetUpLvl(lvlButtonsParent.GetChild(i), dataI.maxStarNrObtained, dataI.bestTime);
            }
            if (dataLength < 18) SetUpLvl(lvlButtonsParent.GetChild(dataLength), 0, 0);
        }

        void SetUpLvl(Transform element, int starsCounter, float timer)
        {
            element.GetChild(1).gameObject.SetActive(false);
            element.GetChild(0).gameObject.SetActive(true);
            var elementActive = element.GetChild(0);
            var stars = elementActive.GetChild(1);
            for (var i = 0; i < (starsCounter > 3 ? 3 : starsCounter); i++)
                stars.GetChild(i).GetChild(0).gameObject.SetActive(true);
            elementActive.GetChild(2).GetComponent<TextMeshProUGUI>().text = Utility.TimeToString(timer);
        }
    }
}