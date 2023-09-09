using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : IndestructibleManager  // TODO: Add this to unity.
{
    [Header("Blocks")] [SerializeField] public GameObject
        ghostBlock,
        grass,
        dirt,
        slopeGrass,
        halfSlopeGrass,
        temporaryPlatform,
        horizontalMovingPlatform,
        verticalMovingPlatform,
        circularMovingPlatform,
        watter,
        watterBottom;

    [Header("Objects")] [SerializeField] public GameObject
        player,
        coin,
        endLvl,
        enemy,
        heal,
        spike;
    
    public List<Sprite> plantSprites;
}