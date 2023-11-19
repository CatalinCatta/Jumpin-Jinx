using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerSkinEditor : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform character, outline;
    [NonSerialized] public BodyPartHandler SelectedBodyPart;
    [NonSerialized] public bool SkinEditorEnabled;
    private float _characterOriginalPosition, _outlineOriginalPosition;
    private RevenueHandler _revenueHandler;
    private PlayerManager _playerManager;
    
    private void Awake()
    {
        _revenueHandler = GetComponent<RevenueHandler>();
        _playerManager = PlayerManager.Instance;
    }

    private void Start()
    {
        _characterOriginalPosition = character.position.x;
        _outlineOriginalPosition = outline.position.x;
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

    public void DeactivateSelector()
    {
        line.startColor = line.endColor = Color.clear;
        if (SelectedBodyPart == null)return;
        var selectedBodyPartTransform = SelectedBodyPart.transform;
        selectedBodyPartTransform.GetComponent<Collider2D>().enabled = true;
        if (selectedBodyPartTransform.childCount == 0) return;
        selectedBodyPartTransform.GetChild(0).gameObject.SetActive(false);
    }

    public bool TryToPurchaseBodyPart(string bodyPart, Skin skin)
    {
        if (!_revenueHandler.TryToConsumeGold(Dictionaries.Skin[skin].price)) return false;
        
        var skinName = Dictionaries.Skin[skin].name;

        if (_playerManager.Skins.ContainsKey(skinName)) _playerManager.Skins[skinName].Add(bodyPart);
        else _playerManager.Skins.Add(skinName, new List<string> { bodyPart });
                
        return true;
    }

    public void SaveSkinPreference()
    {
        foreach (Transform bodyPart in outline)
            if (bodyPart.childCount > 0 &&
                bodyPart.GetChild(0).TryGetComponent<BodyPartSelector>(out var bodyPartSelector))
                bodyPartSelector.ChangeToLastPurchasedSkin();
    }
    
    private void MoveCharacterInMiddle() => StartCoroutine(MoveSmoothly(true));
    
    private void MoveCharacterToOriginalPosition() => StartCoroutine(MoveSmoothly(false));

    private void FastCloseSkinMenu() => SwitchSkinMenu(false);

    private IEnumerator MoveSmoothly(bool centerIt)
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

        SwitchSkinMenu(centerIt);
    }

    private void SwitchSkinMenu(bool activate)
    {
        var startCharacterPosition = character.position;
        var startOutlinePosition = outline.position;
        
        character.position = new Vector3(activate ? 0 : _characterOriginalPosition, startCharacterPosition.y,
            startCharacterPosition.z);
        outline.position = new Vector3(activate ? 0 : _outlineOriginalPosition, startOutlinePosition.y,
            startOutlinePosition.z);
        SkinEditorEnabled = activate;
        
        DeactivateSelector();
    }
    
    private void Update()
    {
        if (SelectedBodyPart != null)
            line.SetPosition(0,
                SelectedBodyPart.transform.GetComponent<SpriteSkin>().boneTransforms[0].position +
                Vector3.up * (SelectedBodyPart.name == "Armor" ? 1.5f : 0));
    }
}