using System;

/// <summary>
/// Manages player-related data and upgrades.
/// </summary>
public class PlayerManager : IndestructibleManager<PlayerManager>
{
    [NonSerialized] public Enhancement[] Upgrades, Buffs;
    [NonSerialized] public int Gold, Gems;
    
    protected override void Awake()
    {
        base.Awake();
        Load();
    }
    
    private void Load()
    {
        var player = SaveAndLoadSystem.LoadPlayer();

        if (player == null)
        {
            Initialize();
            return;
        }

        Gold = player.gold;
        Gems = player.gems;

        if (player.Upgrades == null) SetUpUpgrades();
        else Upgrades = player.Upgrades;

        if (player.Buffs == null) SetUpBuffs();
        else Buffs = player.Buffs;

        SetUpEnhancement();
    }

    public void Save() => SaveAndLoadSystem.SavePlayer(this);

    public void ResetPlayer()
    {
        SaveAndLoadSystem.DeletePlayerSave();
        Initialize();
    }

    private void Initialize()
    {
        SetUpUpgrades();
        SetUpBuffs();
        
        Gold = 0;
        Gems = 0;

        SetUpEnhancement();
    }

    private void SetUpEnhancement()
    {
        foreach (var upgrade in Upgrades) upgrade.Price = PriceCalculator(upgrade.Quantity);
        for (var i = 0; i < Enum.GetValues(typeof(BuffType)).Length; i++)
            Buffs[i].Price = Dictionaries.BuffDetails[(BuffType)i].price;
    }

    private void SetUpUpgrades()
    {
        Upgrades = new Enhancement[Enum.GetValues(typeof(UpgradeType)).Length];
        for (var i = 0; i < Enum.GetValues(typeof(UpgradeType)).Length; i++) Upgrades[i] = new Enhancement();
    }

    private void SetUpBuffs()
    {
        Buffs = new Enhancement[Enum.GetValues(typeof(BuffType)).Length];
        for (var i = 0; i < Enum.GetValues(typeof(BuffType)).Length; i++) Buffs[i] = new Enhancement();
    }
    
    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;
}