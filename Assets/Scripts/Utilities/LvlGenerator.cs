﻿using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using GameObject = UnityEngine.GameObject;

/// <summary>
/// Generates a level based on configuration data.
/// </summary>
public class LvlGenerator : MonoBehaviour
{
    [Header("Parents")] [SerializeField] private Transform tilesParent, plantsParent, objectsParent;
    
    [Header("Utilities")]
    [SerializeField] private Transform background;
    [SerializeField] private TutorialManager tutorialManager;

    private PrefabManager _prefabManager;
    private LvlManager _lvlManager;
    private int _height, _length, _coinsCounter;
    private GameObject[,] _platforms;
    
    private void Start()
    {
        _prefabManager = PrefabManager.Instance;
        _lvlManager = LvlManager.Instance;
        
        SettingsManager.ChangeBackground(background);

        try
        {
            var lvl = _lvlManager.IsCampaign
                ? JsonConvert.DeserializeObject<Level[]>(Resources.Load<TextAsset>(Path.Join("LevelData", "maps"))
                    .text)[_lvlManager.CurrentLvl - 1]
                : JsonConvert.DeserializeObject<Level>(File.ReadAllText(Path.Join(Path.GetFullPath(@"CustomLevels"),
                    _lvlManager.LvlTitle, _lvlManager.LvlTitle + ".json")));
            
            var map = lvl.Maps;

            _height = map.Length;
            _length = map[0].Length;
            _platforms = new GameObject[_height, _length];

            for (var x = 0; x < _length; x++)
            {
                Instantiate(_prefabManager.acid, new Vector3(x - _length / 2, -_height / 2, 2) * 1.28f,
                    Quaternion.identity, tilesParent);
                Instantiate(_prefabManager.ghostBlock, new Vector3(x - _length / 2, -_height / 2 - 1, 2) * 1.28f,
                    Quaternion.identity, tilesParent);
                Instantiate(_prefabManager.ghostBlock, new Vector3(x - _length / 2, _height / 2, 2) * 1.28f,
                    Quaternion.identity, tilesParent);
            }
            
            for (var i = 0; i < _height; i++)
            for (var j = 0; j < _length; j++)
                GenerateObject(map[i][j], i, j, true);

            for (var i = 0; i < _height; i++)
            for (var j = 0; j < _length; j++)
                GenerateObject(map[i][j], i, j, false);

            _lvlManager.CoinsInLevel = _coinsCounter;
            _lvlManager.TimerLimitForStars = (lvl.TimerLimitForStars[2], lvl.TimerLimitForStars[1],
                lvl.TimerLimitForStars[0]);
        }
        catch (IOException ex) 
        {
            Debug.Log("Error reading the file: " + ex.Message);
        }
        catch (JsonException ex)
        {
            Debug.Log("Error deserializing JSON: " + ex.Message);
        }

        if (_lvlManager.IsCampaign) StartTutorial();
    }

    private void StartTutorial()
    {
        switch (_lvlManager.CurrentLvl)
        {
            case 1:
                tutorialManager.SetUpMovement();
                break;
            case 4:
                tutorialManager.SetUpWatter();
                break;
            case 7:
                tutorialManager.SetUpSpike();
                break;
            case 10:
                tutorialManager.SetUpTemporaryPlatform();
                break;
            case 13:
                tutorialManager.SetUpEnemy();
                break;
            case 16:
                tutorialManager.SetUpPlatform();
                break;
            case 18:
                FindObjectOfType<InGameMenu>().transform.GetChild(0).GetChild(0).GetChild(4).GetChild(2).gameObject
                    .SetActive(false);  // Hide Next Level Button
                break;
        }
    }

    private void GenerateObject(char character, int row, int column, bool isPlatform)
    {
        var objectDetails = Dictionaries.ObjectBuild.FirstOrDefault(kv => kv.Value.character == character);
        if (Dictionaries.ObjectBuild.All(kv => kv.Value.character != character) ||
            (isPlatform && objectDetails.Value.category != ObjectBuildCategory.Block) ||
            (!isPlatform && objectDetails.Value.category == ObjectBuildCategory.Block)) return;

        if (row == _height - 1 && objectDetails.Value.category == ObjectBuildCategory.Block &&
            objectDetails.Key is not ObjectBuildType.Acid and not ObjectBuildType.AcidBottom)
            CreateBottomGround(column);

        if (objectDetails.Key == ObjectBuildType.Coin) _coinsCounter++;

        var objectCreated = Instantiate(objectDetails.Value.prefab, objectDetails.Key == ObjectBuildType.Player
                ? new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -5f)
                : objectDetails.Key != ObjectBuildType.Coin &&
                  objectDetails.Value.category is ObjectBuildCategory.Enemy or ObjectBuildCategory.Object
                    ? new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -1f)
                    : objectDetails.Key switch
                    {
                        ObjectBuildType.Spike => new Vector2((column - _length / 2) * 1.28f,
                            -(row - _height / 2) * 1.28f - .5f),
                        ObjectBuildType.SpikeUpsideDown => new Vector2((column - _length / 2) * 1.28f,
                            -(row - _height / 2) * 1.28f + .5f),
                        ObjectBuildType.SpikeLeft => new Vector2((column - _length / 2) * 1.28f + .5f,
                            -(row - _height / 2) * 1.28f),
                        ObjectBuildType.SpikeRight => new Vector2((column - _length / 2) * 1.28f - .5f,
                            -(row - _height / 2) * 1.28f),
                        _ => new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f)
                    }, objectDetails.Value.rotation,
            isPlatform ? tilesParent :
            _platforms[row + 1, column] != null ? _platforms[row + 1, column].transform : new GameObject().transform);

        if (objectDetails.Key == ObjectBuildType.Spider) objectCreated.transform.position -= new Vector3(0, 1.3f, 0);
        
        if (isPlatform) _platforms[row, column] = objectCreated;
        
        if (objectCreated.TryGetComponent<SidewaysMoving>(out var sidewaysMoving))
        {
            switch (objectDetails.Key)
            {
                case ObjectBuildType.UpMovingPlatform:
                    sidewaysMoving.movement = 20;
                    sidewaysMoving.direction = SidewaysMoving.Direction.Vertical;
                    break;
                case ObjectBuildType.DownMovingPlatform:
                    sidewaysMoving.movement = -20;
                    sidewaysMoving.direction = SidewaysMoving.Direction.Vertical;
                    break;
                case ObjectBuildType.RightMovingPlatform:
                    sidewaysMoving.movement = 20;
                    sidewaysMoving.direction = SidewaysMoving.Direction.Horizontal;
                    break;
                case ObjectBuildType.LeftMovingPlatform:
                    sidewaysMoving.movement = -20;
                    sidewaysMoving.direction = SidewaysMoving.Direction.Horizontal;
                    break;
            }
        }
        else if (objectCreated.TryGetComponent<CircularMovingPlatform>(out var circularMoving))
        {
            switch (objectDetails.Key)
            {
                case ObjectBuildType.CircularMovingPlatform:
                    circularMoving.rotationAngle = 3f;
                    circularMoving.rotationSpeed = 1f;
                    break;
                case ObjectBuildType.CounterCircularMovingPlatform:
                    circularMoving.rotationAngle = 3f;
                    circularMoving.rotationSpeed = -1f;
                    break;
            }
        }
    }

    private void CreateBottomGround(int column)
    {
        for (var i = 1; i < 11; i++)
            Instantiate(_prefabManager.dirt,
                new Vector2((column - _length / 2) * 1.28f, -(_height / 2 + i) * 1.28f), Quaternion.identity,
                tilesParent);
    }
}