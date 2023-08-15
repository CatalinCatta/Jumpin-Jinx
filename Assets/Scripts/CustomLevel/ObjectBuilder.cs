using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectBuilder : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public ObjectBuildType objectBuildType;
    
    private Image _image;
    private GameBuilder _gameBuilder;
    
    private void Awake() => 
        _gameBuilder = FindObjectOfType<GameBuilder>();

    private void Start() =>
        _image = transform.GetComponent<Image>();

    public void Deselect()
    {
        _gameBuilder.selectedObject = null;
        _image.color = Color.white;

        _gameBuilder.buildingPlacesParent.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var wasAlreadyPressed = _gameBuilder.selectedObject == this;
        
        _gameBuilder.DeselectCurrentItem();

        if (wasAlreadyPressed)
            return;
        
        _gameBuilder.selectedObject = this;
        _image.color = Color.green;
      
        _gameBuilder.buildingPlacesParent.SetActive(true);
    }
}
