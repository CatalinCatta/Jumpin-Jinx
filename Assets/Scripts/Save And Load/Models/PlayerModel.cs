using System;
using System.Collections.Generic;

/// <summary>
/// Represents the model for a player in a game.
/// </summary>
[Serializable]
public class PlayerModel
{
    public int gold, gems;
    public Enhancement[] upgrades, buffs;
    public Dictionary<string, List<string>> Skins;
    public Dictionary<string, string> CurrentSkin;

    /// <summary>
    /// Initializes a new instance of the PlayerModel class.
    /// </summary>
    /// <param name="playerManager">The PlayerManager object containing the player data.</param>
    public PlayerModel(PlayerManager playerManager)
    {
        gold = playerManager.Gold;
        gems = playerManager.Gems;

        upgrades = playerManager.Upgrades;
        buffs = playerManager.Buffs;

        Skins = playerManager.Skins;
        CurrentSkin = playerManager.CurrentSkin;
    }
}