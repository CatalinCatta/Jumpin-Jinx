using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : IndestructibleManager<PrefabManager>
{
    [Header("Walls")] [SerializeField] public GameObject
        endlessModeWall;

    [Header("Backgrounds")] [SerializeField]
    public Sprite
        lightLvlBackground,
        darkLvlBackground,
        lightEndlessBackgorund,
        darkEndlessBackground;

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