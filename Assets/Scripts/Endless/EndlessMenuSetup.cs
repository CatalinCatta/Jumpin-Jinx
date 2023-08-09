using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;
using Random = UnityEngine.Random;

public class EndlessMenuSetup : MonoBehaviour
{
    [SerializeField] private Transform menu;

    private PlayerEndlessStatus _playerEndlessStatus;

    private void Awake() =>
        _playerEndlessStatus = GetComponent<PlayerEndlessStatus>();

    private enum Direction
    {
        Left = -1500,
        Right = 1500,
        Random = 2000
    }
    
    public void Enable() =>
        StartCoroutine(ActivateMenu());

    public void Disable() =>
        StartCoroutine(CloseMenu());
    
    public void ShowUpgrades()
    {
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType)))
            SetUpUpgrades(upgradeType);

        StartCoroutine(AnimateCard(transform.GetChild(2), true));
    }

    public void HideUpgrades() =>
        StartCoroutine(AnimateCard(transform.GetChild(2), false));

    public void ShowShop()
    {
        StartCoroutine(AnimateCard(transform.GetChild(3), true));
      
        foreach (ShopItemType shopItemType in Enum.GetValues(typeof(ShopItemType)))
            SetUpShop(shopItemType);
    }
    public void HideShop() =>
        StartCoroutine(AnimateCard(transform.GetChild(3), false));
    
    public void ShowStore()
    {
        var store = transform.GetChild(4);
        store.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        StartCoroutine(AnimateCard(store, true));
    }
    
    public void HideStore() =>
        StartCoroutine(AnimateCard(transform.GetChild(4), false));
    
    public void ShowStoreEverywhere()
    {
        var store = transform.GetChild(4);
        store.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1f);
        StartCoroutine(AnimateCard(store, true, -300));
    }

    public void Upgrade(int upgradeType)
    {
        switch ((UpgradeType)upgradeType)
        {
            case UpgradeType.Attack:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.atkPrice)
                    return;
                PlayerManager.Instance.atkLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.atkPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.atkPrice = PriceCalculator(PlayerManager.Instance.atkLvl);
                break;
            
            case UpgradeType.MovementSpeed:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.msPrice)
                    return;
                PlayerManager.Instance.msLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.msPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.msPrice = PriceCalculator(PlayerManager.Instance.msLvl);
                break;
            
            case UpgradeType.JumpPower:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.jumpPrice)
                    return;
                PlayerManager.Instance.jumpLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.jumpPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.jumpPrice = PriceCalculator(PlayerManager.Instance.jumpLvl);
                break;

            case UpgradeType.Defence:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.defPrice)
                    return;
                PlayerManager.Instance.defLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.defPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.defPrice = PriceCalculator(PlayerManager.Instance.defLvl);
                break;
            
            case UpgradeType.MaxHealth:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.hpPrice)
                    return;
                PlayerManager.Instance.hpLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.hpPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.hpPrice = PriceCalculator(PlayerManager.Instance.hpLvl);
                break;
            
            default:
                throw new Exception("This UpgradeType was not expected.");
        }
    }

    public void BuyBuff(int buff)
    {
        switch ((ShopItemType)buff)
        {
            case ShopItemType.JumpBuff:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.jumpBuffsPrice)
                    return;
                PlayerManager.Instance.jumpBuffs++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.jumpBuffsPrice);
                SetUpShop((ShopItemType)buff);
                break;

            case ShopItemType.SpeedBuff:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.speedBuffsPrice)
                    return;
                PlayerManager.Instance.speedBuffs++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.speedBuffsPrice);
                SetUpShop((ShopItemType)buff);
                break;
            
            case ShopItemType.SecondChance:
                if (PlayerManager.Instance.gold < PlayerManager.Instance.secondChancesPrice)
                    return;
                PlayerManager.Instance.secondChances++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.secondChancesPrice);
                SetUpShop((ShopItemType)buff);
                break;
            
            default:
                throw new Exception("This ShopItemType was not expected.");
        }
    }
    
    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;
    
    private void SetUpUpgrades(UpgradeType upgradeType)
    {
        var (lvl, price) = upgradeType switch
        {
            UpgradeType.Attack => (PlayerManager.Instance.atkLvl,PlayerManager.Instance.atkPrice),

            UpgradeType.MovementSpeed => (PlayerManager.Instance.msLvl,PlayerManager.Instance.msPrice),

            UpgradeType.JumpPower => (PlayerManager.Instance.jumpLvl,PlayerManager.Instance.jumpPrice),

            UpgradeType.Defence => (PlayerManager.Instance.defLvl,PlayerManager.Instance.defPrice),

            UpgradeType.MaxHealth => (PlayerManager.Instance.hpLvl,PlayerManager.Instance.hpPrice),

            _ => throw new Exception("This UpgradeType was not expected.")
        };
        var element = transform.GetChild(2).GetChild((int)upgradeType);

        Buy(element.GetChild(2), lvl, price);
        ColorLevels(element.GetChild(1), lvl);
    }

    private void SetUpShop(ShopItemType shopItemType)
    {
        var (amount, price) = shopItemType switch
        {
            ShopItemType.JumpBuff => (PlayerManager.Instance.jumpBuffs, PlayerManager.Instance.jumpBuffsPrice),

            ShopItemType.SpeedBuff => (PlayerManager.Instance.speedBuffs, PlayerManager.Instance.speedBuffsPrice),

            ShopItemType.SecondChance => (PlayerManager.Instance.secondChances, PlayerManager.Instance.secondChancesPrice),

            _ => throw new Exception("This ShopItemType was not expected.")
        };
        
        var item = transform.GetChild(3).GetChild((int)shopItemType);
        
        item.GetChild(2).GetComponent<ParameterizedLocalizedString>().SetObject(new object[]{amount});
        item.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = Utils.DoubleToString(price, false);
    }
    
    private static void Buy(Transform label, int lvl, int price)
    {
        var labelText = label.GetChild(0);
        var labelTextComponent = labelText.GetComponent<TextMeshProUGUI>();
        var labelTextRectTransform= labelText.GetComponent<RectTransform>();
        
        if (lvl < 50)
            labelTextComponent.text = Utils.DoubleToString(price, false);
        else
        {
            labelTextComponent.text = "MAX";
            labelTextComponent.alignment = TextAlignmentOptions.Center;
            labelTextRectTransform.anchorMax = new Vector2(1f, labelTextRectTransform.anchorMax.y);
            
            label.GetChild(1).gameObject.SetActive(false);
            label.GetComponent<Button>().interactable = false;
        }        
    }
    
    private static void ColorLevels(Transform lvl, int amount)
    {
        for (var i = 0; i < lvl.childCount; i++)
        {
            var calculatedAtk = amount - i;
            lvl.GetChild(i).GetComponent<Image>().color = new Color(
                calculatedAtk <= 0 ? 0.5f : calculatedAtk is <= 10 or > 40 ? 1f : 0f,
                calculatedAtk <= 0 ? 0.5f : calculatedAtk <= 30 ? 1f : 0f,
                calculatedAtk <= 0 ? 0.5f : calculatedAtk is > 20 and <= 40 ? 1f : 0f);
        }
    }
    
    private IEnumerator ActivateMenu()
    {
        var direction = (Random.Range(0, 2) * 2 - 1) * (int)Direction.Random;
        foreach (Transform child in menu)
        {
            StartCoroutine(AnimateOneMenu(child, false, direction));
            yield return new WaitForSeconds(0.2f);
            direction *= -1;
        }

        var buttons = transform.GetChild(1);
        var childCount = buttons.childCount; 
        
        for (var i = childCount - 1; i >= 0; i--)
        {
            StartCoroutine(AnimateOneMenu(buttons.GetChild(i), true, (int)Direction.Right));
            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine(AnimateOneMenu(transform.GetChild(0), true, (int)Direction.Left));
    }
    
    private IEnumerator CloseMenu()
    {
        StartCoroutine(AnimateOneMenu(transform.GetChild(0), false, (int)Direction.Left));

        var buttons = transform.GetChild(1);
        var childCount = buttons.childCount; 

        for (var i = childCount - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(AnimateOneMenu(buttons.GetChild(i), false, (int)Direction.Right));
        }

        var direction = (Random.Range(0, 2) * 2 - 1) * (int)Direction.Random;
        foreach (Transform child in menu)
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(AnimateOneMenu(child, true, direction));
            direction *= -1;
        }
    }

    private static IEnumerator AnimateOneMenu(Component component, bool isAppearing, int direction)
    {
        if (isAppearing)
            component.gameObject.SetActive(true);
        
        var currentTime = 0f;
        var objectToMove = component.GetComponent<RectTransform>();
        
        while (currentTime < 0.3f)
        {
            currentTime += Time.deltaTime;
            
            objectToMove.anchoredPosition = 
                new Vector2(Mathf.Lerp(isAppearing? direction : 0f, isAppearing? 0f : direction, currentTime / 0.3f), 0f);

            yield return null;
        }

        if (!isAppearing)
            component.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2(0f, 0f);
    }

    private static IEnumerator AnimateCard(Component card, bool isAppearing, float? finalXPosition = 0f)
    {
        if (isAppearing)
            card.gameObject.SetActive(true);
        
        var currentTime = 0f;
        var objectToMove = card.GetComponent<RectTransform>();
        
        while (currentTime < 0.5f)
        {
            currentTime += Time.deltaTime;
            
            objectToMove.anchoredPosition = 
                new Vector2(Mathf.Lerp(isAppearing? 1500f : (float)finalXPosition, isAppearing? (float)finalXPosition : 1500f, currentTime / 0.5f), Mathf.Lerp(isAppearing? 1500f : 0f, isAppearing? 0f :- 1500f, currentTime / 0.5f));

            objectToMove.rotation = 
                Quaternion.Euler(0f, 0f, Mathf.Lerp(isAppearing? -720f : 0f, isAppearing? 0f : -720f, currentTime / 0.5f));

            yield return null;
        }

        if (!isAppearing)
            card.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2((float)finalXPosition, 0f);
        objectToMove.rotation = Quaternion.Euler(0, 0, 0);
    }
}
