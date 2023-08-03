using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject emptyPlatform;
    [SerializeField] private GameObject watter;
    [SerializeField] private GameObject watterBottom;
    [SerializeField] private Transform player;
    [SerializeField] private Sprite spriteBackground;

    private int _lastChunk = -2;
    private List<float> _lastHeights;
    private int[] _platformsLengths = new int[5];
    private int[] _platformsGaps = new int[5];

    private void Start()
    {
        if (SettingsManager.Instance.darkModeOn)
            FindObjectOfType<Camera>().backgroundColor = new Color(0, 0, 0.5f);
    }
    
    private void Update()
    {
        if (player.position.x < (_lastChunk + 1) * 38.4f) 
            return;
        
        _lastChunk++;
        GenerateNewChunk();
        ClearLastChunk();
    }

    private void GenerateNewChunk()
    {

        _platformsLengths = new []{
            Utils.RandomPickNumberExcludingZero(25), Utils.RandomPickNumberExcludingZero(20), Utils.RandomPickNumberExcludingZero(15), Utils.RandomPickNumberExcludingZero(10), Utils.RandomPickNumberExcludingZero(5)
        };
        
        _platformsGaps =  new []{
            Utils.RandomPickNumberExcludingZero(5), Utils.RandomPickNumberExcludingZero(10), Utils.RandomPickNumberExcludingZero(15), Utils.RandomPickNumberExcludingZero(20), Utils.RandomPickNumberExcludingZero(25)
        };
        
        for (var i = (_lastChunk + 1) * 38.4f + 1.28f; i < (_lastChunk + 2) * 38.4f + 1.28f; i += 1.28f)
        {
            Instantiate(watter, new Vector3(i, -1.28f, 1), Quaternion.identity);
            Instantiate(watterBottom, new Vector3(i, -2.56f, 1), Quaternion.identity);
            Instantiate(watterBottom, new Vector3(i, -3.84f, 1), Quaternion.identity);
            Instantiate(watterBottom, new Vector3(i, -5.12f, 1), Quaternion.identity);
            InstantiatePlatform(i);
        }

        for (var i = (_lastChunk + 1) * 38.4f + 1.28f; i < (_lastChunk + 2) * 38.4f + 1.28f; i += 19)
        {
            var background = new GameObject("Background");
            var spriteRenderer = background.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = spriteBackground;
            spriteRenderer.sortingOrder = -1;

            if (SettingsManager.Instance.darkModeOn)
                spriteRenderer.color = new Color(0, 0, 0.49f);

            background.transform.position = new Vector3(i, 4.5f , 0f);
        }
    }
    
    private void ClearLastChunk()
    {
        var startPoint = new Vector2((_lastChunk - 2) * 38.4f - 10, -10);
        var endPoint = new Vector2((_lastChunk - 1) * 38.4f - 1.28f, 30);
        
        foreach (var objectCollider in Physics2D.OverlapAreaAll(startPoint, endPoint))
            Destroy(objectCollider.gameObject);

        foreach (var obj in FindObjectsOfType<GameObject>().Where(obj => obj.name == "Background" && IsWithinPoints(startPoint, endPoint, obj.transform.position)))
            Destroy(obj);

        var wall = Instantiate(emptyPlatform, new Vector2((_lastChunk - 1) * 38.4f - 20, 8), Quaternion.identity);
        wall.transform.localScale *= 10;
    }

    private void InstantiatePlatform(float xPosition)
    {
        for (var i = 0; i < _platformsLengths.Length; i++)
        {
            if (_platformsGaps[i] == 0)
            {
                if (_platformsLengths[i] > 0)
                {
                    _platformsLengths[i]--;
                    var platformObject = Instantiate(platform, new Vector3(xPosition, (3 * i - 1) * 1.28f, 0), Quaternion.identity);
                    var platformType = i == 0 ? (PlatformType)(Utils.RandomPickNumberBetween(0, 3) % 2) :
                        (PlatformType)(Utils.RandomPickNumberBetween(0,
                            Enum.GetValues(typeof(PlatformType)).Length + 1) % 5);
                    
                    var platformComponent = platformObject.GetComponent<Platform>();

                    platformComponent.platformType = platformType;
                    platformComponent.endlessRun = _platformsLengths[i] != 0;
                    
                    if (platformType != PlatformType.Static || i != 0 ||
                        Utils.RandomPickNumberBetween(0, 2) != 0) continue;

                    Instantiate(emptyPlatform, new Vector3(xPosition, -2.56f, -1), Quaternion.identity);
                    Instantiate(emptyPlatform, new Vector3(xPosition, -3.84f, -1), Quaternion.identity);
                    Instantiate(emptyPlatform, new Vector3(xPosition, -5.12f, -1), Quaternion.identity);
                }
                else
                {
                    _platformsLengths[i] = Utils.RandomPickNumberExcludingZero((Math.Abs((i - _platformsLengths.Length - 1) % 5) + 1) * 5);
                    _platformsGaps[i] = Utils.RandomPickNumberExcludingZero((i + 1) * 5);
                }
            }
            else
                _platformsGaps[i]--;
        }
    }

    private static bool IsWithinPoints(Vector3 startPoint, Vector3 endPoint, Vector3 position) => 
        position.x >= Mathf.Min(startPoint.x, endPoint.x) && position.x <= Mathf.Max(startPoint.x, endPoint.x) && position.y >= Mathf.Min(startPoint.y, endPoint.y) && position.y <= Mathf.Max(startPoint.y, endPoint.y);
}
