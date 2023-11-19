using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerSkinEditor : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform character, outline;
    [NonSerialized] public BodyPartHandler SelectedBodyPart;
    [NonSerialized] public bool SkinEditorEnabled;
    private float _characterOriginalPosition, _outlineOriginalPosition;

    private void Start()
    {
        _characterOriginalPosition= character.position.x;
        _outlineOriginalPosition = outline.position.x;
    }

    private void MoveCharacterInMiddle() => StartCoroutine(MoveSmoothly(true));
    
    private void MoveCharacterToOriginalPosition() => StartCoroutine(MoveSmoothly(false));

    private System.Collections.IEnumerator MoveSmoothly(bool centerIt)
    {
        var startCharacterPosition = character.position;
        var startOutlinePosition = outline.position;
        var elapsedTime = 0f;

        while (elapsedTime < .5f)
        {
            character.position = Vector3.Lerp(startCharacterPosition,
                new Vector3(centerIt ? 0 : _characterOriginalPosition, startCharacterPosition.y,
                    startCharacterPosition.z), elapsedTime / .5f);
            outline.position = Vector3.Lerp(startOutlinePosition,
                new Vector3(centerIt ? 0 : _outlineOriginalPosition, startOutlinePosition.y, startOutlinePosition.z),
                elapsedTime / .5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        character.position = new Vector3(centerIt ? 0 : _characterOriginalPosition, startCharacterPosition.y,
            startCharacterPosition.z);
        outline.position = new Vector3(centerIt ? 0 : _outlineOriginalPosition, startOutlinePosition.y,
            startOutlinePosition.z);
        
        SkinEditorEnabled = centerIt;
        DeactivateSelector();
    }
    
    public void ActivateSelector()
    {
        line.startColor = line.endColor = Color.green;
        
        var selectedBodyPartTransform = SelectedBodyPart.transform;
        selectedBodyPartTransform.GetComponent<Collider2D>().enabled = false;
        
        var selector = selectedBodyPartTransform.GetChild(0);
        selector.gameObject.SetActive(true);

        line.SetPosition(1, selector.position);
        line.sortingOrder = selectedBodyPartTransform.GetComponent<SpriteRenderer>().sortingOrder;
    }

    private void Update()
    {
        if (SelectedBodyPart != null)
            line.SetPosition(0,
                SelectedBodyPart.transform.GetComponent<SpriteSkin>().boneTransforms[0].position +
                Vector3.up * (SelectedBodyPart.name == "Armor" ? 1.5f : 0));
    }

    public void DeactivateSelector()
    {
        line.startColor = line.endColor = Color.clear;
        if (SelectedBodyPart == null)return;
        var selectedBodyPartTransform = SelectedBodyPart.transform;
        selectedBodyPartTransform.GetComponent<Collider2D>().enabled = true;
        if (selectedBodyPartTransform.childCount == 0) return;
        selectedBodyPartTransform.GetChild(0).gameObject.SetActive(false);
    }
}