using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Generates a level based on configuration data.
/// </summary>
public class LvlGenerator : MonoBehaviour
{
    [Header("Prefabs")] [SerializeField] private GameObject
        player,
        block,
        coin,
        halfSlopeBlock,
        emptyBlock,
        endLvl,
        enemy,
        heal,
        ghostBlock,
        slopeBlock,
        spike,
        watter,
        watterBottom;

    [Header("Parents")] [SerializeField] private Transform
        tilesParent,
        watterParent,
        plantsParent;

    [Header("Utilities")]
    [SerializeField] private Transform background;
    [SerializeField] private TutorialManager tutorialManager;

    private int 
        _height,
        _length;

    private void Start()
    {
        SettingsManager.ChangeBackground(background);

        try
        {
            var map = JsonConvert
                .DeserializeObject<LevelConfigurations>(Resources.Load<TextAsset>("LevelData/maps").text)
                .Levels[LvlManager.Instance.CurrentLvl - 1].Map;

            _height = map.Length;
            _length = map[0].Length;

            for (var i = 0; i < _height; i++)
            for (var j = 0; j < _length; j++)
                GenerateObject(map[i][j], i, j);
        }
        catch (IOException ex) 
        {
            Debug.Log("Error reading the file: " + ex.Message);
        }
        catch (JsonException ex)
        {
            Debug.Log("Error deserializing JSON: " + ex.Message);
        }

        StartTutorial();
    }

    private void StartTutorial()
    {
        switch (LvlManager.Instance.CurrentLvl)
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

    private void GenerateObject(char character, int row, int column)    //*** TO DO *** : Update after refactoring Platform
    {
        if (row == _height - 1)
            CreateBottomGround(character, row, column);
        
        Platform platform;

        switch (character)
        {
            case 'P':
                Instantiate(player,
                    new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -5f),
                    Quaternion.identity);
                break;
            case 'C':
                Instantiate(coin, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity);
                break;
            case 'G':
                Instantiate(block, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case 'T':
                Instantiate(block, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent).GetComponent<Platform>().platformType = PlatformType.Temporary;
                break;
            case '^':
            case 'v':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.VerticalMoving;
                platform.movement = character == '^' ? 20 : -20;
                break;
            case 'R':
            case 'L':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.HorizontalMoving;
                platform.movement = character == 'R' ? 20 : -20;
                break;
            case 'o':
            case 'O':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.CircularMoving;
                platform.rotationAngle = 3;
                platform.rotationSpeed = character == 'o' ? 1 : -1;
                break;
            case '{':
                Instantiate(halfSlopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case '}':
                Instantiate(halfSlopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 180, 0), tilesParent);
                break;
            case 'D':
                Instantiate(emptyBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case 'X':
                Instantiate(endLvl,
                    new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -1),
                    Quaternion.identity);
                break;
            case 'E':
                Instantiate(enemy, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -1),
                    Quaternion.identity);
                break;
            case '0':
                Instantiate(ghostBlock, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f, -1),
                    Quaternion.identity);
                break;
            case 'H':
                Instantiate(heal, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -1),
                    Quaternion.identity);
                break;
            case '<':
                Instantiate(slopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 180, 0), tilesParent);
                break;
            case '>':
                Instantiate(slopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case 'S':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f - .5f),
                    Quaternion.identity);
                break;
            case 's':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .5f),
                    Quaternion.Euler(0, 0, 180));
                break;
            case '[':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f + .5f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 0, 90));
                break;
            case ']':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f - .5f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 0, -90));
                break;
            case 'W':
                Instantiate(watter, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, watterParent);
                break;
            case 'M':
                Instantiate(watterBottom, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
        }
    }

    private void CreateBottomGround(char character, int row, int column)
    {
        for (var i = 1; i < 11; i++)
            Instantiate(character == 'W' ? watterBottom : emptyBlock,
                new Vector2((column - _length / 2) * 1.28f, -(row + i - _height / 2) * 1.28f), Quaternion.identity,
                tilesParent);
    }
}