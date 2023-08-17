using UnityEngine;

public class GameBuilder : MonoBehaviour
{
    public GameObject buildPlacePrefab;
    public ObjectBuilder selectedObject;
    [SerializeField] public GameObject buildingPlacesParent;

    public int columns = 29;
    public int rows = 17;
    public GameObject[,] BuildingPlaces;

    private void Start() =>
        BuildBuildPlaces();

    private void BuildBuildPlaces()
    {
        BuildingPlaces = new GameObject[rows, columns];
        
        for (var i = 0; i < rows; i++)
        for (var j = 0; j < columns; j++)
            BuildingPlaces[i, j] = Instantiate(buildPlacePrefab,
                new Vector3((j - (columns - 1) / 2) * 1.28f, (i - (rows - 1) / 2) * 1.28f, -10), Quaternion.identity,
                buildingPlacesParent.transform);
    }
    
    public void DeselectCurrentItem()
    {
        if (selectedObject == null) return;
        
        selectedObject.Deselect();
    }
}