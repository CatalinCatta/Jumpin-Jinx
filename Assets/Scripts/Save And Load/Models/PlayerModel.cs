using System;

[Serializable]
public class PlayerModel
{
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

    public PlayerModel(PlayerManager playerManager)
    {
        gold = playerManager.gold;
        gems = playerManager.gems;
        
        atkLvl = playerManager.atkLvl;
        msLvl = playerManager.msLvl;
        jumpLvl = playerManager.jumpLvl;
        defLvl = playerManager.defLvl;
        hpLvl = playerManager.hpLvl;
        
        jumpBuffs = playerManager.jumpBuffs;
        speedBuffs = playerManager.speedBuffs;
        secondChances = playerManager.secondChances;
    }
}