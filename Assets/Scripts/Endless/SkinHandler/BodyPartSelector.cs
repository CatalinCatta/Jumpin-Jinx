using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.U2D.Animation;

public class BodyPartSelector : MonoBehaviour
{
   private string _category;
   private Skin _currentSkin, _lastUsableSkin;
   private Transform _transform, _parent;
   private PlayerManager _playerManager;
   private BodyPartHandler _bodyPartHandler;

   private void Start()
   {
      _playerManager = PlayerManager.Instance;
      _transform = transform;
      _parent = _transform.parent;
      _category = _parent.GetComponent<SpriteResolver>().GetCategory();
      _bodyPartHandler = _parent.GetComponent<BodyPartHandler>();
      ChangeSkin(_lastUsableSkin = _currentSkin = Dictionaries.Skin
         .FirstOrDefault(kv => kv.Value.name == _playerManager.CurrentSkin[_category]).Key);
      gameObject.SetActive(false);
   }

   public void PrevSkin() => ChangeSkin(GetPrevSkin(_currentSkin));

   public void NextSkin() => ChangeSkin(GetNextSkin(_currentSkin));

   public void Buy()
   {
      if (!_bodyPartHandler.TryToPurchaseSkin(_category, _currentSkin)) return;
      
      HandlePaymentStatus();
      _bodyPartHandler.ChangeSkinCounter(_currentSkin, -1, false);
      _bodyPartHandler.ChangeSkinCounter(_currentSkin, +1, true);
   }

   public void ChangeToLastPurchasedSkin()
   {
      _playerManager.CurrentSkin[_category] = Dictionaries.Skin[_lastUsableSkin].name;
      if (HasBoughtTheSkin()) return;

      var selectedSkin = _lastUsableSkin;
      while (_currentSkin != selectedSkin) NextSkin();
   }

   private void ChangeSkin(Skin newSkin)
   {
      SetUpCounter(newSkin);
      
      if (HasBoughtTheSkin()) _lastUsableSkin = _currentSkin;

      _parent.GetComponent<SpriteResolver>()
         .SetCategoryAndLabel(_category, Dictionaries.Skin[_currentSkin].name);
      _parent.parent.parent.GetChild(0).GetChild(_parent.GetSiblingIndex())
         .GetComponent<SpriteResolver>().SetCategoryAndLabel(_category, Dictionaries.Skin[_currentSkin].name);

      HandlePaymentStatus();
      ChangeAllSkinSelectors();
   }

   private void SetUpCounter(Skin newSkin)
   {
      if (newSkin != _currentSkin)
      {
         _bodyPartHandler.ChangeSkinCounter(_currentSkin, -1, HasBoughtTheSkin());
         _currentSkin = newSkin;
      }
      _bodyPartHandler.ChangeSkinCounter(_currentSkin, +1, HasBoughtTheSkin());
   }
   
   private void HandlePaymentStatus()
   {
      var payButton = _transform.GetChild(1);
      var usableSkin = HasBoughtTheSkin();
      payButton.gameObject.SetActive(!usableSkin);
      
      var selectedIcon = _transform.GetChild(5);
      selectedIcon.GetComponent<SpriteRenderer>().color =
         selectedIcon.GetChild(1).GetComponent<SpriteRenderer>().color = usableSkin ? Color.green : Color.red;

      if (!usableSkin)
         payButton.GetChild(0).GetComponent<TextMeshPro>().text = Utility.FormatDoubleWithUnits(Dictionaries.Skin[_currentSkin].price, false);
   }

   private void ChangeAllSkinSelectors()
   {
      ChangeSkinSelector(0, GetPrevSkin(GetPrevSkin(_currentSkin)));
      ChangeSkinSelector(1, GetPrevSkin(_currentSkin));
      ChangeSkinSelector(2, _currentSkin);
      ChangeSkinSelector(3, GetNextSkin(_currentSkin));
      ChangeSkinSelector(4, GetNextSkin(GetNextSkin(_currentSkin)));
   }

   private bool HasBoughtTheSkin() => _playerManager.Skins.ContainsKey(Dictionaries.Skin[_currentSkin].name) &&
                                      _playerManager.Skins[Dictionaries.Skin[_currentSkin].name].Contains(_category);
   
   private void ChangeSkinSelector(int skinNr, Skin newSkin) =>
      _transform.GetChild(0).GetChild(skinNr).GetComponent<SpriteRenderer>().sprite = _parent.parent
         .GetComponent<SpriteLibrary>().GetSprite(_category, Dictionaries.Skin[newSkin].name);

   private static Skin GetPrevSkin(Skin currentSkin) =>
      currentSkin == 0 ? (Skin)Enum.GetValues(typeof(Skin)).Length - 1 : currentSkin - 1;

   private static Skin GetNextSkin(Skin currentSkin) =>
      currentSkin == (Skin)Enum.GetValues(typeof(Skin)).Length - 1 ? 0 : currentSkin + 1;
}