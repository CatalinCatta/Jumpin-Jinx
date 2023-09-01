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

    private void Awake() =>
        _playerEndlessStatus = GetComponent<PlayerEndlessStatus>();

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
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.AtkPrice)
                    return;
                PlayerManager.Instance.AtkLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.AtkPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.AtkPrice = PriceCalculator(PlayerManager.Instance.AtkLvl);
                break;

            case UpgradeType.MovementSpeed:
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.MsPrice)
                    return;
                PlayerManager.Instance.MSLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.MsPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.MsPrice = PriceCalculator(PlayerManager.Instance.MSLvl);
                break;

            case UpgradeType.JumpPower:
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.JumpPrice)
                    return;
                PlayerManager.Instance.JumpLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.JumpPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.JumpPrice = PriceCalculator(PlayerManager.Instance.JumpLvl);
                break;

            case UpgradeType.Defence:
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.DefPrice)
                    return;
                PlayerManager.Instance.DefLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.DefPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.DefPrice = PriceCalculator(PlayerManager.Instance.DefLvl);
                break;

            case UpgradeType.MaxHealth:
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.HpPrice)
                    return;
                PlayerManager.Instance.HpLvl++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.HpPrice);
                SetUpUpgrades((UpgradeType)upgradeType);
                PlayerManager.Instance.HpPrice = PriceCalculator(PlayerManager.Instance.HpLvl);
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
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.JumpBuffsPrice)
                    return;
                PlayerManager.Instance.JumpBuffs++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.JumpBuffsPrice);
                SetUpShop((ShopItemType)buff);
                break;

            case ShopItemType.SpeedBuff:
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.SpeedBuffsPrice)
                    return;
                PlayerManager.Instance.SpeedBuffs++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.SpeedBuffsPrice);
                SetUpShop((ShopItemType)buff);
                break;

            case ShopItemType.SecondChance:
                if (PlayerManager.Instance.Gold < PlayerManager.Instance.SecondChancesPrice)
                    return;
                PlayerManager.Instance.SecondChances++;
                _playerEndlessStatus.UpdateGold(-PlayerManager.Instance.SecondChancesPrice);
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
            UpgradeType.Attack => (PlayerManager.Instance.AtkLvl, PlayerManager.Instance.AtkPrice),

            UpgradeType.MovementSpeed => (PlayerManager.Instance.MSLvl, PlayerManager.Instance.MsPrice),

            UpgradeType.JumpPower => (PlayerManager.Instance.JumpLvl, PlayerManager.Instance.JumpPrice),

            UpgradeType.Defence => (PlayerManager.Instance.DefLvl, PlayerManager.Instance.DefPrice),

            UpgradeType.MaxHealth => (PlayerManager.Instance.HpLvl, PlayerManager.Instance.HpPrice),

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
            ShopItemType.JumpBuff => (PlayerManager.Instance.JumpBuffs, PlayerManager.Instance.JumpBuffsPrice),

            ShopItemType.SpeedBuff => (PlayerManager.Instance.SpeedBuffs, PlayerManager.Instance.SpeedBuffsPrice),

            ShopItemType.SecondChance => (PlayerManager.Instance.SecondChances, PlayerManager.Instance.SecondChancesPrice),

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