using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class GameBuildSave : MonoBehaviour
{
    private GameBuilder _gameBuilder;
    private string _fileName;
    
    private void Awake() =>
        _gameBuilder = GetComponent<GameBuilder>();

    public void ChangeFileName(string newFileName) =>
        _fileName = newFileName;
    
    public void SaveMap()
    {
        try
        {
            var path = Path.GetFullPath(@"CustomLevels");
            var finalPath = path + $"/{LvlManager.LvlTitle}.json";
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
                LvlManager.LvlTitle = _fileName;
                finalPath = path + $"/{LvlManager.LvlTitle}.json";
            }
            
            File.WriteAllText(finalPath, JsonConvert.SerializeObject(new LevelConfigurations
            {
                Levels = new[] { MapObjects() }
            }));
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
        
        for (var i = 0; i < _gameBuilder.rows; i++)
        {
            var str = new StringBuilder();
            for (var j = 0; j < _gameBuilder.columns; j++)
            {
                switch (_gameBuilder.BuildingPlaces[i, j].GetComponent<BuildingPlace>().block)
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
                switch (_gameBuilder.BuildingPlaces[i, j].GetComponent<BuildingPlace>().@object)
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
    
    private string MapEnveiroments()
    {
        return "";
    }
}
