using UnityEngine;

public class GameBuilder : MonoBehaviour
{
    public ObjectBuilder selectedObject;
    [SerializeField] public GameObject buildingPlacesParent;

    public void DeselectCurrentItem()
    {
        if (selectedObject == null) return;
        
        selectedObject.Deselect();
    }
}