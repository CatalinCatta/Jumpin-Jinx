using UnityEngine;
using UnityEngine.UI;

public class BuildingPlace : MonoBehaviour
{
    private GameBuilder _gameBuilder;
    private Status _status;
    private SpriteRenderer _spriteRenderer;

    private ObjectBuildType _block;
    private ObjectBuildType _environment;
    private ObjectBuildType _object;
    
    private enum Status
    {
        FullByBlock,
        FullByPlant,
        FullByObject,
        Free,
        NotCompatible
    }
    
    private void Awake() => 
        _gameBuilder = FindObjectOfType<GameBuilder>();

    private void Start() =>
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();

    private void OnMouseOver()
    {
        var temporaryItemTransform = transform.GetChild(1);
        var temporaryItemRenderer = temporaryItemTransform.GetComponent<SpriteRenderer>();
        
        var selectedObject = _gameBuilder.selectedObject;
        var selectedTransform = selectedObject.transform;

        _spriteRenderer.color = new Color(0f, 1f, 0f, 0.5f);

        switch (selectedObject.objectBuildType)
        {
            case < ObjectBuildType.Spike:
                if (_block == ObjectBuildType.Null)
                    break;
                
                _spriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
                _status = Status.FullByBlock;
                ShowItems(false);
                break;
            
            case < ObjectBuildType.Player:
                if (_environment == ObjectBuildType.Null) break;
                if (IsCompatibleWithEnvironmentAndPlants()) return;
                             
                _spriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
                _status = Status.FullByBlock;
                transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                break;
            
            default:
                if (_object == ObjectBuildType.Null) break;
                if (IsCompatibleWithEnvironmentAndPlants()) return;

                _spriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
                _status = Status.FullByBlock;
                transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                break;
        }
   
        transform.GetChild(1).gameObject.SetActive(true);
        temporaryItemRenderer.sprite = selectedTransform.GetChild(0).GetComponent<Image>().sprite;
        temporaryItemTransform.rotation = selectedTransform.rotation;
    }

    private void ShowItems(bool show)
    {
        foreach (Transform child in transform.GetChild(0))
            child.gameObject.SetActive(show);
    }

    private bool IsCompatibleWithEnvironmentAndPlants()
    {
        if (_block >= ObjectBuildType.HalfSlopeDirt)
            return false;

        _status = Status.NotCompatible;
        _spriteRenderer.color = new Color(1f, 0f, 0f, 1f);
        return true;
    }

    private void OnMouseExit()
    {
        _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        ShowItems(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
