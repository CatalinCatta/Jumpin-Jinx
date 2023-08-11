using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public int gold = 10_000;
    public int gems = 50;

    public int atkLvl;
    public int msLvl;
    public int jumpLvl;
    public int defLvl;
    public int hpLvl;

    public int jumpBuffs = 3;
    public int speedBuffs = 3;
    public int secondChances;

    public int atkPrice = 15;
    public int msPrice = 15;
    public int jumpPrice = 15;
    public int defPrice = 15;
    public int hpPrice = 15;

    public int jumpBuffsPrice = 200;
    public int speedBuffsPrice = 200;
    public int secondChancesPrice = 2_000;
    
    private void Awake()
    {
        Load();
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Load()
    {
        var player = SaveAndLoadSystem.LoadPlayer();

        if (player == null)
            return;
        
        gold = player.gold;
        gems = player.gems;
        
        atkLvl = player.atkLvl;
        msLvl = player.msLvl;
        jumpLvl = player.jumpLvl;
        defLvl = player.defLvl;
        hpLvl = player.hpLvl;
        
        jumpBuffs = player.jumpBuffs;
        speedBuffs = player.speedBuffs;
        secondChances = player.secondChances;
    }

    public void Save() =>
        SaveAndLoadSystem.SavePlayer(this);
}
