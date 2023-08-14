using UnityEngine;
using UnityEngine.UI;

public class BuildingPlace : MonoBehaviour
{
    private GameBuilder _gameBuilder;
    private Status _status;
    private Image _image;

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
        _image = transform.GetComponent<Image>();

    private void OnMouseOver()
    {
        switch (_gameBuilder.selectedObject.objectBuildType)
        {
            case < ObjectBuildType.Spike:
                if (_block == ObjectBuildType.Null)
                    break;
                _image.color = new Color(1f, 0.5f, 0f, 1f);
                _status = Status.FullByBlock;
                break;
            
            case < ObjectBuildType.Player:
                if (_environment == ObjectBuildType.Null) break;
                if (IsCompatibleWithEnvironmentAndPlants()) return;
                             
                _image.color = new Color(1f, 0.5f, 0f, 1f);
                _status = Status.FullByBlock;
                break;
            
            default:
                if (_object == ObjectBuildType.Null) break;
                if (IsCompatibleWithEnvironmentAndPlants()) return;

                _image.color = new Color(1f, 0.5f, 0f, 1f);
                _status = Status.FullByBlock;
                break;
        }
    }

    private bool IsCompatibleWithEnvironmentAndPlants()
    {
        if (_block >= ObjectBuildType.HalfSlopeDirt)
            return false;

        _status = Status.NotCompatible;
        _image.color = new Color(1f, 0f, 0f, 1f);
        return true;
    }

    private void OnMouseExit()
    {
        for (var i = 1; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        
        _image.color = new Color(1f, 1f, 1f, 0.5f);
    }
}
