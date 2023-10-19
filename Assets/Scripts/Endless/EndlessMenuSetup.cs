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
    [SerializeField] private Transform upgrade, shop, store;
    [SerializeField] private Sprite enableButtonIcon, disableButtonIcon, goldIcon, gemIcon;
    private RevenueHandler _revenueHandler;
    private PlayerManager _playerManager;

    private void Awake() => _revenueHandler = GetComponent<RevenueHandler>();

    private void Start()
    {
        _playerManager = PlayerManager.Instance;
        ShowUpgrades();
        ShowShop();
        ShowStore();
    }
    
    public void ShowUpgrades()
    {
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType))) SetUpUpgrades(upgradeType);
    }

    public void ShowShop()
    {
        foreach (BuffType buffType in Enum.GetValues(typeof(BuffType))) SetUpShop(buffType);
    }

    public void ShowStore()
    {
        var enoughDiamonds = _playerManager.Gems >= 10;
        var goldPage = store.GetChild(1);
        for(var i = 0; i < goldPage.childCount-1; i++)
        {
            var deal = goldPage.GetChild(i);
            var contentSection = deal.GetChild(0);
            var buySection = deal.GetChild(1);
            var price = buySection.GetChild(0);
            
            deal.GetComponent<Button>().interactable = enoughDiamonds;

            contentSection.GetChild(0).GetComponent<Image>().color =
                contentSection.GetChild(1).GetComponent<TextMeshProUGUI>().color =
                    buySection.GetChild(1).GetComponent<TextMeshProUGUI>().color =
                        deal.GetChild(2).GetComponent<Image>().color = enoughDiamonds ? Color.white : Color.grey;

            price.GetChild(0).GetComponent<TextMeshProUGUI>().color = price.GetChild(1).GetComponent<Image>().color =
                enoughDiamonds ? Color.white : Color.red;
        }
    }

    private void DisplayMoneyCheck(Transform price, bool sufficientMoney)
    {
        var button = price.parent;
        price.GetComponent<TextMeshProUGUI>().color = sufficientMoney ? Color.white : Color.red;
        button.GetComponent<Image>().sprite = sufficientMoney ? enableButtonIcon : disableButtonIcon;
        button.GetComponent<Button>().interactable = sufficientMoney;
    }

    /// <summary>
    /// Upgrade player status.
    /// </summary>
    /// <param name="upgradeType">Int representing an <see cref="UpgradeType"/>.</param>
    /// <exception cref="Exception">Throw when <paramref name="upgradeType"/> exceed <see cref="UpgradeType"/> length.</exception>
    public void Upgrade(int upgradeType)
    {
        var lvl = _playerManager.Upgrades[upgradeType].Quantity;
        if (lvl != 0 && lvl % 10 == 0)
        {
            if (!_revenueHandler.TryToConsumeGems((int)(lvl * .5))) return;
        }
        else if (!_revenueHandler.TryToConsumeGold(_playerManager.Upgrades[upgradeType].Price)) return;

        _playerManager.Upgrades[upgradeType].Quantity++;
        _playerManager.Upgrades[upgradeType].Price = PriceCalculator(_playerManager.Upgrades[upgradeType].Quantity); 
        SetUpUpgrades((UpgradeType)upgradeType);
        ShowUpgrades();
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
        ShowShop();
    }

    private static int PriceCalculator(int lvl) => (lvl + 1) * 15;

    private void SetUpUpgrades(UpgradeType upgradeType)
    {
        var upgradeDetail = _playerManager.Upgrades[(int)upgradeType];
        var element = upgrade.GetChild((int)upgradeType);
        
        SetUpColorForLevels(element.GetChild(1), upgradeDetail.Quantity);
        SetUpText(element.GetChild(2), upgradeDetail.Quantity, upgradeDetail.Price);
    }

    private void SetUpShop(BuffType buffType)
    {
        var buffDetail = _playerManager.Buffs[(int)buffType];
        var item = shop.GetChild((int)buffType);
        var priceTransform = item.GetChild(3).GetChild(1);
        
        item.GetChild(2).GetComponent<ParameterizedLocalizedString>().SetObject(new object[] { buffDetail.Quantity });
        priceTransform.GetComponent<TextMeshProUGUI>().text =
            Utility.FormatDoubleWithUnits(buffDetail.Price, false);
        
        DisplayMoneyCheck(priceTransform, buffDetail.Price <= _playerManager.Gold);
    }

    private void SetUpText(Transform label, int lvl, int price)
    {
        var labelText = label.GetChild(0);
        var labelTextComponent = labelText.GetComponent<TextMeshProUGUI>();
        var isRankingUp = lvl != 0 && lvl % 10 == 0;
        
        label.GetChild(1).gameObject.SetActive(!isRankingUp);
        label.GetChild(2).gameObject.SetActive(isRankingUp);
        label.GetChild(3).GetComponent<CustomParticles>().particleImage = isRankingUp ? gemIcon : goldIcon;
        
        price = isRankingUp ? (int)(lvl * .5) : price;
        
        if (lvl < 50)
        {
            labelTextComponent.text = Utility.FormatDoubleWithUnits(price, false);
            DisplayMoneyCheck(labelText, price <= (isRankingUp ? _playerManager.Gems : _playerManager.Gold));
        }
        else
        {
            var labelTextRectTransform = labelText.GetComponent<RectTransform>();

            labelTextComponent.text = "MAX";
            labelTextComponent.alignment = TextAlignmentOptions.Center;
            labelTextRectTransform.anchorMax = new Vector2(1f, labelTextRectTransform.anchorMax.y);
            labelTextComponent.color = Color.gray;

            label.GetChild(1).gameObject.SetActive(false);
            label.GetComponent<Button>().interactable = false;
            label.GetComponent<Image>().sprite = disableButtonIcon;
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