using UnityEngine;

public class PrefabManager : IndestructibleManager<PrefabManager>
{
    [Header("Walls")] [SerializeField] public GameObject endlessModeWall;

    [Header("Backgrounds")] [SerializeField]
    public Sprite lightLvlBackground;

    [SerializeField] public Sprite darkLvlBackground, lightEndlessBackgorund, darkEndlessBackground;

    [Header("BLOCKS")] 
    [Header("BlocksPrefabs")] [SerializeField] public GameObject ghostBlock;

    [SerializeField] public GameObject 
        grass,
        dirt,
        slopeGrass,
        halfSlopeGrass,
        temporaryPlatform,
        horizontalMovingPlatform,
        verticalMovingPlatform,
        circularMovingPlatform,
        acid,
        acidBottom;

    [Header("BlocksSprites")] [SerializeField]
    public Sprite ghostBlockSprite;

    [SerializeField] public Sprite
        grassSprite,
        dirtSprite,
        slopeGrassSprite,
        halfSlopeGrassSprite,
        temporaryPlatformSprite,
        horizontalMovingPlatformSprite,
        verticalMovingPlatformSprite,
        circularMovingPlatformSprite,
        acidSprite,
        acidBottomSprite;

    [Header("OBJECTS")] [Header("ObjectsPrefabs")] [SerializeField]
    public GameObject player;

    [SerializeField] public GameObject
        coin,
        gem,
        endLvl,
        heal,
        spike;

    [Header("ObjectsSprites")] [SerializeField]
    public Sprite playerSprite;

    [SerializeField] public Sprite
        coinSprite,
        gemSprite,
        endLvlSprite,
        healSprite,
        spikeSprite;

    [Header("Enemy")] 
    [SerializeField] public GameObject
        spider;
    [SerializeField] public Sprite
        spiderSprite;

    [Header("PLANTS")] [Header("PlantsPrefabs")] [SerializeField]
    public GameObject plantPlaceHolder;

    [Header("PlantsSprites")] [SerializeField]
    public Sprite plantPlaceHolderSprite;
}