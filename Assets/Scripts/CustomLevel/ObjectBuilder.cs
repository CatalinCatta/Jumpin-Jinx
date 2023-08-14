using UnityEngine;
using UnityEngine.UI;

public class ObjectBuilder : MonoBehaviour
{
    [SerializeField] public ObjectBuildType objectBuildType;
    
    private Image _image;
    private GameBuilder _gameBuilder;
    
    private void Awake() => 
        _gameBuilder = FindObjectOfType<GameBuilder>();

    private void Start() =>
        _image = transform.GetComponent<Image>();

    public void Deselected()
    {
        _gameBuilder.selectedObject = this;
        _image.color = Color.white;

        _gameBuilder.buildingPlacesParent.SetActive(false);
    }

    private void OnMouseDown()
    {
        _gameBuilder.selectedObject = null;
        _image.color = Color.green;
      
        _gameBuilder.buildingPlacesParent.SetActive(true);
    }
}
