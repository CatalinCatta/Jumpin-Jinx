using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a place where players can construct objects or structures.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BuildingPlace : MonoBehaviour
{
    public ObjectBuildType block, environment, objectType;
    [Header("Neighbors")]
    private BuildingPlace _buildingPlaceDown, _buildingPlaceUp, _buildingPlaceLeft, _buildingPlaceRight;
    public (int x, int y) PositionInArray;
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

        if (PositionInArray.x < _gameBuilder.Rows - 1)
            _gameBuilder.BuildingPlaces[PositionInArray.x + 1, PositionInArray.y].TryGetComponent(out _buildingPlaceUp);

        if (PositionInArray.x > 1)
            _gameBuilder.BuildingPlaces[PositionInArray.x - 1, PositionInArray.y].TryGetComponent(out _buildingPlaceDown);

        if (PositionInArray.y < _gameBuilder.Columns - 1)
            _gameBuilder.BuildingPlaces[PositionInArray.x, PositionInArray.y + 1].TryGetComponent(out _buildingPlaceRight);

        if (PositionInArray.y > 1)
            _gameBuilder.BuildingPlaces[PositionInArray.x, PositionInArray.y - 1].TryGetComponent(out _buildingPlaceLeft);
    }

    private void OnMouseExit()
    {
        if (_gameBuilder.SelectedObject == null && !_gameBuilder.DeleteMode) return;

        _spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
        ShowAllItems(_localTransform, true);

        if (_buildingPlaceUp != null) ShowAllItems(_buildingPlaceUp.transform, true);
        if (_buildingPlaceDown != null) ShowAllItems(_buildingPlaceDown.transform, true);
        if (_buildingPlaceLeft != null) ShowAllItems(_buildingPlaceLeft.transform, true);
        if (_buildingPlaceRight != null) ShowAllItems(_buildingPlaceRight.transform, true);

        _localTransform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        var isMousePressed = Input.GetMouseButton(0);

        if (_gameBuilder.DeleteMode) Delete(isMousePressed);
        else if(_gameBuilder.SelectedObject != null ) Build(isMousePressed);
    }

    private void Delete(bool isDestroying)
    {
        if (block != ObjectBuildType.Null) EmptyAbove(!isDestroying);
        ClearSpikes(!isDestroying);
        EmptyCurrent(!isDestroying);
        _spriteRenderer.color = Color.red;
    }

    private void Build(bool isConstructing)
    {
        var temporaryItemTransform = _localTransform.GetChild(1);
        var selectedObject = _gameBuilder.SelectedObject;
        var selectedObjTransform = selectedObject.transform.GetChild(0);
        var selectedObjectSprite = selectedObjTransform.GetComponent<Image>().sprite;
        var selectedObjectRotation = selectedObjTransform.rotation;
        var currentCategory = Dictionaries.ObjectBuild[selectedObject.objectBuildType].category;
        
        _spriteRenderer.color = isConstructing? Color.white : Color.green;
        temporaryItemTransform.gameObject.SetActive(!isConstructing);
        ChangeSprite(temporaryItemTransform, selectedObjectSprite, selectedObjectRotation);

        if (currentCategory == ObjectBuildCategory.Block)
        {
            EmptyCurrent(!isConstructing);
            if (isConstructing)
                ChangeObject(this, selectedObjectSprite, BuildType.Block, selectedObject.objectBuildType,
                    selectedObjectRotation);
            return;
        }

        if (selectedObject.objectBuildType switch
            {
                ObjectBuildType.SpikeUpsideDown => _buildingPlaceUp == null || !IsSolidBlock(_buildingPlaceUp),
                ObjectBuildType.SpikeRight => _buildingPlaceLeft == null || !IsSolidBlock(_buildingPlaceLeft),
                ObjectBuildType.SpikeLeft => _buildingPlaceRight == null || !IsSolidBlock(_buildingPlaceRight),
                _ => _buildingPlaceDown == null || !IsSolidBlock(_buildingPlaceDown)
            })
        {
            _spriteRenderer.color = Color.red;
            return;
        }
        
        EmptyAbove(!isConstructing);
        ClearSpikes(!isConstructing);
        
        if (currentCategory == ObjectBuildCategory.Plant)
        {
            EmptyCurrent(!isConstructing, BuildType.Object);
            if (isConstructing)
                ChangeObject(this, selectedObjectSprite, BuildType.Environment, selectedObject.objectBuildType,
                    selectedObjectRotation);
            return;
        }

        EmptyCurrent(!isConstructing, BuildType.Environment);
        
        if (isConstructing)
        {
            ChangeObject(this, selectedObjectSprite, BuildType.Object, selectedObject.objectBuildType,
                selectedObjectRotation);

            var (x, y) = (-1, -1);
            
            switch (selectedObject.objectBuildType)
            {
                case ObjectBuildType.Player:
                    _gameBuilder.playerCounter.text = "0";
                    if (_gameBuilder.PlayerPosition != PositionInArray) (x, y) = _gameBuilder.PlayerPosition;
                    _gameBuilder.PlayerPosition = PositionInArray;
                    break;
                
                case ObjectBuildType.EndLvl:
                    _gameBuilder.endLvlCounter.text = "0";
                    if (_gameBuilder.EndLvlPosition != PositionInArray) (x, y) = _gameBuilder.EndLvlPosition;
                    _gameBuilder.EndLvlPosition = PositionInArray;
                    break;
            }

            if (x != -1)
                ChangeObject(_gameBuilder.BuildingPlaces[x, y].GetComponent<BuildingPlace>(), null, BuildType.Object);
        }

        bool IsSolidBlock(BuildingPlace buildingPlace) =>buildingPlace.block is not ObjectBuildType.Null
            and not ObjectBuildType.SlopeDirt and not ObjectBuildType.SlopeDirtRotated
            and not ObjectBuildType.HalfSlopeDirt and not ObjectBuildType.HalfSlopeDirtRotated;

    }

    private void ClearSpikes(bool temporary)
    {
        var isSpikeUnder = _buildingPlaceDown != null && _buildingPlaceDown.objectType == ObjectBuildType.SpikeUpsideDown;
        var isSpikeOnLeft = _buildingPlaceLeft != null && _buildingPlaceLeft.objectType == ObjectBuildType.SpikeLeft;
        var isSpikeOnRight = _buildingPlaceRight != null && _buildingPlaceRight.objectType == ObjectBuildType.SpikeRight;

        Debug.Log((isSpikeUnder, isSpikeOnLeft, isSpikeOnRight));
        if (temporary)
        {
            if (isSpikeUnder) ShowOneItem(_buildingPlaceDown, false, BuildType.Object);
            if (isSpikeOnLeft) ShowOneItem(_buildingPlaceLeft, false, BuildType.Object);
            if (isSpikeOnRight) ShowOneItem(_buildingPlaceRight, false, BuildType.Object);

            if (isSpikeUnder || isSpikeOnLeft || isSpikeOnRight) _spriteRenderer.color = Color.yellow;
        }
        else
        {
            if (isSpikeUnder) ChangeObject(_buildingPlaceDown, null, BuildType.Object);
            if (isSpikeOnLeft) ChangeObject(_buildingPlaceLeft, null, BuildType.Object);
            if (isSpikeOnRight) ChangeObject(_buildingPlaceRight, null, BuildType.Object);
        }
    }
    
    private void EmptyAbove(bool temporary)
    {
        if (_buildingPlaceUp == null) return;
        
        if (temporary)
        {
            if (IsObjectUsed())
            {
                ShowOneItem(_buildingPlaceUp, false, BuildType.Object);
                _spriteRenderer.color = Color.yellow;
            }
            if (!IsEnvUsed()) return;
            
            ShowOneItem(_buildingPlaceUp, false, BuildType.Environment);
            _spriteRenderer.color = Color.yellow;
        }
        else
        {
            if (IsObjectUsed()) ChangeObject(_buildingPlaceUp, null, BuildType.Object);
            if (IsEnvUsed()) ChangeObject(_buildingPlaceUp, null, BuildType.Environment);
        }

        bool IsObjectUsed() => _buildingPlaceUp.objectType is not ObjectBuildType.Null and not ObjectBuildType.SpikeLeft
            and not ObjectBuildType.SpikeRight and not ObjectBuildType.SpikeUpsideDown;

        bool IsEnvUsed() => _buildingPlaceUp.environment != ObjectBuildType.Null;
    }
    
    private void EmptyCurrent(bool temporary, BuildType skipBuildType = BuildType.Null)
    {
        if (!IsNeeded()) return;

        if (temporary)
        {
            ShowAllItems(_localTransform, false, skipBuildType);
            _spriteRenderer.color = Color.yellow;
        }
        else
        {
            if (skipBuildType != BuildType.Block) ChangeObject(this, null, BuildType.Block);
            if (skipBuildType != BuildType.Environment) ChangeObject(this, null, BuildType.Environment);
            if (skipBuildType != BuildType.Object)
            {
                switch (objectType)
                {
                    case ObjectBuildType.Player:
                        _gameBuilder.playerCounter.text = "1";
                        _gameBuilder.PlayerPosition = (-1, -1);
                        break;
                  
                    case ObjectBuildType.EndLvl:
                        _gameBuilder.endLvlCounter.text = "1";
                        _gameBuilder.EndLvlPosition = (-1, -1);
                        break;
                }
                ChangeObject(this, null, BuildType.Object);
            }
        }
        
        bool IsNeeded() => (block != ObjectBuildType.Null && skipBuildType != BuildType.Block) ||
                           (environment != ObjectBuildType.Null && skipBuildType != BuildType.Environment) ||
                           (objectType != ObjectBuildType.Null && skipBuildType != BuildType.Object);
    }
    
    private static void ShowAllItems(Component buildPlaceTransform, bool show, BuildType skipBuildType = BuildType.Null)
    {
        for (var i = 1; i < Enum.GetValues(typeof(BuildType)).Length; i++)
            if ((BuildType)i != skipBuildType)
                ShowOneItem(buildPlaceTransform, show, (BuildType)i);
    }

    private static void ShowOneItem(Component grandParent, bool show, BuildType itemToShow) =>
        grandParent.transform.GetChild(0).GetChild((int)itemToShow - 1).gameObject.SetActive(show);

    private static void ChangeObject(BuildingPlace grandParent, [CanBeNull] Sprite sprite, BuildType objectCategory,
        ObjectBuildType objectType= ObjectBuildType.Null, Quaternion? rotation = null)
    {
        rotation ??= Quaternion.identity;
        
        switch (objectCategory)
        {
            case BuildType.Block:
                grandParent.block = objectType;
                break;
            
            case BuildType.Environment:
                grandParent.environment = objectType;
                break;
            
            case BuildType.Object:
                grandParent.objectType = objectType;
                break;
        }

        var child = grandParent.transform.GetChild(0).GetChild((int)objectCategory - 1);
        ChangeSprite(child, sprite, (Quaternion)rotation);
    }

    private static void ChangeSprite(Transform spriteTransform, Sprite newSprite, Quaternion newRotation)
    {
        spriteTransform.GetComponent<SpriteRenderer>().sprite = newSprite;
        spriteTransform.rotation = newRotation;
    }
}