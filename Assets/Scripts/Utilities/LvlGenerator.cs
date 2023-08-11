using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class LvlGenerator : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject halfSlopeBlock;
    [SerializeField] private GameObject emptyBlock;
    [SerializeField] private GameObject endLvl;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject heal;
    [SerializeField] private GameObject ghostBlock;
    [SerializeField] private GameObject slopeBlock;
    [SerializeField] private GameObject spike;
    [SerializeField] private GameObject watter;
    [SerializeField] private GameObject watterBottom;

    [SerializeField] private Transform tilesParent;
    [SerializeField] private Transform watterParent;
    [SerializeField] private Transform plantsParent;

    [SerializeField] private Transform background;
    
    [SerializeField] private TutorialManager tutorialManager;
    
    private int _height;
    private int _length;
    
    private void Start()
    {
        SettingsManager.ChangeBackground(background);
        
        try
        {
            var map = JsonConvert
                .DeserializeObject<LevelConfigurations>(Resources.Load<TextAsset>("LevelData/maps").text)
                .Levels[LvlManager.Instance.currentLvl - 1].Map;

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
        switch (LvlManager.Instance.currentLvl)
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
                FindObjectOfType<InGameMenu>().transform.GetChild(0).GetChild(0).GetChild(4).GetChild(2).gameObject.SetActive(false);
                break;
        }
    }
    
    private void GenerateObject(char character, int row, int column)
    {
        if (row == _height - 1)
            for (var i = 1; i < 11; i++)
                Instantiate(character == 'W'? watterBottom : emptyBlock, new Vector2((column - _length / 2) * 1.28f, -(row + i - _height / 2) * 1.28f), Quaternion.identity, tilesParent);
        Platform platform;
        
        switch (character)
        {
            case 'P':
                Instantiate(player, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + 0.4f, -5f), Quaternion.identity);
                break;
            case 'C':
                Instantiate(coin, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity);
                break;
            case 'G':
                Instantiate(block, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, tilesParent);
                break;
            case 'T':
                Instantiate(block, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, tilesParent).GetComponent<Platform>().platformType = PlatformType.Temporary;
                break;
            case 'V':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.VerticalMoving;
                platform.movement = 20;
                break;
            case 'U':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.VerticalMoving;
                platform.movement = 20;
                Instantiate(watter, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, watterParent);
                break;
            case '-':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.HorizontalMoving;
                platform.movement = 20;
                break;
            case '_':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.HorizontalMoving;
                platform.movement = 20;
                Instantiate(watter, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, watterParent);
                break;
            case 'o':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.CircularMoving;
                platform.rotationAngle = 3;
                platform.rotationSpeed = 1;
                break;
            case 'O':
                platform = Instantiate(block,
                    new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity,
                    tilesParent).GetComponent<Platform>();
                platform.platformType = PlatformType.CircularMoving;
                platform.rotationAngle = 3;
                platform.rotationSpeed = -1;
                break;
            case '{':
                Instantiate(halfSlopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, tilesParent);
                break;
            case '}':
                Instantiate(halfSlopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.Euler(0, 180, 0), tilesParent);
                break;
            case 'D':
                Instantiate(emptyBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, tilesParent);
                break;
            case 'X':
                Instantiate(endLvl, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + 0.4f, -1), Quaternion.identity);
                break;
            case 'E':
                Instantiate(enemy, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + 0.4f, -1), Quaternion.identity);
                break;
            case '0':
                Instantiate(ghostBlock, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f, -1), Quaternion.identity);
                break;
            case 'H':
                Instantiate(heal, new Vector3((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + 0.4f, -1), Quaternion.identity);
                break;
            case '<':
                Instantiate(slopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.Euler(0, 180, 0), tilesParent);
                break;
            case '>':
                Instantiate(slopeBlock, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, tilesParent);
                break;
            case 'S':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f - 0.5f), Quaternion.identity);
                break;
            case 's':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f + 0.5f), Quaternion.Euler(0, 0, 180));
                break;
            case '[':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f + 0.5f, -(row - _height / 2) * 1.28f), Quaternion.Euler(0, 0, 90));
                break;
            case ']':
                Instantiate(spike, new Vector2((column - _length / 2) * 1.28f - 0.5f, -(row - _height / 2) * 1.28f), Quaternion.Euler(0, 0, -90));
                break;
            case 'W':
                Instantiate(watter, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, watterParent);
                break;
            case 'M':
                Instantiate(watterBottom, new Vector2((column - _length / 2) * 1.28f, -(row - _height / 2) * 1.28f), Quaternion.identity, tilesParent);
                break;
        }
    }
    
    private class LevelConfigurations
    {
        public Level[] Levels { get; set; }
    }

    private class Level
    {
        public string[] Map { get; set; }
    }
}
