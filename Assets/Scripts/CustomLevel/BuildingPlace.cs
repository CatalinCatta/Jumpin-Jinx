using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a place where players can construct objects or structures.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BuildingPlace : MonoBehaviour
{
    [NonSerialized] public ObjectBuildType Block, Environment, Object;

    [Header("Neighbors")]
    private BuildingPlace _buildingPlaceDown, _buildingPlaceUp, _buildingPlaceLeft, _buildingPlaceRight;

    private GameBuilder _gameBuilder;
    private Transform _localTransform;
    private SpriteRenderer _spriteRenderer;

    private enum BuildType
    {
        Null,
        Block,
        Environment,
        Object
    }
    
    
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
        var buildingPlaceLeftTransform =
            Mathf.RoundToInt(position.x / 1.28f) == -Mathf.RoundToInt((_gameBuilder.Columns - 1f) / 2f)
                ? null
                : GetBuildingPlaceFromDirection(Directions.Left);
        var buildingPlaceRightTransform =
            Mathf.RoundToInt(position.x / 1.28f) == Mathf.RoundToInt((_gameBuilder.Columns - 1f) / 2f)
                ? null
                : GetBuildingPlaceFromDirection(Directions.Right);

        if (buildingPlaceUpTransform != null) buildingPlaceUpTransform.TryGetComponent(out _buildingPlaceUp);
        if (buildingPlaceDownTransform != null) buildingPlaceDownTransform.TryGetComponent(out _buildingPlaceDown);
        if (buildingPlaceLeftTransform != null) buildingPlaceLeftTransform.TryGetComponent(out _buildingPlaceLeft);
        if (buildingPlaceRightTransform != null) buildingPlaceRightTransform.TryGetComponent(out _buildingPlaceRight);
    }

    private void OnMouseExit()
    {
        if (_gameBuilder.SelectedObject == null && !_gameBuilder.DeleteMode) return;

        _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
        ShowItems(_localTransform, true);

        if (_buildingPlaceUp != null) ShowItems(_buildingPlaceUp.transform, true);
        if (_buildingPlaceDown != null) ShowItems(_buildingPlaceDown.transform, true);

        _localTransform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (_gameBuilder.SelectedObject == null && !_gameBuilder.DeleteMode) return;

        var isMousePressed = Input.GetMouseButton(0);

        if (_gameBuilder.DeleteMode) Delete(isMousePressed);
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
        EmptyCurrent(!isDestroying);
        if (Block != ObjectBuildType.Null) EmptyAbove(!isDestroying);
    }

    private void Build(bool isConstructing)
    {
        var finalItemTransform = _localTransform.GetChild(0);
        var temporaryItemTransform = _localTransform.GetChild(1);
        var selectedObject = _gameBuilder.SelectedObject;
        var selectedObjTransform = selectedObject.transform.GetChild(0);
        var selectedObjectSprite = selectedObjTransform.GetComponent<Image>().sprite;
        var selectedObjectRotation = selectedObjTransform.rotation;
        var currentCategory = Dictionaries.ObjectBuild[selectedObject.objectBuildType].category;
        
        _spriteRenderer.color = isConstructing? Color.white : Color.green;
        temporaryItemTransform.gameObject.SetActive(!isConstructing);
        temporaryItemTransform.GetComponent<SpriteRenderer>().sprite = selectedObjectSprite;
        temporaryItemTransform.rotation = selectedObjectRotation;

        if (currentCategory == ObjectBuildCategory.Block)
        {
            EmptyCurrent(!isConstructing);
            if (!isConstructing) return;
            Block = selectedObject.objectBuildType;
            finalItemTransform.GetChild(0).GetComponent<SpriteRenderer>().sprite = selectedObjectSprite;
            finalItemTransform.GetChild(0).rotation = selectedObjectRotation;
            return;
        }

        if (!CanBuildHere())
        {
            _spriteRenderer.color = new Color(1f, 0f, 0f, .5f);
            return;
        }
        
        if (currentCategory == ObjectBuildCategory.Plant)
        {
            EmptyCurrent(!isConstructing, BuildType.Object);
            EmptyAbove(!isConstructing, BuildType.Block);
            if (!isConstructing) return;
            Environment = selectedObject.objectBuildType;
            finalItemTransform.GetChild(1).GetComponent<SpriteRenderer>().sprite = selectedObjectSprite;
            finalItemTransform.GetChild(1).rotation = selectedObjectRotation;
            return;
        }

        EmptyCurrent(!isConstructing, BuildType.Environment);
        if (selectedObject.objectBuildType is not ObjectBuildType.SpikeUpsideDown and not ObjectBuildType.SpikeLeft
            and not ObjectBuildType.SpikeRight) EmptyAbove(!isConstructing, BuildType.Block);
        
        if (isConstructing)
        {
            finalItemTransform.GetChild(2).GetComponent<SpriteRenderer>().sprite = selectedObjectSprite;
            finalItemTransform.GetChild(2).rotation = selectedObjectRotation;
            Object = selectedObject.objectBuildType;

            GameObject specialObject = null;
            var currentPosition = _localTransform.position;
            var arrayPosition = (
                Mathf.RoundToInt(currentPosition.y / 1.28f) + Mathf.RoundToInt((_gameBuilder.Rows - 1f) / 2f),
                Mathf.RoundToInt(currentPosition.x / 1.28f) + Mathf.RoundToInt((_gameBuilder.Columns - 1f) / 2f));
                
            switch (selectedObject.objectBuildType)
            {
                case ObjectBuildType.Player:
                    _gameBuilder.playerCounter.text = "0";
                    if (_gameBuilder.PlayerPosition != (-1, -1) && _gameBuilder.PlayerPosition != arrayPosition)
                        specialObject = _gameBuilder.BuildingPlaces[_gameBuilder.PlayerPosition.x,
                            _gameBuilder.PlayerPosition.y];
                    _gameBuilder.PlayerPosition = arrayPosition;
                    break;
                
                case ObjectBuildType.EndLvl:
                    _gameBuilder.endLvlCounter.text = "0";
                    if (_gameBuilder.EndLvlPosition != (-1, -1) && _gameBuilder.EndLvlPosition != arrayPosition)
                        specialObject = _gameBuilder.BuildingPlaces[_gameBuilder.EndLvlPosition.x,
                            _gameBuilder.EndLvlPosition.y];
                    _gameBuilder.EndLvlPosition = arrayPosition;
                    break;
            }

            if (specialObject != null)
            {
                specialObject.transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
                specialObject.GetComponent<BuildingPlace>().Object = ObjectBuildType.Null;
            }
        }

        bool IsOnSolidBlock() => _buildingPlaceDown != null && _buildingPlaceDown.Block is not ObjectBuildType.Null
            and not ObjectBuildType.SlopeDirt and not ObjectBuildType.SlopeDirtRotated
            and not ObjectBuildType.HalfSlopeDirt and not ObjectBuildType.HalfSlopeDirtRotated;

        bool CanBuildHere()
        {
            if (currentCategory != ObjectBuildCategory.Damage) return IsOnSolidBlock();

            return selectedObject.objectBuildType switch
            {
                ObjectBuildType.Spike when !IsOnSolidBlock() => false,
                ObjectBuildType.SpikeUpsideDown when _buildingPlaceUp == null ||
                                                     _buildingPlaceUp.Block == ObjectBuildType.Null => false,
                ObjectBuildType.SpikeLeft when _buildingPlaceLeft == null ||
                                               _buildingPlaceLeft.Block == ObjectBuildType.Null => false,
                ObjectBuildType.SpikeRight when _buildingPlaceRight == null ||
                                                _buildingPlaceRight.Block == ObjectBuildType.Null => false,
                _ => true
            };
        }
    }
    
    private void EmptyAbove(bool temporary, BuildType skipBuildType = BuildType.Null)
    {
        if (_buildingPlaceUp == null) return;
        if (temporary)
        {
            ShowItems(_buildingPlaceUp.transform, false, skipBuildType);
            if ((_buildingPlaceUp.Block != ObjectBuildType.Null && skipBuildType != BuildType.Block) ||
                (_buildingPlaceUp.Environment != ObjectBuildType.Null && skipBuildType != BuildType.Environment) ||
                (_buildingPlaceUp.Object != ObjectBuildType.Null && skipBuildType != BuildType.Object))
                _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
        }
        else
        {
            ShowItems(_buildingPlaceUp.transform, true);
            ClearItems(_buildingPlaceUp.transform, _buildingPlaceUp, skipBuildType);
        }
    }
    
    private void EmptyCurrent(bool temporary, BuildType skipBuildType = BuildType.Null)
    {
        if (temporary)
        {
            ShowItems(_localTransform, false, skipBuildType);
            if ((Block != ObjectBuildType.Null && skipBuildType != BuildType.Block) ||
                (Environment != ObjectBuildType.Null && skipBuildType != BuildType.Environment) ||
                (Object != ObjectBuildType.Null && skipBuildType != BuildType.Object))
                _spriteRenderer.color = new Color(1f, .5f, 0f, .5f);
        }
        else
        {
            ShowItems(_localTransform, true);
            ClearItems(_localTransform, this, skipBuildType);
        }
    }
    
    private static void ShowItems(Transform buildPlaceTransform, bool show, BuildType skipBuildType = BuildType.Null)
    {
        var keepItem = buildPlaceTransform.GetChild(0);

        if (skipBuildType != BuildType.Block) keepItem.GetChild(0).gameObject.SetActive(show);
        if (skipBuildType != BuildType.Environment) keepItem.GetChild(1).gameObject.SetActive(show);
        if (skipBuildType != BuildType.Object) keepItem.GetChild(2).gameObject.SetActive(show);
    }

    private void ClearItems(Transform buildPlaceTransform, BuildingPlace buildingPlace, BuildType skipBuildType = BuildType.Null)
    {
        var keepItem = buildPlaceTransform.GetChild(0);
        
        if (skipBuildType != BuildType.Block)
        {
            buildingPlace.Block = ObjectBuildType.Null;
            keepItem.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
        if (skipBuildType != BuildType.Environment)
        {
            buildingPlace.Environment = ObjectBuildType.Null;
            keepItem.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
        }
        if (skipBuildType != BuildType.Object)
        {
            if (buildingPlace.Object == ObjectBuildType.Player)
            {
                _gameBuilder.playerCounter.text = "1";
                _gameBuilder.PlayerPosition = (-1, -1);
            }
            else if (buildingPlace.Object == ObjectBuildType.EndLvl)
            {
                _gameBuilder.endLvlCounter.text = "1";
                _gameBuilder.EndLvlPosition = (-1, -1);
            }
            buildingPlace.Object = ObjectBuildType.Null;
            keepItem.GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
        }
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