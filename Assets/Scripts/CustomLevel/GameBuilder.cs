using System;
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
    public Transform _currentButton;
    public Sprite selectedButton, deselectedButton;
    public TextMeshProUGUI playerCounter, endLvlCounter;
    
    [Header("Building Place")] [SerializeField]
    private GameObject buildPlacePrefab, buildingPlacesParent;

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

        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Columns; j++)
            BuildingPlaces[i, j] = Instantiate(buildPlacePrefab,
                new Vector3((j - (Columns - 1) / 2) * 1.28f, (i - (Rows - 1) / 2) * 1.28f, -10), Quaternion.identity,
                buildingPlacesParent.transform);
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
        if (button == _currentButton) return;

        _currentButton.GetComponent<Image>().sprite = deselectedButton;
        _currentButton.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f, 1f);

        DeselectCurrentItem();

        _currentButton = button;

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