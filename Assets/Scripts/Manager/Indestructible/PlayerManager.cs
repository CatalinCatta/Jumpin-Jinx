using System;
using UnityEngine;

/// <summary>
/// Manages player-related data and upgrades.
/// </summary>
public class PlayerManager : IndestructibleManager
{
    [Header("Stats And Levels")] [NonSerialized]
    public int
        Gold,
        Gems,
        AtkLvl,
        MSLvl,
        JumpLvl,
        DefLvl,
        HpLvl;

    [Header("Power Ups")] [NonSerialized] public int 
        JumpBuffs, 
        SpeedBuffs, 
        SecondChances;

    [Header("Upgrade Prices")] [NonSerialized]
    public int
        AtkPrice,
        MsPrice,
        JumpPrice,
        DefPrice,
        HpPrice;

    [Header("Power Ups Prices")] [NonSerialized]
    public int
        JumpBuffsPrice,
        SpeedBuffsPrice,
        SecondChancesPrice;

    protected override void DoSomethingAtAwakeBeginning() => Load();

    private void Load()
    {
        var player = SaveAndLoadSystem.LoadPlayer();
        
        JumpBuffsPrice = 200;
        SpeedBuffsPrice = 200;
        SecondChancesPrice = 2_000;
        
        if (player == null)
        {
            Initialize();
            return;
        }

        Gold = player.gold;
        Gems = player.gems;

        AtkLvl = player.atkLvl;
        MSLvl = player.msLvl;
        JumpLvl = player.jumpLvl;
        DefLvl = player.defLvl;
        HpLvl = player.hpLvl;

        JumpBuffs = player.jumpBuffs;
        SpeedBuffs = player.speedBuffs;
        SecondChances = player.secondChances;

        SetUpPrice();
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

        AtkLvl = 0;
        MSLvl = 0;
        JumpLvl = 0;
        DefLvl = 0;
        HpLvl = 0;

        JumpBuffs = 3;
        SpeedBuffs = 3;
        SecondChances = 1;

        SetUpPrice();
    }

    private void SetUpPrice()
    {
        AtkPrice = PriceCalculator(AtkLvl);
        MsPrice = PriceCalculator(MSLvl);
        JumpPrice = PriceCalculator(JumpLvl);
        DefPrice = PriceCalculator(DefLvl);
        HpPrice = PriceCalculator(HpLvl);
    }

    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;
}