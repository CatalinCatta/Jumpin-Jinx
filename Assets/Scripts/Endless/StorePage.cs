using UnityEngine;
using UnityEngine.UI;

public class StorePage : MonoBehaviour
{
    private GameObject _goldPage;
    private GameObject _gemPage;
    
    private Image _goldButtonImage;
    private Image _gemButtonImage;
    
    private Image _goldIcon;
    private Image _gemIcon;

    private void Awake()
    {
        var currentTransform = transform;
        var parentTransform = currentTransform.parent;

        _goldPage = parentTransform.GetChild(1).gameObject;
        _gemPage = parentTransform.GetChild(2).gameObject;
        
        var goldButton = currentTransform.GetChild(0);
        var gemButton = currentTransform.GetChild(1);

        _goldButtonImage = goldButton.GetComponent<Image>();
        _gemButtonImage = gemButton.GetComponent<Image>();
        
        _goldIcon = goldButton.GetChild(0).GetComponent<Image>();
        _gemIcon = gemButton.GetChild(0).GetComponent<Image>();
    }

    public void SelectGold()
    {
        _goldButtonImage.color = Color.white;
        _goldIcon.color = Color.white;

        _gemButtonImage.color = Color.gray;
        _gemIcon.color = Color.gray;
        
        _goldPage.SetActive(true);
        _gemPage.SetActive(false);
    }
    
    public void SelectGem()
    {
        _gemButtonImage.color = Color.white;
        _gemIcon.color = Color.white;
        
        _goldButtonImage.color = Color.gray;
        _goldIcon.color = Color.gray;

        _gemPage.SetActive(true);
        _goldPage.SetActive(false);
    }
}
