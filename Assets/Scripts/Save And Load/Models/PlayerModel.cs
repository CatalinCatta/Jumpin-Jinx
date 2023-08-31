using System;

/// <summary>
/// Represents the model for a player in a game.
/// </summary>
[Serializable]
public class PlayerModel
{
    // Player revenues
    public int gold;
    public int gems;

    // Player upgrade levels
    public int atkLvl;
    public int msLvl;
    public int jumpLvl;
    public int defLvl;
    public int hpLvl;

    // Player buffs
    public int jumpBuffs;
    public int speedBuffs;
    public int secondChances;

    /// <summary>
    /// Initializes a new instance of the PlayerModel class.
    /// </summary>
    /// <param name="playerManager">The PlayerManager object containing the player data.</param>
    public PlayerModel(PlayerManager playerManager)
    {
        gold = playerManager.Gold;
        gems = playerManager.Gems;

        atkLvl = playerManager.AtkLvl;
        msLvl = playerManager.MSLvl;
        jumpLvl = playerManager.JumpLvl;
        defLvl = playerManager.DefLvl;
        hpLvl = playerManager.HpLvl;

        jumpBuffs = playerManager.JumpBuffs;
        speedBuffs = playerManager.SpeedBuffs;
        secondChances = playerManager.SecondChances;
    }
}