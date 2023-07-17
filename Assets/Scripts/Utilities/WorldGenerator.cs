using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject emptyPlatform;
    [SerializeField] private GameObject watter;
    [SerializeField] private Transform player;
    [SerializeField] private List<Sprite> backgrounds;

    private int _lastChunk = -2;
    private List<float> _lastHeights;
    private int[] _platformsLengths = new int[5];
    private int[] _platformsGaps = new int[5];
    
    private void Update()
    {
        if (player.position.x < (_lastChunk + 1) * 38.25) 
            return;
        
        _lastChunk++;
        GenerateNewChunk();
        ClearLastChunk();
    }

    private void GenerateNewChunk()
    {

        _platformsLengths = new []{
            Utils.RandomPickNumberExcludingZero(10), Utils.RandomPickNumberExcludingZero(7), Utils.RandomPickNumberExcludingZero(4), Utils.RandomPickNumberExcludingZero(4), Utils.RandomPickNumberExcludingZero(2)
        };
        
        _platformsGaps =  new []{
            Utils.RandomPickNumberExcludingZero(3), Utils.RandomPickNumberExcludingZero(7), Utils.RandomPickNumberExcludingZero(10), Utils.RandomPickNumberExcludingZero(15), Utils.RandomPickNumberExcludingZero(20)
        };
        
        for (var i = (_lastChunk + 1) * 38.25f + 2.55f; i < (_lastChunk + 2) * 38.25 + 2.55f; i += 2.55f)
        {
            Instantiate(watter, new Vector3(i, -3.55f, 1), Quaternion.identity);
            Instantiate(emptyPlatform, new Vector3(i, -6.11f, 1), Quaternion.identity);
            InstantiatePlatform(i);
        }

        for (var i = (_lastChunk + 1) * 38.25f + 2.55f; i < (_lastChunk + 2) * 38.25 + 2.55f; i += 19)
        {
            for (var j = 0; j < 4; j++)
            {
                var background = new GameObject("Background");
                var spriteRenderer = background.AddComponent<SpriteRenderer>();

                spriteRenderer.sprite = backgrounds[j == 0 ? 0 : 1];
                spriteRenderer.sortingOrder = -1;
                
                background.transform.rotation = Quaternion.Euler(j%2==0? 0 : 180, 0, 0);
                background.transform.position = new Vector3(i, j * 10.8f, 0f);
            }
        }
    }
    
    private void ClearLastChunk()
    {
        var startPoint = new Vector2((_lastChunk - 2) * 38.25f - 10, -10);
        var endPoint = new Vector2((_lastChunk - 1) * 38.25f - 2.55f, 30);
        
        foreach (var objectCollider in Physics2D.OverlapAreaAll(startPoint, endPoint))
            Destroy(objectCollider.gameObject);

        foreach (var obj in FindObjectsOfType<GameObject>().Where(obj => obj.name == "Background" && IsWithinPoints(startPoint, endPoint, obj.transform.position)))
            Destroy(obj);

        var wall = Instantiate(emptyPlatform, new Vector2((_lastChunk - 1) * 38.25f - 20, 8), Quaternion.identity);
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
                    var platformObject = Instantiate(platform, new Vector3(xPosition,  i * 2.55f * 2 -3.55f, 0), Quaternion.identity);
                    platformObject.GetComponent<Platform>().platformType =
                        i == 0 ? (PlatformType)Utils.RandomPickNumberBetween(0, 2) : 
                            (PlatformType)Utils.RandomPickNumberBetween(0, Enum.GetValues(typeof(PlatformType)).Length);
                }
                else
                {
                    _platformsLengths[i] = Utils.RandomPickNumberExcludingZero(10);
                    _platformsGaps[i] = Utils.RandomPickNumberExcludingZero(2);
                }
            }
            else
                _platformsGaps[i]--;   
        }
    }

    private static bool IsWithinPoints(Vector3 startPoint, Vector3 endPoint, Vector3 position) => 
        position.x >= Mathf.Min(startPoint.x, endPoint.x) && position.x <= Mathf.Max(startPoint.x, endPoint.x) && position.y >= Mathf.Min(startPoint.y, endPoint.y) && position.y <= Mathf.Max(startPoint.y, endPoint.y);
}
