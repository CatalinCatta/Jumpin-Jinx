using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents an object that can be built in the game.
/// Handles selection and deselection of build objects.
/// </summary>
public class ObjectBuilder : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public ObjectBuildType objectBuildType;

    private GameBuilder _gameBuilder;
    private Image _image;

    private void Awake() => _gameBuilder = FindObjectOfType<GameBuilder>();

    private void Start() => _image = transform.GetComponent<Image>();

    public void OnPointerDown(PointerEventData eventData)
    {
        var wasAlreadyPressed = _gameBuilder.SelectedObject == this;

        _gameBuilder.DeselectCurrentItem();

        if (wasAlreadyPressed) return;

        _gameBuilder.SelectedObject = this;
        _image.color = Color.green;

        _gameBuilder.ShowBuildPlaces(true);
    }
}