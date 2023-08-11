using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public int gold;
    public int gems;

    public int atkLvl;
    public int msLvl;
    public int jumpLvl;
    public int defLvl;
    public int hpLvl;

    public int jumpBuffs;
    public int speedBuffs;
    public int secondChances;

    public int atkPrice;
    public int msPrice;
    public int jumpPrice;
    public int defPrice;
    public int hpPrice;

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
        {
            Initialize();
            return;
        }

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
  
        SetUpPrice();
    }

    public void Save() =>
        SaveAndLoadSystem.SavePlayer(this);

    public void ResetPlayer()
    {
        SaveAndLoadSystem.DeletePlayerSave();
        Initialize();
    }

    private void Initialize()
    {
        gold = 5_000;
        gems = 50;
        
        atkLvl = 0;
        msLvl = 0;
        jumpLvl = 0;
        defLvl = 0;
        hpLvl = 0;
        
        jumpBuffs = 3;
        speedBuffs = 3;
        secondChances = 1;
        
        SetUpPrice();
    }

    private void SetUpPrice()
    {
        atkPrice = PriceCalculator(atkLvl);
        msPrice = PriceCalculator(msLvl);
        jumpPrice = PriceCalculator(jumpLvl);
        defPrice = PriceCalculator(defLvl);
        hpPrice = PriceCalculator(hpLvl);
    }
    
    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;
}
