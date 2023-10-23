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
    public static readonly Dictionary<ObjectBuildType, (ObjectBuildCategory category, char character, GameObject prefab)> ObjectBuild = new()
    {
        { ObjectBuildType.Dirt, (ObjectBuildCategory.Block, 'D', PrefabManager.Instance.dirt) },
        { ObjectBuildType.StaticGrass, (ObjectBuildCategory.Block, 'G', PrefabManager.Instance.grass) },
        { ObjectBuildType.TemporaryPlatform, (ObjectBuildCategory.Block, 'T', PrefabManager.Instance.temporaryPlatform) },
        { ObjectBuildType.UpMovingPlatform, (ObjectBuildCategory.Block, '^', PrefabManager.Instance.verticalMovingPlatform) },
        { ObjectBuildType.DownMovingPlatform, (ObjectBuildCategory.Block, 'v', PrefabManager.Instance.verticalMovingPlatform) },
        { ObjectBuildType.RightMovingPlatform, (ObjectBuildCategory.Block, 'R', PrefabManager.Instance.horizontalMovingPlatform) },
        { ObjectBuildType.LeftMovingPlatform, (ObjectBuildCategory.Block, 'L', PrefabManager.Instance.horizontalMovingPlatform) },
        { ObjectBuildType.CircularMovingPlatform, (ObjectBuildCategory.Block, 'O', PrefabManager.Instance.circularMovingPlatform) },
        { ObjectBuildType.CounterCircularMovingPlatform, (ObjectBuildCategory.Block, 'o', PrefabManager.Instance.circularMovingPlatform) },
        { ObjectBuildType.HalfSlopeDirt, (ObjectBuildCategory.Block, '}', PrefabManager.Instance.halfSlopeGrass) },
        { ObjectBuildType.HalfSlopeDirtRotated, (ObjectBuildCategory.Block, '{', PrefabManager.Instance.halfSlopeGrass) },
        { ObjectBuildType.SlopeDirt, (ObjectBuildCategory.Block, '>', PrefabManager.Instance.slopeGrass) },
        { ObjectBuildType.SlopeDirtRotated, (ObjectBuildCategory.Block, '<', PrefabManager.Instance.slopeGrass) },
        
        { ObjectBuildType.Acid, (ObjectBuildCategory.Damage, 'W', PrefabManager.Instance.acid) },
        { ObjectBuildType.AcidBottom, (ObjectBuildCategory.Damage, 'M', PrefabManager.Instance.acidBottom) },
        { ObjectBuildType.SpikeUpsideDown, (ObjectBuildCategory.Damage, 'S', PrefabManager.Instance.spike) },
        { ObjectBuildType.Spike, (ObjectBuildCategory.Damage, 's', PrefabManager.Instance.spike) },
        { ObjectBuildType.SpikeLeft, (ObjectBuildCategory.Damage, '[', PrefabManager.Instance.spike) },
        { ObjectBuildType.SpikeRight, (ObjectBuildCategory.Damage, ']', PrefabManager.Instance.spike) },
        
        { ObjectBuildType.PlantPlaceHolder, (ObjectBuildCategory.Plant, '.', PrefabManager.Instance.plantPlaceHolder) },
        
        { ObjectBuildType.Player, (ObjectBuildCategory.Object, 'P', PrefabManager.Instance.player) },
        { ObjectBuildType.EndLvl, (ObjectBuildCategory.Object, 'X', PrefabManager.Instance.endLvl) },
        { ObjectBuildType.Coin, (ObjectBuildCategory.Object, 'C', PrefabManager.Instance.coin) },
        { ObjectBuildType.Health, (ObjectBuildCategory.Object, 'H', PrefabManager.Instance.heal) },
        
        { ObjectBuildType.Spider, (ObjectBuildCategory.Enemy, 'S', PrefabManager.Instance.spider) }
    };
}