using TMPro;
using UnityEngine;

/// <summary>
/// Manages the display and interactions related to player's revenue (gold and gems) in an endless game mode.
/// </summary>
public class RevenueHandler : MonoBehaviour
{
    [SerializeField] private Transform revenue;
    private PlayerManager _playerManager;
    
    private void Start()
    {
        _playerManager = PlayerManager.Instance;
        DisplayRevenue();
    }

    private void DisplayRevenue()
    {
        DisplayGold();
        DisplayGems();
    }

    private void DisplayGold() => DisplayRevenue(0, _playerManager.Gold);

    private void DisplayGems() => DisplayRevenue(1, _playerManager.Gems);

    private void DisplayRevenue(int child, int amount) =>
        revenue.GetChild(child).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{amount:n0}";
    
    /// <summary>
    /// Checks if the player has enough gold and, if so, consumes the specified amount of gold.
    /// </summary>
    /// <param name="amount">The amount of gold to consume.</param>
    /// <returns>
    ///   <c>true</c> if the player has enough gold and the gold is successfully consumed; otherwise, <c>false</c>.
    /// </returns>
    public bool TryToConsumeGold(int amount)
    {
        if (_playerManager.Gold < amount) return false;
        UpdateGold(-amount);
        return true;
    }

    private void UpdateGold(int amount)
    {
        _playerManager.Gold += amount;
        DisplayGold();
    }

    /// <summary>
    /// Checks if the player has enough gems and, if so, consumes the specified amount of gems.
    /// </summary>
    /// <param name="amount">The amount of gems to consume.</param>
    /// <returns>
    ///   <c>true</c> if the player has enough gems and the gems is successfully consumed; otherwise, <c>false</c>.
    /// </returns>
    public bool TryToConsumeGems(int amount)
    {
        if (_playerManager.Gems < amount) return false;
        UpdateGems(-amount);
        return true;
    }
    
    private void UpdateGems(int amount)
    {
        _playerManager.Gems += amount;
        DisplayGems();
    }

    public void BuyGold(int deal)
    {
        switch (deal)
        {
            case 7:
                UpdateGold(_playerManager.Gold > 10_000 ? -10_000 : -_playerManager.Gold);
                break;

            default:
                if (!TryToConsumeGems(10)) return;
                UpdateGold(1_000);
                break;
        }
    }

    public void BuyGems(int deal)
    {
        switch (deal)
        {
            case 7:
                UpdateGems(_playerManager.Gems > 1_000 ? -1_000 : -_playerManager.Gems);
                break;
            
            default:
                UpdateGems(1_000);
                break;
        }
    }
    
    
}