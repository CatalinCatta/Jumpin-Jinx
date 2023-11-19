using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerSkinEditor : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [NonSerialized] public BodyPartHandler SelectedBodyPart;
    public bool skinEditorEnabled;
    
    public void ActivateSelector()
    {
        line.startColor = line.endColor = Color.green;
        var selectedBodyPartTransform = SelectedBodyPart.transform;
        if (selectedBodyPartTransform.childCount == 0) return;
            
        var selector = selectedBodyPartTransform.GetChild(0);
        selector.gameObject.SetActive(true);
        line.SetPosition(1, selector.position);
        line.sortingOrder = selectedBodyPartTransform.GetComponent<SpriteRenderer>().sortingOrder - 1;
    }

    private void Update()
    {
        if (SelectedBodyPart != null) line.SetPosition(0, SelectedBodyPart.transform.GetComponent<SpriteSkin>().boneTransforms[0].position);
    }

    public void DeactivateSelector()
    {
        line.startColor = line.endColor = Color.clear;
        var selectedBodyPartTransform = SelectedBodyPart.transform;
        if (selectedBodyPartTransform.childCount == 0) return;
        selectedBodyPartTransform.GetChild(0).gameObject.SetActive(false);
    }
}