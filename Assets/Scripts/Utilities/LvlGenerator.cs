using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Generates a level based on configuration data.
/// </summary>
public class LvlGenerator : MonoBehaviour
{
    [Header("Parents")] [SerializeField] private Transform
        tilesParent,
        watterParent,
        plantsParent;

    [Header("Utilities")]
    [SerializeField] private Transform background;
    [SerializeField] private TutorialManager tutorialManager;

    private PrefabManager _prefabManager;
    private LvlManager _lvlManager;
    
    private int 
        _height,
        _length,
        _coinsCounter;

    private void Start()
    {
        _prefabManager = PrefabManager.Instance;
        _lvlManager = LvlManager.Instance;
        
        SettingsManager.ChangeBackground(background);

        try
        {
            var lvl =
                JsonConvert.DeserializeObject<Level[]>(Resources.Load<TextAsset>("LevelData/maps").text)[
                    _lvlManager.CurrentLvl - 1];
            var map = lvl.Maps;
            
            _height = map.Length;
            _length = map[0].Length;

            for (var i = 0; i < _height; i++)
            for (var j = 0; j < _length; j++)
                GenerateObject(map[i][j], i, j);

            _lvlManager.CoinsInLevel = _coinsCounter;
            _lvlManager.TimerLimitForStars = (lvl.TimerLimitForStars[0], lvl.TimerLimitForStars[1],
                lvl.TimerLimitForStars[2]);
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

    private void GenerateObject(char character, int row, int column)
    {
        if (row == _height - 1) CreateBottomGround(character, row, column);
        
        switch (character)
        {
            case 'P':
                Instantiate(_prefabManager.player,
                    new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -5f),
                    Quaternion.identity);
                break;
            case 'C':
                Instantiate(_prefabManager.coin,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity);
                _coinsCounter++;
                break;
            case 'G':
                Instantiate(_prefabManager.grass,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case 'T':
                Instantiate(_prefabManager.temporaryPlatform,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case '^':
            case 'v':
            case 'R':
            case 'L':
                var vertical = character is '^' or 'v';
                var sidewaysMovingPlatform = Instantiate(
                    vertical ? _prefabManager.verticalMovingPlatform : _prefabManager.horizontalMovingPlatform,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<SidewaysMoving>();
                sidewaysMovingPlatform.movement = character is '^' or 'R' ? 20 : -20;
                sidewaysMovingPlatform.direction = vertical ? SidewaysMoving.Direction.Vertical : SidewaysMoving.Direction.Horizontal;
                break;
            case 'o':
            case 'O':
                var circularMovingPlatform = Instantiate(_prefabManager.circularMovingPlatform,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<CircularMovingPlatform>();
                circularMovingPlatform.rotationAngle = 3f;
                circularMovingPlatform.rotationSpeed = character == 'o' ? 1f : -1f;
                break;
            case '{':
                Instantiate(_prefabManager.halfSlopeGrass,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case '}':
                Instantiate(_prefabManager.halfSlopeGrass,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 180, 0), tilesParent);
                break;
            case 'D':
                Instantiate(_prefabManager.dirt,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case 'X':
                Instantiate(_prefabManager.endLvl,
                    new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -1),
                    Quaternion.identity);
                break;
            case 'E':
                Instantiate(_prefabManager.spider,
                    new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -1),
                    Quaternion.identity);
                break;
            case '0':
                Instantiate(_prefabManager.ghostBlock,
                    new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f, -1),
                    Quaternion.identity);
                break;
            case 'H':
                Instantiate(_prefabManager.heal,
                    new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .4f, -1),
                    Quaternion.identity);
                break;
            case '<':
                Instantiate(_prefabManager.slopeGrass,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 180, 0), tilesParent);
                break;
            case '>':
                Instantiate(_prefabManager.slopeGrass,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
            case 'S':
                Instantiate(_prefabManager.spike,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f - .5f),
                    Quaternion.identity);
                break;
            case 's':
                Instantiate(_prefabManager.spike,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + .5f),
                    Quaternion.Euler(0, 0, 180));
                break;
            case '[':
                Instantiate(_prefabManager.spike,
                    new Vector2((column - _length / 2) * 1.28f + .5f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 0, 90));
                break;
            case ']':
                Instantiate(_prefabManager.spike,
                    new Vector2((column - _length / 2) * 1.28f - .5f, -(row - _height / 2) * 1.28f),
                    Quaternion.Euler(0, 0, -90));
                break;
            case 'W':
                Instantiate(_prefabManager.watter,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, watterParent);
                break;
            case 'M':
                Instantiate(_prefabManager.watterBottom,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f),
                    Quaternion.identity, tilesParent);
                break;
        }
    }

    private void CreateBottomGround(char character, int row, int column)
    {
        for (var i = 1; i < 11; i++)
            Instantiate(character == 'W' ? _prefabManager.watterBottom : _prefabManager.dirt,
                new Vector2((column - _length / 2) * 1.28f, -(row + i - _height / 2) * 1.28f), Quaternion.identity,
                tilesParent);
    }
}