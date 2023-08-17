using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBuilder : MonoBehaviour
{
    public GameObject buildPlacePrefab;
    public ObjectBuilder selectedObject;
    [SerializeField] private GameObject buildingPlacesParent;

    public int columns = 29;
    public int rows = 17;
    public GameObject[,] BuildingPlaces;

    [SerializeField] private Transform currentButton;
    [SerializeField] private GameObject currentMenu;

    public bool deleteMode;
    
    private void Start() =>
        BuildBuildPlaces();

    public void BackToMenu()=>
        SceneManager.LoadScene("StartMenu");
    
    private void BuildBuildPlaces()
    {
        BuildingPlaces = new GameObject[rows, columns];
        
        for (var i = 0; i < rows; i++)
        for (var j = 0; j < columns; j++)
            BuildingPlaces[i, j] = Instantiate(buildPlacePrefab,
                new Vector3((j - (columns - 1) / 2) * 1.28f, (i - (rows - 1) / 2) * 1.28f, -10), Quaternion.identity,
                buildingPlacesParent.transform);
    }

    public void DeleteModeSwitch()
    {
        deleteMode = !deleteMode;
        ShowBuildPlaces(deleteMode);
    }
    
    public void DeselectCurrentItem()
    {
        deleteMode = false;
        ShowBuildPlaces(false);

        if (selectedObject == null) return;
        selectedObject.transform.GetComponent<Image>().color = Color.white;
        selectedObject = null;
    }

    public void ActivateButton(Transform button)
    {
        if (button == currentButton) return;
        
        currentButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
        currentButton.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
    
        DeselectCurrentItem();

        currentButton = button;
      
        button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        button.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }
    
    public void OpenMenu(GameObject menu)
    {
        if (menu == currentMenu) return;
        
        currentMenu.SetActive(false);

        currentMenu = menu;
        
        menu.SetActive(true);
    }

    public void ShowBuildPlaces(bool show)
    {
        for (var i = 0; i < rows; i++)
        for (var j = 0; j < columns; j++)
            BuildingPlaces[i, j].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, show ? 0.5f : 0f);
    }
}