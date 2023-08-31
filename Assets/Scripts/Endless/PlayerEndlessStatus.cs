using TMPro;
using UnityEngine;

/// <summary>
/// Manages the display and interactions related to player's revenue (gold and gems) in an endless game mode.
/// </summary>
public class PlayerEndlessStatus : MonoBehaviour
{
    [SerializeField] private Transform revenue;

    private void Start() =>
        DisplayRevenue();

    private void DisplayRevenue()
    {
        DisplayGold();
        DisplayGems();
    }

    private void DisplayGold() =>
        revenue.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{PlayerManager.Instance.Gold:n0}";

    private void DisplayGems() =>
        revenue.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{PlayerManager.Instance.Gems:n0}";

    public void UpdateGold(int amount)
    {
        PlayerManager.Instance.Gold += amount;
        DisplayGold();
    }

    public void UpdateGems(int amount)
    {
        PlayerManager.Instance.Gems += amount;
        DisplayGems();
    }

    public void BuyGold(int deal)
    {
        switch (deal)
        {
            default:
                if (PlayerManager.Instance.Gems < 10) return;
                UpdateGold(1_000);
                UpdateGems(-10);
                break;
        }
    }

    public void BuyGems(int deal)
    {
        switch (deal)
        {
            default:
                UpdateGems(1_000);
                break;
        }
    }
}