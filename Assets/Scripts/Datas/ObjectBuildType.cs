using System.Collections.Generic;
using UnityEngine;

public enum ObjectBuildType
{
    Null,

    Dirt,
    StaticGrass,
    TemporaryPlatform,
    UpMovingPlatform,
    DownMovingPlatform,
    LeftMovingPlatform,
    RightMovingPlatform,
    CircularMovingPlatform,
    CounterCircularMovingPlatform,
    HalfSlopeDirt,
    HalfSlopeDirtRotated,
    SlopeDirt,
    SlopeDirtRotated,

    Acid,
    AcidBottom,
    Spike,
    SpikeUpsideDown,
    SpikeLeft,
    SpikeRight,

    PlantPlaceHolder,

    Player,
    EndLvl,
    Coin,
    Health,
    Spider
}

public enum ObjectBuildCategory
{
    Block,
    Damage,
    Plant,
    Object,
    Enemy
}

public static partial class Dictionaries
{
    public static readonly
        Dictionary<ObjectBuildType, (ObjectBuildCategory category, char character, GameObject prefab, Sprite sprite,
            Quaternion rotation)> ObjectBuild = new()
        {
            {
                ObjectBuildType.Dirt,
                (ObjectBuildCategory.Block, 'D', PrefabManager.Instance.dirt, PrefabManager.Instance.dirtSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.StaticGrass,
                (ObjectBuildCategory.Block, 'G', PrefabManager.Instance.grass, PrefabManager.Instance.grassSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.TemporaryPlatform,
                (ObjectBuildCategory.Block, 'T', PrefabManager.Instance.temporaryPlatform,
                    PrefabManager.Instance.temporaryPlatformSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.UpMovingPlatform,
                (ObjectBuildCategory.Block, '^', PrefabManager.Instance.verticalMovingPlatform,
                    PrefabManager.Instance.verticalMovingPlatformSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.DownMovingPlatform,
                (ObjectBuildCategory.Block, 'v', PrefabManager.Instance.verticalMovingPlatform,
                    PrefabManager.Instance.verticalMovingPlatformSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.RightMovingPlatform,
                (ObjectBuildCategory.Block, 'R', PrefabManager.Instance.horizontalMovingPlatform,
                    PrefabManager.Instance.horizontalMovingPlatformSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.LeftMovingPlatform,
                (ObjectBuildCategory.Block, 'L', PrefabManager.Instance.horizontalMovingPlatform,
                    PrefabManager.Instance.horizontalMovingPlatformSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.CircularMovingPlatform,
                (ObjectBuildCategory.Block, 'O', PrefabManager.Instance.circularMovingPlatform,
                    PrefabManager.Instance.circularMovingPlatformSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.CounterCircularMovingPlatform,
                (ObjectBuildCategory.Block, 'o', PrefabManager.Instance.circularMovingPlatform,
                    PrefabManager.Instance.circularMovingPlatformSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.HalfSlopeDirt,
                (ObjectBuildCategory.Block, '}', PrefabManager.Instance.halfSlopeGrass,
                    PrefabManager.Instance.halfSlopeGrassSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.HalfSlopeDirtRotated,
                (ObjectBuildCategory.Block, '{', PrefabManager.Instance.halfSlopeGrass,
                    PrefabManager.Instance.halfSlopeGrassSprite, Quaternion.Euler(0, 180, 0))
            },
            {
                ObjectBuildType.SlopeDirt,
                (ObjectBuildCategory.Block, '>', PrefabManager.Instance.slopeGrass,
                    PrefabManager.Instance.slopeGrassSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.SlopeDirtRotated,
                (ObjectBuildCategory.Block, '<', PrefabManager.Instance.slopeGrass,
                    PrefabManager.Instance.slopeGrassSprite,
                    Quaternion.Euler(0, 180, 0))
            },
            {
                ObjectBuildType.Acid,
                (ObjectBuildCategory.Block, 'W', PrefabManager.Instance.acid, PrefabManager.Instance.acidSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.AcidBottom,
                (ObjectBuildCategory.Block, 'M', PrefabManager.Instance.acidBottom,
                    PrefabManager.Instance.acidBottomSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.Spike,
                (ObjectBuildCategory.Damage, 's', PrefabManager.Instance.spike, PrefabManager.Instance.spikeSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.SpikeUpsideDown,
                (ObjectBuildCategory.Damage, 'S', PrefabManager.Instance.spike, PrefabManager.Instance.spikeSprite,
                    Quaternion.Euler(0, 0, 180))
            },
            {
                ObjectBuildType.SpikeLeft,
                (ObjectBuildCategory.Damage, '[', PrefabManager.Instance.spike, PrefabManager.Instance.spikeSprite,
                    Quaternion.Euler(0, 0, 90))
            },
            {
                ObjectBuildType.SpikeRight,
                (ObjectBuildCategory.Damage, ']', PrefabManager.Instance.spike, PrefabManager.Instance.spikeSprite,
                    Quaternion.Euler(0, 0, 270))
            },
            {
                ObjectBuildType.PlantPlaceHolder,
                (ObjectBuildCategory.Plant, '.', PrefabManager.Instance.plantPlaceHolder,
                    PrefabManager.Instance.plantPlaceHolderSprite, Quaternion.identity)
            },
            {
                ObjectBuildType.Player,
                (ObjectBuildCategory.Object, 'P', PrefabManager.Instance.player, PrefabManager.Instance.playerSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.EndLvl,
                (ObjectBuildCategory.Object, 'X', PrefabManager.Instance.endLvl, PrefabManager.Instance.endLvlSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.Coin,
                (ObjectBuildCategory.Object, 'C', PrefabManager.Instance.coin, PrefabManager.Instance.coinSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.Health,
                (ObjectBuildCategory.Object, 'H', PrefabManager.Instance.heal, PrefabManager.Instance.healSprite,
                    Quaternion.identity)
            },
            {
                ObjectBuildType.Spider,
                (ObjectBuildCategory.Enemy, 'p', PrefabManager.Instance.spider, PrefabManager.Instance.spiderSprite,
                    Quaternion.identity)
            }
        };
}