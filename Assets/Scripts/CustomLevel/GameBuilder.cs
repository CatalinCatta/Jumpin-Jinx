using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBuilder : MonoBehaviour
{
    [Header("Size")] [NonSerialized] public int Columns, Rows;
    [Header("Editors")] [NonSerialized] public bool DeleteMode, MoveMode;

    [Header("Object Builder")] [NonSerialized]
    public ObjectBuilder SelectedObject;
    public Transform currentButton;
    public Sprite selectedButton, deselectedButton;
    public TextMeshProUGUI playerCounter, endLvlCounter;

    [Header("Building Place")] [SerializeField]
    private GameObject buildPlacePrefab;

    [SerializeField] private Transform buildingPlacesParent;

    [Header("Utilities")]
    [SerializeField] private GameObject currentMenu;
    [NonSerialized] public GameObject[,] BuildingPlaces;
    [NonSerialized] public (int x, int y) PlayerPosition = (-1, -1), EndLvlPosition = (-1, -1); 
    
    private void Start() => BuildBuildPlaces();

    /// <summary>
    /// Returns to the main menu scene.
    /// </summary>
    public void BackToMenu() => SceneManager.LoadScene("StartMenu");

    private void BuildBuildPlaces()
    {
        Columns = 29;
        Rows = 17;
        
        BuildingPlaces = new GameObject[Rows, Columns];
        var lvlManager = LvlManager.Instance;

        var path = Path.Join(Path.GetFullPath(@"CustomLevels"), lvlManager.LvlTitle);
        var prebuilt = Directory.Exists(path);
        var map = new string[Rows];
        if (prebuilt)
        {
            var finalPath = Path.Join(path, lvlManager.LvlTitle + ".json");
            prebuilt = File.Exists(finalPath); 
            if (prebuilt) map = JsonConvert.DeserializeObject<Level>(File.ReadAllText(finalPath)).Maps;
        }
        
        for (var i = 0; i < Rows ; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                var buildPlace = BuildingPlaces[i, j] = Instantiate(buildPlacePrefab,
                    new Vector3((j - (Columns - 1) / 2) * 1.28f, (i - (Rows - 1) / 2) * 1.28f, -10),
                    Quaternion.identity, buildingPlacesParent);
                buildPlace.GetComponent<BuildingPlace>().PositionInArray = (i, j);
                if (!prebuilt) continue;
                
                var objectToBuild= Dictionaries.ObjectBuild.FirstOrDefault(kv => kv.Value.character == map[Rows-i-1][j]);

                switch (objectToBuild.Value.category)
                {
                    case ObjectBuildCategory.Block:
                        buildPlace.GetComponent<BuildingPlace>().block = objectToBuild.Key;
                        break;
                    
                    case ObjectBuildCategory.Plant:
                        buildPlace.GetComponent<BuildingPlace>().environment = objectToBuild.Key;
                        break;

                    default:
                        buildPlace.GetComponent<BuildingPlace>().objectType = objectToBuild.Key;
                        break;
                }

                var spriteTransform = buildPlace.transform.GetChild(0).GetChild(objectToBuild.Value.category switch
                {
                    ObjectBuildCategory.Block => 0,
                    ObjectBuildCategory.Plant => 1,
                    _ => 2
                });
                spriteTransform.GetComponent<SpriteRenderer>().sprite = objectToBuild.Value.sprite;
                spriteTransform.rotation = objectToBuild.Value.rotation;
            }
        }

        buildingPlacesParent.localScale = new Vector3(.8f, .8f, 1f);
        buildingPlacesParent.position = new Vector3(4f, -2f, -1f);
    }

    /// <summary>
    /// Toggles the delete mode for clearing placed objects.
    /// </summary>
    public void DeleteModeSwitch()
    {
        DeleteMode = !DeleteMode;
        ShowBuildPlaces(DeleteMode);
    }

    /// <summary>
    /// Deselects the currently selected object and hides building places.
    /// </summary>
    public void DeselectCurrentItem()
    {
        DeleteMode = false;
        ShowBuildPlaces(false);

        if (SelectedObject == null) return;
        SelectedObject.transform.GetComponent<Image>().color = Color.white;
        SelectedObject = null;
    }

    /// <summary>
    /// Activates the specified button and deactivates the previously active one.
    /// </summary>
    public void ActivateButton(Transform button)
    {
        if (button == currentButton) return;

        currentButton.GetComponent<Image>().sprite = deselectedButton;
        currentButton.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f, 1f);

        DeselectCurrentItem();

        currentButton = button;

        button.GetComponent<Image>().sprite = selectedButton;
        button.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    /// <summary>
    /// Opens the specified menu and closes the previously open one.
    /// </summary>
    public void OpenMenu(GameObject menu)
    {
        if (menu == currentMenu) return;

        currentMenu.SetActive(false);

        currentMenu = menu;

        menu.SetActive(true);
    }

    /// <summary>
    /// Shows or hides building places on the grid.
    /// </summary>
    public void ShowBuildPlaces(bool show)
    {
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Columns; j++)
            BuildingPlaces[i, j].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, show ? .5f : 0f);
    }
}