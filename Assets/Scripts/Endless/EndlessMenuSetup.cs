using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the setup, interactions, and animations for the endless game mode menu.
/// </summary>
[RequireComponent(typeof(PlayerEndlessStatus))]
public class EndlessMenuSetup : MonoBehaviour
{
    [SerializeField] private Transform menu;
    private PlayerEndlessStatus _playerEndlessStatus;
    private PlayerManager _playerManager;

    private void Awake() =>
        _playerEndlessStatus = GetComponent<PlayerEndlessStatus>();

    private void Start() =>
        _playerManager = (PlayerManager)IndestructibleManager.Instance;

    /// <summary>
    /// Open Endless Menu.
    /// </summary>
    public void Enable() =>
        StartCoroutine(ActivateMenu());

    /// <summary>
    /// Close Endless Menu.
    /// </summary>
    public void Disable() =>
        StartCoroutine(CloseMenu());

    /// <summary>
    /// Open Upgrades Menu.
    /// </summary>
    public void ShowUpgrades()
    {
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType)))
            SetUpUpgrades(upgradeType);

        StartCoroutine(AnimateCard(transform.GetChild(2), true));
    }

    /// <summary>
    /// Close Upgrades Menu.
    /// </summary>
    public void HideUpgrades() =>
        StartCoroutine(AnimateCard(transform.GetChild(2), false));

    /// <summary>
    /// Open Shop Menu.
    /// </summary>
    public void ShowShop()
    {
        StartCoroutine(AnimateCard(transform.GetChild(3), true));

        foreach (ShopItemType shopItemType in Enum.GetValues(typeof(ShopItemType)))
            SetUpShop(shopItemType);
    }

    /// <summary>
    /// Close Shop Menu.
    /// </summary>
    public void HideShop() =>
        StartCoroutine(AnimateCard(transform.GetChild(3), false));

    /// <summary>
    /// Open Store Menu.
    /// </summary>
    public void ShowStore()
    {
        var store = transform.GetChild(4);
        store.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        StartCoroutine(AnimateCard(store, true));
    }

    /// <summary>
    /// Close Store Menu.
    /// </summary>
    public void HideStore() =>
        StartCoroutine(AnimateCard(transform.GetChild(4), false));

    /// <summary>
    /// Open Global Store Menu.
    /// </summary>
    public void ShowStoreEverywhere()
    {
        var store = transform.GetChild(4);
        store.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1f);
        StartCoroutine(AnimateCard(store, true, -300));
    }

    /// <summary>
    /// Upgrade player status.
    /// </summary>
    /// <param name="upgradeType">Int representing an <see cref="UpgradeType"/>.</param>
    /// <exception cref="Exception">Throw when <paramref name="upgradeType"/> exceed <see cref="UpgradeType"/> length.</exception>
    public void Upgrade(int upgradeType)
    {
        switch ((UpgradeType)upgradeType)
        {
            case UpgradeType.Attack:
                if (_playerManager.Gold < _playerManager.AtkPrice)
                    return;
                _playerManager.AtkLvl++;
                _playerEndlessStatus.UpdateGold(-_playerManager.AtkPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                _playerManager.AtkPrice = PriceCalculator(_playerManager.AtkLvl);
                break;

            case UpgradeType.MovementSpeed:
                if (_playerManager.Gold < _playerManager.MsPrice)
                    return;
                _playerManager.MSLvl++;
                _playerEndlessStatus.UpdateGold(-_playerManager.MsPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                _playerManager.MsPrice = PriceCalculator(_playerManager.MSLvl);
                break;

            case UpgradeType.JumpPower:
                if (_playerManager.Gold < _playerManager.JumpPrice)
                    return;
                _playerManager.JumpLvl++;
                _playerEndlessStatus.UpdateGold(-_playerManager.JumpPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                _playerManager.JumpPrice = PriceCalculator(_playerManager.JumpLvl);
                break;

            case UpgradeType.Defence:
                if (_playerManager.Gold < _playerManager.DefPrice)
                    return;
                _playerManager.DefLvl++;
                _playerEndlessStatus.UpdateGold(-_playerManager.DefPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                _playerManager.DefPrice = PriceCalculator(_playerManager.DefLvl);
                break;

            case UpgradeType.MaxHealth:
                if (_playerManager.Gold < _playerManager.HpPrice)
                    return;
                _playerManager.HpLvl++;
                _playerEndlessStatus.UpdateGold(-_playerManager.HpPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                _playerManager.HpPrice = PriceCalculator(_playerManager.HpLvl);
                break;

            default:
                throw new Exception("This UpgradeType was not expected.");
        }
    }

    /// <summary>
    /// Buy Power Ups.
    /// </summary>
    /// <param name="buff">Int representing an <see cref="ShopItemType"/>.</param>
    /// <exception cref="Exception">Throw when <paramref name="buff"/> exceed <see cref="ShopItemType"/> length.</exception>
    public void BuyBuff(int buff)
    {
        switch ((ShopItemType)buff)
        {
            case ShopItemType.JumpBuff:
                if (_playerManager.Gold < _playerManager.JumpBuffsPrice)
                    return;
                _playerManager.JumpBuffs++;
                _playerEndlessStatus.UpdateGold(-_playerManager.JumpBuffsPrice);
                SetUpShop((ShopItemType)buff);
                break;

            case ShopItemType.SpeedBuff:
                if (_playerManager.Gold < _playerManager.SpeedBuffsPrice)
                    return;
                _playerManager.SpeedBuffs++;
                _playerEndlessStatus.UpdateGold(-_playerManager.SpeedBuffsPrice);
                SetUpShop((ShopItemType)buff);
                break;

            case ShopItemType.SecondChance:
                if (_playerManager.Gold < _playerManager.SecondChancesPrice)
                    return;
                _playerManager.SecondChances++;
                _playerEndlessStatus.UpdateGold(-_playerManager.SecondChancesPrice);
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
            UpgradeType.Attack => (_playerManager.AtkLvl, _playerManager.AtkPrice),

            UpgradeType.MovementSpeed => (_playerManager.MSLvl, _playerManager.MsPrice),

            UpgradeType.JumpPower => (_playerManager.JumpLvl, _playerManager.JumpPrice),

            UpgradeType.Defence => (_playerManager.DefLvl, _playerManager.DefPrice),

            UpgradeType.MaxHealth => (_playerManager.HpLvl, _playerManager.HpPrice),

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
            ShopItemType.JumpBuff => (_playerManager.JumpBuffs, _playerManager.JumpBuffsPrice),

            ShopItemType.SpeedBuff => (_playerManager.SpeedBuffs, _playerManager.SpeedBuffsPrice),

            ShopItemType.SecondChance => (_playerManager.SecondChances, _playerManager.SecondChancesPrice),

            _ => throw new Exception("This ShopItemType was not expected.")
        };
        var item = transform.GetChild(3).GetChild((int)shopItemType);

        item.GetChild(2).GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { amount });
        item.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = Utility.FormatDoubleWithUnits(price, false);
    }

    private static void Buy(Transform label, int lvl, int price)
    {
        var labelText = label.GetChild(0);
        var labelTextComponent = labelText.GetComponent<TextMeshProUGUI>();
        var labelTextRectTransform = labelText.GetComponent<RectTransform>();

        if (lvl < 50)
            labelTextComponent.text = Utility.FormatDoubleWithUnits(price, false);
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
                calculatedAtk <= 0 ? .5f : calculatedAtk is <= 10 or > 40 ? 1f : 0f,
                calculatedAtk <= 0 ? .5f : calculatedAtk <= 30 ? 1f : 0f,
                calculatedAtk <= 0 ? .5f : calculatedAtk is > 20 and <= 40 ? 1f : 0f);
        }
    }

    private IEnumerator ActivateMenu()
    {
        var direction = (Random.Range(0, 2) * 2 - 1) * (int)MenuDirection.Random;
        var menuChildNr = menu.childCount;

        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 2), false, (int)MenuDirection.Left));
        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 1), false, (int)MenuDirection.Right));
        yield return new WaitForSeconds(.2f);

        for (var i = 0; i < menuChildNr - 2; i++)
        {
            StartCoroutine(AnimateOneMenu(menu.GetChild(i), false, direction));
            yield return new WaitForSeconds(.2f);
            direction *= -1;
        }

        var buttons = transform.GetChild(1);
        var childCount = buttons.childCount;

        for (var i = childCount - 1; i >= 0; i--)
        {
            StartCoroutine(AnimateOneMenu(buttons.GetChild(i), true, (int)MenuDirection.Right));
            yield return new WaitForSeconds(.2f);
        }

        StartCoroutine(AnimateOneMenu(transform.GetChild(0), true, (int)MenuDirection.Left));
    }

    private IEnumerator CloseMenu()
    {
        StartCoroutine(AnimateOneMenu(transform.GetChild(0), false, (int)MenuDirection.Left));

        var buttons = transform.GetChild(1);
        var childCount = buttons.childCount;

        for (var i = childCount - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(.2f);
            StartCoroutine(AnimateOneMenu(buttons.GetChild(i), false, (int)MenuDirection.Right));
        }

        var direction = (Random.Range(0, 2) * 2 - 1) * (int)MenuDirection.Random;
        var menuChildNr = menu.childCount;

        for (var i = 0; i < menuChildNr - 2; i++)
        {
            yield return new WaitForSeconds(.2f);
            StartCoroutine(AnimateOneMenu(menu.GetChild(i), true, direction));
            direction *= -1;
        }

        yield return new WaitForSeconds(.2f);
        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 2), true, (int)MenuDirection.Left));
        StartCoroutine(AnimateOneMenu(menu.GetChild(menuChildNr - 1), true, (int)MenuDirection.Right));
    }

    private static IEnumerator AnimateOneMenu(Component component, bool isAppearing, int direction)
    {
        if (isAppearing)
            component.gameObject.SetActive(true);

        var currentTime = 0f;
        var objectToMove = component.GetComponent<RectTransform>();

        while (currentTime < .3f)
        {
            currentTime += Time.deltaTime;

            objectToMove.anchoredPosition =
                new Vector2(Mathf.Lerp(isAppearing ? direction : 0f, isAppearing ? 0f : direction, currentTime / .3f),
                    0f);

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

        while (currentTime < .5f)
        {
            currentTime += Time.deltaTime;

            objectToMove.anchoredPosition =
                new Vector2(
                    Mathf.Lerp(isAppearing ? 1500f : (float)finalXPosition, isAppearing ? (float)finalXPosition : 1500f,
                        currentTime / .5f),
                    Mathf.Lerp(isAppearing ? 1500f : 0f, isAppearing ? 0f : -1500f, currentTime / .5f));

            objectToMove.rotation =
                Quaternion.Euler(0f, 0f,
                    Mathf.Lerp(isAppearing ? -720f : 0f, isAppearing ? 0f : -720f, currentTime / .5f));

            yield return null;
        }

        if (!isAppearing)
            card.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2((float)finalXPosition, 0f);
        objectToMove.rotation = Quaternion.Euler(0, 0, 0);
    }
}