using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a place where players can construct objects or structures.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BuildingPlace : MonoBehaviour
{
    [NonSerialized] public ObjectBuildType Block, Environment, Object;
    
    [Header("Neighbors")] private BuildingPlace _buildingPlaceDown, _buildingPlaceUp;

    private GameBuilder _gameBuilder;
    private Transform _localTransform;
    private SpriteRenderer _spriteRenderer;

    private void Awake() => _gameBuilder = FindObjectOfType<GameBuilder>();

    private void Start()
    {
        _localTransform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        var position = _localTransform.position;
        var buildingPlaceUpTransform =
            Mathf.RoundToInt(position.y / 1.28f) == Mathf.RoundToInt((_gameBuilder.Rows - 1f) / 2f)
                ? null
                : GetBuildingPlaceFromDirection(Directions.Up);
        var buildingPlaceDownTransform =
            Mathf.RoundToInt(position.y / 1.28f) == -Mathf.RoundToInt((_gameBuilder.Rows - 1f) / 2f)
                ? null
                : GetBuildingPlaceFromDirection(Directions.Down);

        if (buildingPlaceUpTransform != null)
            buildingPlaceUpTransform.TryGetComponent(out _buildingPlaceUp);

        if (buildingPlaceDownTransform != null)
            buildingPlaceDownTransform.TryGetComponent(out _buildingPlaceDown);
    }

    private void OnMouseExit()
    {
        if (_gameBuilder.SelectedObject == null && !_gameBuilder.DeleteMode) return;

        _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
        ShowItems(_localTransform.GetChild(0), true);

        if (_buildingPlaceUp != null)
            ShowItems(_buildingPlaceUp.transform.GetChild(0), true);

        if (_buildingPlaceDown != null)
            ShowItems(_buildingPlaceDown.transform.GetChild(0), true);

        _localTransform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (_gameBuilder.SelectedObject == null && !_gameBuilder.DeleteMode) return;

        var isMousePressed = Input.GetMouseButton(0);

        if (_gameBuilder.DeleteMode)
            Delete(isMousePressed);
        else
        {
            if (_gameBuilder.SelectedObject == null) return;
            Build(isMousePressed);
        }
    }

    private void Delete(bool isDestroying)
    {
        _localTransform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;

        if (Block != ObjectBuildType.Null || Environment != ObjectBuildType.Null || Object != ObjectBuildType.Null)
            _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);

        if (isDestroying)
        {
            if (Block != ObjectBuildType.Null && _buildingPlaceUp != null &&
                _buildingPlaceUp.Environment != ObjectBuildType.Null)
                ClearItems(_buildingPlaceUp.transform.GetChild(0), _buildingPlaceUp);
            ClearItems(transform.GetChild(0), this);
        }
        else
        {
            if (Block != ObjectBuildType.Null && _buildingPlaceUp != null &&
                _buildingPlaceUp.Environment != ObjectBuildType.Null)
                ShowItems(_buildingPlaceUp.transform.GetChild(0), false);
            ShowItems(transform.GetChild(0), false);
            _localTransform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void Build(bool isConstructing)
    {
        var finalItemTransform = _localTransform.GetChild(0);
        var finalBlockItemTransform = finalItemTransform.GetChild(0);
        var finalEnvironmentItemTransform = finalItemTransform.GetChild(1);
        var finalObjectItemTransform = finalItemTransform.GetChild(2);

        var finalBlockItemSpriteRender = finalBlockItemTransform.GetComponent<SpriteRenderer>();
        var finalEnvironmentItemSpriteRender = finalEnvironmentItemTransform.GetComponent<SpriteRenderer>();
        var finalObjectItemSpriteRender = finalObjectItemTransform.GetComponent<SpriteRenderer>();

        var temporaryItemTransform = _localTransform.GetChild(1);
        var temporaryItemRenderer = temporaryItemTransform.GetComponent<SpriteRenderer>();

        var selectedObject = _gameBuilder.SelectedObject;
        var selectedObjectTransform = selectedObject.transform;
        var selectedObjectSprite = selectedObjectTransform.GetChild(0).GetComponent<Image>().sprite;

        _spriteRenderer.color = new Color(0f, 1f, 0f, .5f);

        switch (selectedObject.objectBuildType)
        {
            case < ObjectBuildType.HalfSlopeDirt:
                if (isConstructing)
                {
                    temporaryItemTransform.gameObject.SetActive(false);
                    ShowItems(finalItemTransform, true);

                    ClearItems(finalItemTransform, this);
                    _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
                    Block = selectedObject.objectBuildType;
                    finalBlockItemSpriteRender.sprite = selectedObjectSprite;
                }
                else
                {
                    temporaryItemTransform.gameObject.SetActive(true);
                    ShowItems(finalItemTransform, false);

                    if (Block != ObjectBuildType.Null || Environment != ObjectBuildType.Null ||
                        Object != ObjectBuildType.Null)
                        _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);

                    temporaryItemRenderer.sprite = selectedObjectSprite;
                }

                break;

            case < ObjectBuildType.Spike:
                if (isConstructing)
                {
                    temporaryItemTransform.gameObject.SetActive(false);
                    ShowItems(finalItemTransform, true);

                    ClearItems(finalItemTransform, this);
                    _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
                    Block = selectedObject.objectBuildType;
                    finalBlockItemSpriteRender.sprite = selectedObjectSprite;

                    if (_buildingPlaceUp != null && (_buildingPlaceUp.Environment != ObjectBuildType.Null ||
                                                     _buildingPlaceUp.Object != ObjectBuildType.Null))
                        ClearItems(_buildingPlaceUp.transform, _buildingPlaceUp);
                }
                else
                {
                    temporaryItemTransform.gameObject.SetActive(true);
                    ShowItems(finalItemTransform, false);

                    if (_buildingPlaceUp != null && (_buildingPlaceUp.Environment != ObjectBuildType.Null ||
                                                     _buildingPlaceUp.Object != ObjectBuildType.Null))
                    {
                        ShowItems(_buildingPlaceUp.transform, false);
                        _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
                    }
                    else if (Block != ObjectBuildType.Null || Environment != ObjectBuildType.Null ||
                             Object != ObjectBuildType.Null)
                    {
                        _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
                    }

                    temporaryItemRenderer.sprite = selectedObjectSprite;
                }

                break;

            case < ObjectBuildType.Player:
                if (_buildingPlaceDown == null ||
                    _buildingPlaceDown.Block is >= ObjectBuildType.HalfSlopeDirt or < ObjectBuildType.Dirt)
                {
                    _spriteRenderer.color = new Color(1f, 0f, 0f, .5f);
                    return;
                }

                if (isConstructing)
                {
                    temporaryItemTransform.gameObject.SetActive(false);
                    ShowItems(finalItemTransform, true);

                    finalBlockItemSpriteRender.sprite = null;
                    finalEnvironmentItemSpriteRender.sprite = selectedObjectSprite;

                    Environment = selectedObject.objectBuildType;
                    Block = ObjectBuildType.Null;

                    _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);

                    if (_buildingPlaceUp != null && _buildingPlaceUp.Environment != ObjectBuildType.Null)
                    {
                        _buildingPlaceUp.Environment = ObjectBuildType.Null;
                        _buildingPlaceUp.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
                    }
                }
                else
                {
                    temporaryItemTransform.gameObject.SetActive(true);
                    ShowItems(finalItemTransform, false);
                    finalItemTransform.GetChild(2).gameObject.SetActive(true);

                    if (_buildingPlaceUp != null && _buildingPlaceUp.Environment != ObjectBuildType.Null)
                    {
                        _buildingPlaceUp.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
                    }
                    else if (Block != ObjectBuildType.Null || Environment != ObjectBuildType.Null)
                    {
                        _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
                    }

                    temporaryItemRenderer.sprite = selectedObjectSprite;
                }

                break;

            default:
                if (_buildingPlaceDown == null ||
                    _buildingPlaceDown.Block is >= ObjectBuildType.HalfSlopeDirt or < ObjectBuildType.Dirt)
                {
                    _spriteRenderer.color = new Color(1f, 0f, 0f, .5f);
                    return;
                }

                if (isConstructing)
                {
                    temporaryItemTransform.gameObject.SetActive(false);
                    ShowItems(finalItemTransform, true);

                    finalBlockItemSpriteRender.sprite = null;
                    finalObjectItemSpriteRender.sprite = selectedObjectSprite;

                    Object = selectedObject.objectBuildType;
                    Block = ObjectBuildType.Null;

                    _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);

                    if (_buildingPlaceUp != null && _buildingPlaceUp.Environment != ObjectBuildType.Null)
                    {
                        _buildingPlaceUp.Environment = ObjectBuildType.Null;
                        _buildingPlaceUp.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
                    }
                }
                else
                {
                    temporaryItemTransform.gameObject.SetActive(true);
                    ShowItems(finalItemTransform, false);
                    finalItemTransform.GetChild(1).gameObject.SetActive(true);

                    if (_buildingPlaceUp != null && _buildingPlaceUp.Environment != ObjectBuildType.Null)
                    {
                        _buildingPlaceUp.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
                    }
                    else if (Block != ObjectBuildType.Null || Object != ObjectBuildType.Null)
                    {
                        _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
                    }

                    temporaryItemRenderer.sprite = selectedObjectSprite;
                }

                break;
        }
    }

    private static void ShowItems(Transform buildPlaceTransform, bool show)
    {
        foreach (Transform child in buildPlaceTransform)
            child.gameObject.SetActive(show);
    }

    private static void ClearItems(Transform buildPlaceTransform, BuildingPlace buildingPlace)
    {
        buildingPlace.Block = ObjectBuildType.Null;
        buildingPlace.Environment = ObjectBuildType.Null;
        buildingPlace.Object = ObjectBuildType.Null;

        foreach (Transform child in buildPlaceTransform)
            child.GetComponent<SpriteRenderer>().sprite = null;
    }

    private GameObject GetBuildingPlaceFromDirection(Directions directions)
    {
        var currentPosition = transform.position;
        var multiplier = Utility.DirectionModifiers[(int)directions];

        return _gameBuilder.BuildingPlaces[
            Mathf.RoundToInt(currentPosition.y / 1.28f) + Mathf.RoundToInt((_gameBuilder.Rows - 1f) / 2f) +
            multiplier.y,
            Mathf.RoundToInt(currentPosition.x / 1.28f) + Mathf.RoundToInt((_gameBuilder.Columns - 1f) / 2f) +
            multiplier.x];
    }
}