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

    public bool TryToPurchaseSkin(string bodyPart, Skin skin) => skinEditor.TryToPurchaseBodyPart(bodyPart, skin);

    private void Update()
    {
        if (!skinEditor.SkinEditorEnabled) skinEditor.SelectedBodyPart._sprite.color = Color.clear;
    }

    private void OnMouseOver()
    {
        if (skinEditor.SkinEditorEnabled && skinEditor.SelectedBodyPart != this) _sprite.color = Color.yellow;
    }

    private void OnMouseDown()
    {
        if (!skinEditor.SkinEditorEnabled) return;
  
        _sprite.color = Color.green;
        if (skinEditor.SelectedBodyPart != null) skinEditor.SelectedBodyPart._sprite.color = Color.clear;

        skinEditor.DeactivateSelector();
        skinEditor.SelectedBodyPart = this;
        skinEditor.ActivateSelector();
    }

    private void OnMouseExit()
    {
        if (skinEditor.SelectedBodyPart != this) _sprite.color = Color.clear;
    }
}