using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the saving of custom game maps and configurations.
/// </summary>
[RequireComponent(typeof(GameBuilder))]
public class GameBuildSave : MonoBehaviour
{
    private GameBuilder _gameBuilder;
    private LvlManager _lvlManager;
    [SerializeField] private TMP_InputField title;
    
    private void Awake() => _gameBuilder = GetComponent<GameBuilder>();

    private void Start()
    {
        _lvlManager = LvlManager.Instance;
        title.text = _lvlManager.LvlTitle;
    }

    /// <summary>
    /// Changes the filename used for saving.
    /// </summary>
    public void ChangeFileName(string newFileName) => title.text = title.text == _lvlManager.LvlTitle
        ? title.text
        : _lvlManager.LvlTitle = Utility.ReturnFirstPossibleName(title.text,
            Directory.GetDirectories(Path.GetFullPath(@"CustomLevels")).ToList());

    public void StartGame() => StartCoroutine(StartGameCoroutine());

    private IEnumerator StartGameCoroutine()
    {
        SaveMap();
        yield return 0;
        _lvlManager.LvlTitle = title.text;
        _lvlManager.IsCampaign = false;
        _lvlManager.StartScene(1);
    }

    /// <summary>
    /// Saves the current map configuration to a JSON file.
    /// </summary>
    public void SaveMap()
    {
        try
        {
            var path = Path.Join(Path.GetFullPath(@"CustomLevels"), _lvlManager.LvlTitle);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var finalPath = Path.Join(path, $"{_lvlManager.LvlTitle}.json");
            if (File.Exists(finalPath)) File.Delete(finalPath);
            File.WriteAllText(finalPath, JsonConvert.SerializeObject(MapObjects()));
            
            var screenshotPath = Path.Join(path, "Screenshot.jpg");
            if (File.Exists(screenshotPath)) File.Delete(screenshotPath);
            ScreenCapture.CaptureScreenshot(screenshotPath,2);
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

        for (var i = _gameBuilder.Rows - 1; i > -1; i--)
        {
            var str = new StringBuilder();
            for (var j = 0; j < _gameBuilder.Columns; j++)
            {
                var element = _gameBuilder.BuildingPlaces[i, j].GetComponent<BuildingPlace>().block;
                if (element == ObjectBuildType.Null)
                    element = _gameBuilder.BuildingPlaces[i, j].GetComponent<BuildingPlace>().objectType;

                str.Append(element != ObjectBuildType.Null ? Dictionaries.ObjectBuild[element].character : ' ');
            }

            line.Add(str.ToString());
        }

        lvl.Maps = line.ToArray();
        return lvl;
    }
}