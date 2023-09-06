using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour  // TODO: Add this to unity.
{
    public static PrefabManager Instance;

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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}