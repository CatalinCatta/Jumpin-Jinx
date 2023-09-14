using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Manages the saving of custom game maps and configurations.
/// </summary>
[RequireComponent(typeof(GameBuilder))]
public class GameBuildSave : MonoBehaviour
{
    private string _fileName;
    private GameBuilder _gameBuilder;

    private void Awake() => _gameBuilder = GetComponent<GameBuilder>();

    /// <summary>
    /// Changes the filename used for saving.
    /// </summary>
    public void ChangeFileName(string newFileName) => _fileName = newFileName;

    /// <summary>
    /// Saves the current map configuration to a JSON file.
    /// </summary>
    public void SaveMap()
    {
        var lvlManager = LvlManager.Instance;
        
        try
        {
            var path = Path.GetFullPath(@"CustomLevels");
            var finalPath = path + $"/{lvlManager.LvlTitle}.json";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
                lvlManager.LvlTitle = _fileName;
                finalPath = path + $"/{lvlManager.LvlTitle}.json";
            }

            File.WriteAllText(finalPath, JsonConvert.SerializeObject(new[] { MapObjects() }));
        }
        catch (IOException ex)
        {
            Debug.Log("Error writing the file: " + ex.Message);
        }
        catch (JsonException ex)
        {
            Debug.Log("Error serializing JSON: " + ex.Message);
        }
    }

    private Level MapObjects()
    {
        var lvl = new Level();
        var line = new List<string>();

        for (var i = 0; i < _gameBuilder.Rows; i++)
        {
            var str = new StringBuilder();
            for (var j = 0; j < _gameBuilder.Columns; j++)
            {
                switch (_gameBuilder.BuildingPlaces[i, j].GetComponent<BuildingPlace>().Block)
                {
                    case ObjectBuildType.Null:
                        break;
                    case ObjectBuildType.Dirt:
                        str.Append('D');
                        continue;
                    case ObjectBuildType.StaticGrass:
                        str.Append('G');
                        continue;
                    case ObjectBuildType.HalfSlopeDirt:
                        str.Append('}');
                        continue;
                    case ObjectBuildType.HalfSlopeDirtRotated:
                        str.Append('{');
                        continue;
                    case ObjectBuildType.SlopeDirt:
                        str.Append('>');
                        continue;
                    case ObjectBuildType.SlopeDirtRotated:
                        str.Append('<');
                        continue;
                }

                switch (_gameBuilder.BuildingPlaces[i, j].GetComponent<BuildingPlace>().Object)
                {
                    case ObjectBuildType.Null:
                        break;
                    case ObjectBuildType.Player:
                        str.Append('P');
                        continue;
                    case ObjectBuildType.EndLvl:
                        str.Append('X');
                        continue;
                    case ObjectBuildType.Coin:
                        str.Append('C');
                        continue;
                    case ObjectBuildType.Health:
                        str.Append('H');
                        continue;
                    case ObjectBuildType.Enemy:
                        str.Append('E');
                        continue;
                }

                str.Append(' ');
            }

            line.Add(str.ToString());
        }

        lvl.Map = line.ToArray();

        return lvl;
    }
}