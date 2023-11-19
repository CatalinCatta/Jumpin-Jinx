using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages player-related data and upgrades.
/// </summary>
public class PlayerManager : IndestructibleManager<PlayerManager>
{
    [NonSerialized] public Enhancement[] Upgrades, Buffs;
    [NonSerialized] public int Gold, Gems;
    [NonSerialized] public Dictionary<string, List<string>> Skins;
    [NonSerialized] public Dictionary<string, string> CurrentSkin;

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

        if (player.upgrades == null) SetUpUpgrades();
        else Upgrades = player.upgrades;

        if (player.buffs == null) SetUpBuffs();
        else Buffs = player.buffs;

        if (player.Skins == null) SetUpSkins();
        else Skins = player.Skins;
        
        if (player.CurrentSkin == null) SetUpCurrentSkin();
        else CurrentSkin = player.CurrentSkin;
        
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
        SetUpSkins();
        
        Gold = 0;
        Gems = 0;

        SetUpEnhancement();
        SetUpCurrentSkin();
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

    private void SetUpSkins() => Skins = new Dictionary<string, List<string>>
        { { Dictionaries.Skin[Skin.Classic].name, PrefabManager.Instance.playerSkins.GetCategoryNames().ToList() } };
    
    private void SetUpCurrentSkin()
    {
        CurrentSkin = new Dictionary<string, string>();
        foreach (var bodyPart in PrefabManager.Instance.playerSkins.GetCategoryNames())
            CurrentSkin.Add(bodyPart, Dictionaries.Skin[Skin.Classic].name);
    }
    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;
}