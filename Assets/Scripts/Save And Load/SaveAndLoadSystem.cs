using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveAndLoadSystem
{
    private static readonly string Path = Application.persistentDataPath;
    private static readonly string SettingsPath = Path + "/settings.set";
    private static readonly string PlayerPath = Path + "/player.play";
    
    private static readonly BinaryFormatter Formatter = new();

    public static void SaveSettings(SettingsManager settingsManager) =>
        Save(new SettingsModel(settingsManager), SettingsPath);

    public static void SavePlayer(PlayerManager playerManager) =>
        Save(new PlayerModel(playerManager), PlayerPath);

    public static SettingsModel LoadSettings() =>
        Load(SettingsPath) as SettingsModel;
    
    public static PlayerModel LoadPlayer() =>
        Load(PlayerPath) as PlayerModel;
    
    private static void Save(object obj, string path)
    {
        var stream = new FileStream(path, FileMode.Create);
        
        Formatter.Serialize(stream, obj);
        stream.Close();   
    }

    private static object Load(string path)
    {
        if (!File.Exists(path)) return null;
        
        var stream = new FileStream(path, FileMode.Open);
        var data = Formatter.Deserialize(stream) as SettingsModel;
        
        stream.Close();
        return data;
    }
}