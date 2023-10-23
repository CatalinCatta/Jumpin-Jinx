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
        chunkHeight = 17,
        chunkLength = 30,
        maxNrOfBlocksInRow = 5,
        verticalMinSpaceBetweenBlocks = 3,
        verticalMaxJumpGap = 3,
        horizontalMaxJumpGap = 2;

    [SerializeField] private bool selfCalculateJumpValues = true;
    [SerializeField] private float unitSize = 1.28f;
    [SerializeField] private Vector2 startingPosition;

    [Header("PreloadedObjects")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject preBuildObjectsParent;

    [Header("Utils")]
    private GameObject[][,] _maps;
    private int _currentChunk;
    private PrefabManager _prefabManager;
    private GameObject[] _chunksParents;
    
    private void Start()
    {
        _maps = new GameObject[2][,];
        for (var i = 0; i < 2; i++) _maps[i] = new GameObject[chunkHeight, chunkLength];
        _prefabManager = PrefabManager.Instance;
        _chunksParents = new[] { new GameObject(), new GameObject(), new GameObject() };
        if (player == null) player = GameObject.FindGameObjectsWithTag("Player")[0].transform;

        if (selfCalculateJumpValues)
        {
            var playerManager = PlayerManager.Instance;
            verticalMaxJumpGap = (int)(playerManager.Upgrades[(int)UpgradeType.JumpPower].Quantity / 16.66666f) + 1;
            // horizontalMaxJumpGap = 
        }

        _currentChunk = (int)(startingPosition.x / chunkLength / unitSize);
        //GenerateMap(chunkHeight - 1 - (int)(startingPosition.y / unitSize), (int)(startingPosition.x / unitSize), true);
        SetUpFirstPlatform();
        
        for (var i = 0; i < chunkLength; i++)
            Instantiate(_prefabManager.acid, new Vector3((i + _currentChunk * chunkLength) * unitSize, 0, 5), Quaternion.identity,
                _chunksParents[2].transform);
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
        if ((int)(startingPosition.x / chunkLength / unitSize) == _currentChunk - 2) Destroy(preBuildObjectsParent);

        Destroy(_chunksParents[0]);
        _chunksParents[0] = _chunksParents[1];
        _chunksParents[1] = _chunksParents[2];
        _chunksParents[2] = new GameObject();

        // Shift arrays stored.
        _maps[0] = _maps[1];
        _maps[1] = new GameObject[chunkHeight, chunkLength];
    }

    #endregion

    #region GenerateChunk
    
    private void GenerateNewChunk()
    {
        // New wall.
        var wallPrefab = _prefabManager.endlessModeWall;
        var wallScale = wallPrefab.transform.GetComponent<Collider2D>().bounds.size;
        Instantiate(wallPrefab, new Vector2((_currentChunk - 2) * chunkLength - wallScale.x / 2, wallScale.y / 2),
            Quaternion.identity, _chunksParents[0].transform);

        // New acid
        for (var i = 0; i < chunkLength; i++)
            Instantiate(_prefabManager.acid, new Vector3((i + _currentChunk * chunkLength) * unitSize, 0, 5),
                Quaternion.identity, _chunksParents[2].transform);
        Instantiate(_prefabManager.acidBottom, new Vector2((_currentChunk + 2) * chunkLength * unitSize / 2, -6.2f),
            Quaternion.identity, _chunksParents[2].transform).transform.localScale = new Vector3(20, 5, 1);
        
        // New platforms
        for (var i = chunkHeight - 1; i > -1; i--)
        {
            for (var j = chunkLength - horizontalMaxJumpGap - 1; j < chunkLength; j++)
            {
                if (_maps[0][i, j] == null) continue;
                GenerateMap(i, 0, true);
                break;
            }
        }
    }

    private void GenerateMap(int x, int y, bool continueWithoutGap)
    {
        if ((y >= chunkLength - horizontalMaxJumpGap && !continueWithoutGap)|| y >= chunkLength) return;

        // 50% chance for normal platform.
        var platformType = continueWithoutGap ? PlatformType.Static :
            Utility.GetRandomNumberExcludingZero(3) == 1 ? PlatformType.Static :
            (PlatformType)Utility.GetRandomNumberExcludingZero(Enum.GetValues(typeof(PlatformType)).Length - 1); // removed last platform (Circular Platform)

        if (continueWithoutGap || platformType != PlatformType.Static)
        {
            if (!CanBuildPlatform(x, y)) return;
    
            GeneratePlatforms(x, y, platformType);
        }
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
                if (x - i < chunkHeight && x - i >= 0 && CanBuildPlatform(x - i, y + widthGap))
                    possibleContinuations.Add(x - i);
            
            if (possibleContinuations.Count == 0) return;
            GeneratePlatforms(possibleContinuations[Utility.GetRandomNumberBetween(0, possibleContinuations.Count)],
                y + widthGap, platformType);

        }
    }

    private void GeneratePlatforms(int x, int y, PlatformType platformType)
    {
        switch (platformType)
        {
            case PlatformType.VerticalMoving:
                var height = Utility.GetRandomNumberExcludingZero(chunkHeight / 2);
                for (var i = 0; i < height + 1; i++)
                {
                    if (x - i < 0 || _maps[1][x - i, y] != null)
                    {
                        CreateVerticalPlatform(i - 1);
                        return;
                    }
                    CreateObject((x - i, y), _prefabManager.ghostBlock);
                }
                CreateVerticalPlatform(height);
                return;
            
            case PlatformType.HorizontalMoving:
                var length = Utility.GetRandomNumberExcludingZero(chunkLength / 3);
                for (var i = 0; i < length + 1; i++)
                {
                    if (y + i >= chunkLength || !CanBuildPlatform(x, y + i))
                    {
                        CreateHorizontalPlatform(i - 1);
                        return;
                    }
                    CreateObject((x, y + i), _prefabManager.ghostBlock);
                }
                CreateHorizontalPlatform(length);
                return;
            
            default:
                var len = Utility.GetRandomNumberExcludingZero(maxNrOfBlocksInRow /
                                                               (platformType == PlatformType.Temporary ? 2 : 1));
                for (var i = y; i < y + len; i++)
                {
                    if (i >= chunkLength || !CanBuildPlatform(x, i))
                    {
                        SetUpStaticPlatform(i - 1);
                        return;
                    }

                    CreateObject((x, i),
                        platformType == PlatformType.Temporary
                            ? _prefabManager.temporaryPlatform
                            : _prefabManager.grass);
                }
                SetUpStaticPlatform(y + len-1);
                return;
        }

        void CreateVerticalPlatform(int movementValue)
        {
            if (movementValue > verticalMinSpaceBetweenBlocks) GenerateMap(x, y + 1, true);
            var platform = CreateObject((x, y), _prefabManager.verticalMovingPlatform).GetComponent<SidewaysMoving>();
            platform.direction = SidewaysMoving.Direction.Vertical;
            platform.movement = movementValue * unitSize;
            platform.collideWhitGhostBlock = false;
            
            GenerateMap(x - movementValue, y + 1, true);
        }

        void CreateHorizontalPlatform(int movementValue)
        {
            if (x + verticalMinSpaceBetweenBlocks < chunkHeight && CanBuildPlatform(x + verticalMinSpaceBetweenBlocks, y))
                GenerateMap(x + verticalMinSpaceBetweenBlocks, y, true);
            var platform = CreateObject((x, y), _prefabManager.horizontalMovingPlatform).GetComponent<SidewaysMoving>();
            platform.direction = SidewaysMoving.Direction.Horizontal;
            platform.movement = movementValue * unitSize;
            platform.collideWhitGhostBlock = false;
            GenerateMap(x, y + movementValue + 1, true);
        }
        
        void SetUpStaticPlatform(int len)
        {
            _maps[1][x, y].GetComponent<RandomEnvironmentCreator>().allowDamageObject = false;
            if (len > y) _maps[1][x, len].GetComponent<RandomEnvironmentCreator>().allowDamageObject = false;
            GenerateMap(x, len + 1, platformType == PlatformType.Temporary);
        }
        
    }

    private GameObject CreateObject((int x, int y) position, GameObject prefab) => _maps[1][position.x, position.y] =
        Instantiate(prefab,
            new Vector2(position.y + _currentChunk * chunkLength, chunkHeight - 1 - position.x) * unitSize,
            Quaternion.identity, _chunksParents[2].transform);

    private bool CanBuildPlatform(int x, int y)
    {
        if (x < chunkHeight - verticalMinSpaceBetweenBlocks) return CheckUnderBlock(verticalMinSpaceBetweenBlocks);
        for (var i = 1; i < verticalMinSpaceBetweenBlocks + 1; i++)
            if (x == chunkHeight - i)
                return CheckUnderBlock(i);
        return false;
        
        bool CheckUnderBlock(int unities)
        {
            for (var i = 0; i < unities; i++)
                if (_maps[1][x + i, y] != null)
                    return false;
            return true;
        }
    }
    #endregion

    private void SetUpFirstPlatform()
    {
        var platformRows = Utility.GetRandomNumberExcludingZero((chunkHeight - verticalMinSpaceBetweenBlocks - 2) /
                                                                (verticalMinSpaceBetweenBlocks + 1) -1);
        GenerateMap(chunkHeight - 1, 0, true);
        for (var i = 0; i < platformRows + 1; i++)
            GenerateMap(chunkHeight - 1 - (chunkHeight - 1) / (platformRows + 1) * i, 0, true);
        GenerateMap(0, 0, true);
    }
}