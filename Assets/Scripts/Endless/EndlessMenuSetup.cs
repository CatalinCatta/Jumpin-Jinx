using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the setup, interactions, and animations for the endless game mode menu.
/// </summary>
[RequireComponent(typeof(RevenueHandler))]
public class EndlessMenuSetup : MonoBehaviour
{
    [SerializeField] private Transform upgrade, shop;
    private RevenueHandler _revenueHandler;
    private PlayerManager _playerManager;

    private void Awake() => _revenueHandler = GetComponent<RevenueHandler>();

    private void Start()
    {
        _playerManager = PlayerManager.Instance;
        ShowUpgrades();
        ShowShop();
    }
    
    /// <summary>
    /// Open Upgrades Menu.
    /// </summary>
    private void ShowUpgrades() // TODO: Add visual aspect to buy button to represent if can or cannot be bought.
    {
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType))) SetUpUpgrades(upgradeType);
    }


    /// <summary>
    /// Open Shop Menu.
    /// </summary>
    private void ShowShop() // TODO: Add visual aspect to buy button to represent if can or cannot be bought.
    {
        foreach (BuffType buffType in Enum.GetValues(typeof(BuffType))) SetUpShop(buffType);
    }

    /// <summary>
    /// Upgrade player status.
    /// </summary>
    /// <param name="upgradeType">Int representing an <see cref="UpgradeType"/>.</param>
    /// <exception cref="Exception">Throw when <paramref name="upgradeType"/> exceed <see cref="UpgradeType"/> length.</exception>
    public void Upgrade(int upgradeType)
    {
        if (!_revenueHandler.TryToConsumeGold(_playerManager.Upgrades[upgradeType].Price)) return;

        _playerManager.Upgrades[upgradeType].Quantity++;
        _playerManager.Upgrades[upgradeType].Price = PriceCalculator(_playerManager.Upgrades[upgradeType].Quantity); 
        SetUpUpgrades((UpgradeType)upgradeType);
    }

    /// <summary>
    /// Buy Power Ups.
    /// </summary>
    /// <param name="buff">Int representing an <see cref="BuffType"/>.</param>
    /// <exception cref="Exception">Throw when <paramref name="buff"/> exceed <see cref="BuffType"/> length.</exception>
    public void BuyBuff(int buff)
    {
        if (!_revenueHandler.TryToConsumeGold(_playerManager.Buffs[buff].Price)) return;
        _playerManager.Buffs[buff].Quantity++;
        SetUpShop((BuffType)buff);
    }

    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;

    private void SetUpUpgrades(UpgradeType upgradeType)
    {
        var upgradeDetail = _playerManager.Upgrades[(int)upgradeType];
        var element = upgrade.GetChild((int)upgradeType);

        SetUpText(element.GetChild(2), upgradeDetail.Quantity, upgradeDetail.Price);
        SetUpColorForLevels(element.GetChild(1), upgradeDetail.Quantity);
    }

    private void SetUpShop(BuffType buffType)
    {
        var buffDetail = _playerManager.Buffs[(int)buffType];
        var item = shop.GetChild((int)buffType);

        item.GetChild(2).GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { buffDetail.Quantity });
        item.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text =
            Utility.FormatDoubleWithUnits(buffDetail.Price, false);
    }

    private static void SetUpText(Transform label, int lvl, int price)
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

    private static void SetUpColorForLevels(Transform lvl, int amount)
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
}