using System;

/// <summary>
/// Manages player-related data and upgrades.
/// </summary>
public class PlayerManager : IndestructibleManager
{
    [NonSerialized] public Enhancement[] Upgrades, Buffs;
    [NonSerialized] public int Gold, Gems;

    protected override void DoSomethingAtAwakeBeginning() => Load();

    private void Load()
    {
        Upgrades = new Enhancement[Enum.GetValues(typeof(UpgradeType)).Length];
        Buffs = new Enhancement[Enum.GetValues(typeof(BuffType)).Length];
        
        var player = SaveAndLoadSystem.LoadPlayer();

        if (player == null)
        {
            Initialize();
            return;
        }
        
        Gold = player.gold;
        Gems = player.gems;

        Upgrades = player.Upgrades;
        Buffs = player.Buffs;

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
        Gold = 5_000;
        Gems = 50;

        SetUpEnhancement();
    }

    private void SetUpEnhancement()
    {
        foreach (var upgrade in Upgrades) upgrade.Price = PriceCalculator(upgrade.Quantity);
        for (var i = 0; i < Enum.GetValues(typeof(BuffType)).Length; i++)
            Buffs[i].Price = Dictionaries.BuffDetails[(BuffType)i].price;
    }

    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;
}