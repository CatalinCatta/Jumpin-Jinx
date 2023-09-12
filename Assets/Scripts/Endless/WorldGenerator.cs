using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the endless game chunk by chunk, including platforms, background and an wall that prevent player from going back.
/// Keeping always 3 chunks actives, (last, current, next).
/// Delete any gameObject existent in last chunk.
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private int
        chunkHeight = 16,
        chunkLength = 30,
        maxNrOfBlocksInRow = 5,
        verticalMinSpaceBetweenBlocks = 3,
        verticalMaxJumpGap = 4,
        horizontalMaxJumpGap = 3;
    [SerializeField] private float unitSize = 1.28f;
    [SerializeField] private Vector2 startingPosition;

    [Header("PreloadedObjects")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject[] backgrounds;
    
    [Header("Utils")]
    private GameObject[][,] _maps;
    private int _currentChunk;
    private PrefabManager _prefabManager;
    
    private void Start()
    {
        _maps = new GameObject[,][3];
        for (var i = 0; i < 3; i++) _maps[i] = new GameObject[chunkHeight, chunkLength];
        _prefabManager = (PrefabManager)IndestructibleManager.Instance;
        if (player == null) player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        // TODO: Change verticalMaxJumpGap and horizontalMaxJumpGap based on player jump and ms.
    }
    
    private void Update()
    {
        if (player.position.x < _currentChunk * chunkLength * unitSize) return;
        
        _currentChunk++;
        ClearLastChunk();
        GenerateNewChunk();
    }

    #region ClearChunk
    
    private void ClearLastChunk()
    {
        // Destroy wall.
        Destroy(wall);

        // Destroy backgrounds.
        foreach (var background in backgrounds) Destroy(background);

        // Destroy objects.
        for (var i = 0; i < chunkHeight; i++)
        for (var j = 0; j < chunkLength; j++)
            Destroy(_maps[0][i, j]);
        
        // Shift maps stored.
        _maps[0] = _maps[1];
        _maps[1] = _maps[2];
        _maps[3] = new GameObject[chunkHeight, chunkLength];
    }

    #endregion

    #region GenerateChunk
    
    private void GenerateNewChunk()
    {
        // New wall.
        var wallPrefab = _prefabManager.endlessModeWall;
        var wallScale = wallPrefab.transform.GetComponent<Collider>().bounds.size;
        wall = Instantiate(wallPrefab,
            new Vector2((_currentChunk - 2) * chunkLength - wallScale.x / 2, wallScale.y / 2), Quaternion.identity);
        
        // New backgrounds.
        // TODO: Add background when ready.
        
        // New platforms.
        if (_currentChunk == 1)
            GenerateMap((int)(startingPosition.x / unitSize), (int)(startingPosition.y / unitSize), true);
        else
            for (var i = chunkHeight - 1; i < -1; i--)
            for (var j = chunkLength - horizontalMaxJumpGap - 1; j < chunkLength; j++)
                if (_maps[1][i, j] != null)
                    GenerateMap(i, j, false);
    }

    private void GenerateMap(int x, int y, bool continueWithoutGap)
    {
        if (y >= chunkLength - verticalMaxJumpGap) return;

        if (continueWithoutGap) GeneratePlatforms(x, y);
        else
        {
            var widthGap = Utility.GetRandomNumberExcludingZero(horizontalMaxJumpGap + 1);

            for (var i = 0; i < (verticalMaxJumpGap * 2 + 1) / (verticalMinSpaceBetweenBlocks + 1) + 1; i++)
                GeneratePlatformRandomPositioned(widthGap);
        }

        void GeneratePlatformRandomPositioned(int widthGap)
        {
            var possibleContinuations = new List<int>();
            for (var i = -verticalMaxJumpGap; i < verticalMaxJumpGap + 1; i++)
                if (x-i < chunkHeight && x-i >= 0 && CanBuildPlatform(x - i, y + widthGap))
                    possibleContinuations.Add(x - i);
            GeneratePlatforms(possibleContinuations[Utility.GetRandomNumberExcludingZero(possibleContinuations.Count)],
                y + widthGap);

        }
    }

    private void GeneratePlatforms(int x, int y)
    {
        // 50% chance for normal platform.
        var platformType = Utility.GetRandomNumberExcludingZero(3) == 1
            ? PlatformType.Static
            : (PlatformType)Utility.GetRandomNumberExcludingZero(Enum.GetValues(typeof(UpgradeType)).Length);
        (int, int) finalPosition;
        
        switch (platformType)
        {
            case PlatformType.VerticalMoving:
                var height = Utility.GetRandomNumberExcludingZero(chunkHeight/2 + 1) - 1;
                finalPosition = (x - height, height);
                for (var i = 0; i < height + 1; i++)
                {
                    if (x - i < 0 || _maps[2][x - i, y] != null)
                    {
                        finalPosition = (x - i + 1, height);
                        break;
                    }
                    CreateObject((x - i, y), _prefabManager.ghostBlock);
                }
                CreateVerticalPlatform(finalPosition.Item1, finalPosition.Item2);
                break;
            
            case PlatformType.HorizontalMoving:
                var length = Utility.GetRandomNumberExcludingZero(chunkLength / 3 + 1) - 1;
                finalPosition = (y+ length, length);
                for (var i = 0; i < length + 1; i++)
                {
                    if (y + i >= chunkLength || _maps[2][x, y + i] != null)
                    {
                        finalPosition = (y + i - 1, length);
                        break;
                    }   
                    CreateObject((x, y + i), _prefabManager.ghostBlock);
                }
                CreateHorizontalPlatform(finalPosition.Item1, finalPosition.Item2);
                break;
            
            default:
                var len = Utility.GetRandomNumberExcludingZero(maxNrOfBlocksInRow);
                finalPosition = (x, y + len - 1);
                for (var i = y; i < y + len; i++)
                {
                    if (i < chunkLength && CanBuildPlatform(x, i))
                        CreateObject((x, i),
                            platformType == PlatformType.Temporary
                                ? _prefabManager.temporaryPlatform
                                : _prefabManager.grass);
                    else
                    {
                        finalPosition = (x, i - 1);
                        break;
                    }
                }
                GenerateMap(finalPosition.Item1, finalPosition.Item2, platformType == PlatformType.Temporary);
                break;
        }

        void CreateVerticalPlatform(int xPosition, float movement)
        {
            var platform = CreateObject((xPosition, y), _prefabManager.verticalMovingPlatform)
                .GetComponent<SidewaysMoving>();
            platform.direction = SidewaysMoving.Direction.Vertical;
            platform.movement = movement * unitSize;
            GenerateMap(xPosition, y, true);
        }
        
        void CreateHorizontalPlatform(int yPosition, float movement)
        {
            var platform = CreateObject((x, yPosition), _prefabManager.horizontalMovingPlatform)
                .GetComponent<SidewaysMoving>();
            platform.direction = SidewaysMoving.Direction.Horizontal;
            platform.movement = movement * unitSize;
            GenerateMap(x, yPosition, true);
        }
        
        GameObject CreateObject((int x, int y) position, GameObject prefab) => _maps[2][position.x, position.y] =
            Instantiate(prefab, new Vector2(position.y, chunkHeight - position.x) * _currentChunk * unitSize,
                Quaternion.identity);
    }

    private bool CanBuildPlatform(int x, int y)
    {
        if (x < chunkHeight - verticalMinSpaceBetweenBlocks) return CheckUnderBlock(verticalMinSpaceBetweenBlocks);
        for (var i = 1; i < verticalMinSpaceBetweenBlocks + 1; i++)
            if (x == chunkHeight - i)
                return CheckUnderBlock(i);
        return false;
        
        bool CheckUnderBlock(int unities)
        {
            for (var i = 1; i < unities; i++)
                if (_maps[2][x + i, y] != null)
                    return false;
            return true;
        }
    }

    #endregion
}