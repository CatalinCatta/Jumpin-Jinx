using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.U2D.Animation;

public class BodyPartSelector : MonoBehaviour
{
   private Skin _currentSkin;
   private Transform _transform;
   private string _category;
   private PlayerManager _playerManager;
   private Skin _lastUsableSkin;

   private void Start()
   {
      _playerManager = PlayerManager.Instance;
      _transform = transform;
      _category = _transform.parent.GetComponent<SpriteResolver>().GetCategory();
      _lastUsableSkin = _currentSkin = Dictionaries.Skin
         .FirstOrDefault(kv => kv.Value.name == _playerManager.CurrentSkin[_category]).Key;
      ChangeSkin();
      gameObject.SetActive(false);
   }

   public void PrevSkin()
   {
      _currentSkin = GetPrevSkin(_currentSkin);
      ChangeSkin();
   }

   public void NextSkin()
   {
      _currentSkin = GetNextSkin(_currentSkin);
      ChangeSkin();
   }

   public void Buy()
   {
      if (_transform.parent.GetComponent<BodyPartHandler>().TryToPurchaseSkin(_category, _currentSkin))
         HandlePaymentStatus();
   }

   public void ChangeToLastPurchasedSkin()
   {
      _playerManager.CurrentSkin[_category] = Dictionaries.Skin[_lastUsableSkin].name;
      if (HasBoughtTheSkin()) return;

      var selectedSkin = _lastUsableSkin;
      while (_currentSkin != selectedSkin) NextSkin();
   }

   private void ChangeSkin()
   {
      if (HasBoughtTheSkin()) _lastUsableSkin = _currentSkin;

      var transformParent = _transform.parent;
      transformParent.GetComponent<SpriteResolver>()
         .SetCategoryAndLabel(_category, Dictionaries.Skin[_currentSkin].name);
      transformParent.parent.parent.GetChild(0).GetChild(transformParent.GetSiblingIndex())
         .GetComponent<SpriteResolver>().SetCategoryAndLabel(_category, Dictionaries.Skin[_currentSkin].name);

      HandlePaymentStatus();
      ChangeAllSkinSelectors();
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
      _transform.GetChild(0).GetChild(skinNr).GetComponent<SpriteRenderer>().sprite = _transform.parent.parent
         .GetComponent<SpriteLibrary>().GetSprite(_category, Dictionaries.Skin[newSkin].name);

   private static Skin GetPrevSkin(Skin currentSkin) =>
      currentSkin == 0 ? (Skin)Enum.GetValues(typeof(Skin)).Length - 1 : currentSkin - 1;

   private static Skin GetNextSkin(Skin currentSkin) =>
      currentSkin == (Skin)Enum.GetValues(typeof(Skin)).Length - 1 ? 0 : currentSkin + 1;
}