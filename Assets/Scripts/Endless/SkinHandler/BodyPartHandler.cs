using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BodyPartHandler : MonoBehaviour 
{
    private SpriteRenderer _sprite;
    [SerializeField] private PlayerSkinEditor skinEditor;
    
    private void Start()
    {
        _sprite = transform.GetComponent<SpriteRenderer>();
        if (skinEditor == null) skinEditor = FindObjectOfType<PlayerSkinEditor>();
    }

    private void OnMouseOver()
    {
        if (!skinEditor.skinEditorEnabled || skinEditor.SelectedBodyPart == this) return;
       
        _sprite.color = Color.yellow;
        if (!Input.GetMouseButton(0)) return;
        
        _sprite.color = Color.green;
        if (skinEditor.SelectedBodyPart != null)
        {
            skinEditor.SelectedBodyPart._sprite.color = Color.clear;
            skinEditor.DeactivateSelector();
        }
        
        skinEditor.SelectedBodyPart = this;
        skinEditor.ActivateSelector();
    }

    private void OnMouseExit()
    {
        if (skinEditor.SelectedBodyPart != this) _sprite.color = Color.clear;
    }
}