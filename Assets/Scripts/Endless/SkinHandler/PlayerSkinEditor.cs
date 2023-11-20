using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerSkinEditor : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform skinCounter, character, outline;
    [NonSerialized] public BodyPartHandler SelectedBodyPart;
    [NonSerialized] public bool SkinEditorEnabled;
    private float _characterOriginalPosition;
    private RevenueHandler _revenueHandler;
    private PlayerManager _playerManager;
    private Dictionary<Skin, (int owned, int total)> _skinPartsCounter;

    private void Awake()
    {
        _revenueHandler = GetComponent<RevenueHandler>();
        _playerManager = PlayerManager.Instance;
        _skinPartsCounter = new Dictionary<Skin, (int, int)>();
        for (var i = 0; i < Enum.GetValues(typeof(Skin)).Length; i++) _skinPartsCounter.Add((Skin)i, (0, 0));
    }

    private void Start() => _characterOriginalPosition = character.position.x;

    public void CountSkins(Skin skin, int value, bool ownTheSkin)
    {
        _skinPartsCounter[skin] = (_skinPartsCounter[skin].owned + (ownTheSkin ? value : 0),
            _skinPartsCounter[skin].total + value);

        var count = _skinPartsCounter.Count(item => item.Value.total > 0);
        var orderedList = _skinPartsCounter.ToList();
        orderedList.Sort((pair1, pair2) => pair2.Value.total.CompareTo(pair1.Value.total));

        var childNr = skinCounter.childCount;
        for (var i = 0; i < childNr; i++)
        {
            var isUsingTheText = childNr - i <= count;
            skinCounter.GetChild(i).gameObject.SetActive(isUsingTheText);
            if (!isUsingTheText) continue;

            var positionInOrderedList = i - Mathf.Max(childNr - count, 0);
            skinCounter.GetChild(i).GetComponent<TextMeshProUGUI>().text =
                $"{Dictionaries.Skin[orderedList[positionInOrderedList].Key].name}: {Utility.FormatDoubleWithUnits(orderedList[positionInOrderedList].Value.total / .19, true)}% ({orderedList[positionInOrderedList].Value.owned}/{orderedList[positionInOrderedList].Value.total})";
        }

        if (childNr >= count) return;

        int owned = 0, total = 0;
        for (var i = 0; i <= count - childNr; i++)
        {
            owned += orderedList[count - i - 1].Value.owned;
            total += orderedList[count - i - 1].Value.total;
        }
            
        skinCounter.GetChild(childNr - 1).GetComponent<TextMeshProUGUI>().text =
            $"Other: {Utility.FormatDoubleWithUnits(total / .19, true)}% ({owned}/{total})";
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

    private void FastCloseSkinMenu() 
    {
        DeactivateSelector();
        SwitchSkinMenu(false);
    }
    
    private IEnumerator MoveSmoothly(bool centerIt)
    {
        var startCharacterPosition = character.position;
        var elapsedTime = 0f;
        DeactivateSelector();

        while (elapsedTime < .5f)
        {
            character.position = Vector3.Lerp(startCharacterPosition,
                new Vector3(centerIt ? 0 : _characterOriginalPosition, startCharacterPosition.y,
                    startCharacterPosition.z), elapsedTime / .5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SwitchSkinMenu(centerIt);
    }

    private void SwitchSkinMenu(bool activate)
    {
        var startCharacterPosition = character.position;
        
        character.position = new Vector3(activate ? 0 : _characterOriginalPosition, startCharacterPosition.y,
            startCharacterPosition.z);
        SkinEditorEnabled = activate;
    }
    
    private void Update()
    {
        if (SelectedBodyPart != null)
            line.SetPosition(0,
                SelectedBodyPart.transform.GetComponent<SpriteSkin>().boneTransforms[0].position +
                Vector3.up * (SelectedBodyPart.name == "Armor" ? 1.5f : 0));
    }
}