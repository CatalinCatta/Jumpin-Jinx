using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the setup, interactions, and animations for the endless game mode menu.
/// </summary>
[RequireComponent(typeof(RevenueHandler))]
public class EndlessMenuSetup : MonoBehaviour
{
    [SerializeField] private Transform menu;
    private Transform _transform, _upgrade, _shop, _store;
    private RevenueHandler _revenueHandler;
    private PlayerManager _playerManager;

    private void Awake() => _revenueHandler = GetComponent<RevenueHandler>();

    private void Start()
    {
        _transform = transform;
        _upgrade = _transform.GetChild(2);
        _shop = _transform.GetChild(3);
        _store = _transform.GetChild(4);
        _playerManager = (PlayerManager)IndestructibleManager.Instance;
    }

    /// <summary>
    /// Open Endless Menu.
    /// </summary>
    public void Enable() => StartCoroutine(ActivateMenu());

    /// <summary>
    /// Close Endless Menu.
    /// </summary>
    public void Disable() => StartCoroutine(CloseMenu());
    
    /// <summary>
    /// Open Upgrades Menu.
    /// </summary>
    public void ShowUpgrades() // TODO: Add visual aspect to buy button to represent if can or cannot be bought.
    {
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType))) SetUpUpgrades(upgradeType);

        StartCoroutine(AnimateCard(_upgrade, true));
    }

    /// <summary>
    /// Close Upgrades Menu.
    /// </summary>
    public void HideUpgrades() => StartCoroutine(AnimateCard(_upgrade, false));

    /// <summary>
    /// Open Shop Menu.
    /// </summary>
    public void ShowShop() // TODO: Add visual aspect to buy button to represent if can or cannot be bought.
    {
        StartCoroutine(AnimateCard(_shop, true));

        foreach (BuffType buffType in Enum.GetValues(typeof(BuffType))) SetUpShop(buffType);
    }

    /// <summary>
    /// Close Shop Menu.
    /// </summary>
    public void HideShop() => StartCoroutine(AnimateCard(_shop, false));

    /// <summary>
    /// Open Store Menu.
    /// </summary>
    public void ShowStore() // TODO: Add visual aspect to buy button to represent if can or cannot be bought.
    {
        _store.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        StartCoroutine(AnimateCard(_store, true));
    }

    /// <summary>
    /// Close Store Menu.
    /// </summary>
    public void HideStore() => StartCoroutine(AnimateCard(_store, false));

    /// <summary>
    /// Open Global Store Menu.
    /// </summary>
    public void ShowStoreEverywhere()
    {
        _store.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1f);
        StartCoroutine(AnimateCard(_store, true, -300));
    }

    /// <summary>
    /// Upgrade player status.
    /// </summary>
    /// <param name="upgradeType">Int representing an <see cref="UpgradeType"/>.</param>
    /// <exception cref="Exception">Throw when <paramref name="upgradeType"/> exceed <see cref="UpgradeType"/> length.</exception>
    public void Upgrade(int upgradeType)
    {
        if (!_revenueHandler.CanConsumeGold(_playerManager.Buffs[upgradeType].Price)) return;

        _playerManager.Buffs[upgradeType].Price = PriceCalculator(_playerManager.Buffs[upgradeType].Quantity++); 
        SetUpUpgrades((UpgradeType)upgradeType);
    }

    /// <summary>
    /// Buy Power Ups.
    /// </summary>
    /// <param name="buff">Int representing an <see cref="BuffType"/>.</param>
    /// <exception cref="Exception">Throw when <paramref name="buff"/> exceed <see cref="BuffType"/> length.</exception>
    public void BuyBuff(int buff)
    {
        if (!_revenueHandler.CanConsumeGold(_playerManager.Buffs[buff].Price)) return;
        _playerManager.Buffs[buff].Quantity++;
        SetUpShop((BuffType)buff);
    }

    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;

    private void SetUpUpgrades(UpgradeType upgradeType)
    {
        var upgradeDetail = _playerManager.Buffs[(int)upgradeType];
        var element = transform.GetChild(2).GetChild((int)upgradeType);

        Buy(element.GetChild(2), upgradeDetail.Quantity, upgradeDetail.Price);
        ColorLevels(element.GetChild(1), upgradeDetail.Quantity);
    }

    private void SetUpShop(BuffType buffType)
    {
        var buffDetail = _playerManager.Buffs[(int)buffType];
        var item = transform.GetChild(3).GetChild((int)buffType);

        item.GetChild(2).GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { buffDetail.Quantity });
        item.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            Utility.FormatDoubleWithUnits(buffDetail.Price, false);
    }

    private static void Buy(Transform label, int lvl, int price)
    {
        var labelText = label.GetChild(0);
        var labelTextComponent = labelText.GetComponent<TextMeshProUGUI>();

        if (lvl < 50)
            labelTextComponent.text = Utility.FormatDoubleWithUnits(price, false);
        else
        {
            var labelTextRectTransform = labelText.GetComponent<RectTransform>();

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
        if (isAppearing) component.gameObject.SetActive(true);

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

        if (!isAppearing) component.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2(0f, 0f);
    }

    private static IEnumerator AnimateCard(Component card, bool isAppearing, float? finalXPosition = 0f)
    {
        if (isAppearing) card.gameObject.SetActive(true);

        var currentTime = 0f;
        var objectToMove = card.GetComponent<RectTransform>();

        while (currentTime < .5f)
        {
            currentTime += Time.deltaTime;

            objectToMove.anchoredPosition = new Vector2(
                Mathf.Lerp(isAppearing ? 1500f : (float)finalXPosition!, isAppearing ? (float)finalXPosition! : 1500f,
                    currentTime / .5f),
                Mathf.Lerp(isAppearing ? 1500f : 0f, isAppearing ? 0f : -1500f, currentTime / .5f));

            objectToMove.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Lerp(isAppearing ? -720f : 0f, isAppearing ? 0f : -720f, currentTime / .5f));

            yield return null;
        }

        if (!isAppearing) card.gameObject.SetActive(false);

        objectToMove.anchoredPosition = new Vector2((float)finalXPosition!, 0f);
        objectToMove.rotation = Quaternion.Euler(0, 0, 0);
    }
}