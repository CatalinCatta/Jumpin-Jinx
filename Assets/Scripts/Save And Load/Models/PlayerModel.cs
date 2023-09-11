using System;
using System.Collections.Generic;

/// <summary>
/// Represents the model for a player in a game.
/// </summary>
[Serializable]
public class PlayerModel
{
    public int gold, gems;
    public Enhancement[] Upgrades, Buffs;

    /// <summary>
    /// Initializes a new instance of the PlayerModel class.
    /// </summary>
    /// <param name="playerManager">The PlayerManager object containing the player data.</param>
    public PlayerModel(PlayerManager playerManager)
    {
        gold = playerManager.Gold;
        gems = playerManager.Gems;

        Upgrades = playerManager.Upgrades;
        Buffs = playerManager.Buffs;
    }
}