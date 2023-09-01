using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// A static class that provides methods for saving and loading game data.
/// </summary>
public static class SaveAndLoadSystem
{
    #region Paths
    private static readonly BinaryFormatter Formatter = new();
    private static readonly string Path = Application.persistentDataPath;
    
    /// <summary>
    /// <see cref="Application.persistentDataPath"/>/settings.thc
    /// </summary>
    private static readonly string SettingsPath = Path + "/settings.thc";
    
    /// <summary>
    /// <see cref="Application.persistentDataPath"/>/player.thc
    /// </summary>
    private static readonly string PlayerPath = Path + "/player.thc";
    #endregion

    #region Save
    /// <summary>
    /// Saves the settings data to <see cref="SettingsPath"/>.
    /// </summary>
    /// <param name="settingsManager">The settings manager containing the settings data.</param>
    public static void SaveSettings(SettingsManager settingsManager) =>
        Save(new SettingsModel(settingsManager), SettingsPath);

    /// <summary>
    /// Saves the player data to <see cref="PlayerPath"/>.
    /// </summary>
    /// <param name="playerManager">The player manager containing the player data.</param>
    public static void SavePlayer(PlayerManager playerManager) =>
        Save(new PlayerModel(playerManager), PlayerPath);

    private static void Save(object obj, string path)
    {
        var stream = new FileStream(path, FileMode.Create);

        Formatter.Serialize(stream, obj);
        stream.Close();
    }
    #endregion

    #region Load
    /// <summary>
    /// Loads the settings data from <see cref="SettingsPath"/>.
    /// </summary>
    /// <returns>The loaded settings data.</returns>
    public static SettingsModel LoadSettings() =>
        Load(SettingsPath) as SettingsModel;

    /// <summary>
    /// Loads the player data from <see cref="PlayerPath"/>.
    /// </summary>
    /// <returns>The loaded player data.</returns>
    public static PlayerModel LoadPlayer() =>
        Load(PlayerPath) as PlayerModel;

    private static object Load(string path)
    {
        if (!File.Exists(path)) return null;

        var stream = new FileStream(path, FileMode.Open);
        var data = Formatter.Deserialize(stream);

        stream.Close();
        return data;
    }
    #endregion

    #region Reset
    /// <summary>
    /// Deletes the saved settings data.
    /// </summary>
    public static void DeleteSettingsSave() =>
        Reset(PlayerPath);

    /// <summary>
    /// Deletes the saved player data.
    /// </summary>
    public static void DeletePlayerSave() =>
        Reset(PlayerPath);

    private static void Reset(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
    #endregion
}