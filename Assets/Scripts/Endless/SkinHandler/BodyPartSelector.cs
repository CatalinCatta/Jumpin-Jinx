using UnityEngine;
using System;
using UnityEngine.U2D.Animation;

public class BodyPartSelector : MonoBehaviour
{
   private Skin _currentSkin = Skin.Classic;
   private Transform _transform;
   private string _category;

   private void Start()
   {
      _transform = transform;
      _category = _transform.parent.GetComponent<SpriteResolver>().GetCategory();
   }

   public void PrevSkin()
   {
      _currentSkin = GetPrevSkin(_currentSkin);
      ChangeSkin();
      ChangeAllSkinSelectors();
   }

   public void NextSkin()
   {
      _currentSkin = GetNextSkin(_currentSkin);
      ChangeSkin();
      ChangeAllSkinSelectors();
   }

   private void ChangeSkin()
   {
      var transformParent =_transform.parent; 
      transformParent.GetComponent<SpriteResolver>().SetCategoryAndLabel(_category, Dictionaries.Skin[_currentSkin].name);
      transformParent.parent.parent.GetChild(0).GetChild(transformParent.GetSiblingIndex()).GetComponent<SpriteResolver>().SetCategoryAndLabel(_category, Dictionaries.Skin[_currentSkin].name);
   }
   
   private void ChangeAllSkinSelectors()
   {
      ChangeSkinSelector(0, GetPrevSkin(GetPrevSkin(_currentSkin)));
      ChangeSkinSelector(1, GetPrevSkin(_currentSkin));
      ChangeSkinSelector(2, _currentSkin);
      ChangeSkinSelector(3, GetNextSkin(_currentSkin));
      ChangeSkinSelector(4, GetNextSkin(GetNextSkin(_currentSkin)));
   }
   
   private void ChangeSkinSelector(int skinNr, Skin newSkin) =>
      _transform.GetChild(0).GetChild(skinNr).GetComponent<SpriteRenderer>().sprite = _transform.parent.parent
         .GetComponent<SpriteLibrary>().GetSprite(_category, Dictionaries.Skin[newSkin].name);

   private static Skin GetPrevSkin(Skin currentSkin) =>
      currentSkin == 0 ? (Skin)Enum.GetValues(typeof(Skin)).Length - 1 : currentSkin - 1;

   private static Skin GetNextSkin(Skin currentSkin) =>
      currentSkin == (Skin)Enum.GetValues(typeof(Skin)).Length - 1 ? 0 : currentSkin + 1;
}