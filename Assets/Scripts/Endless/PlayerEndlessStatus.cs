using TMPro;
using UnityEngine;
    
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
        revenue.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{PlayerManager.Instance.gold:n0}";
    
    private void DisplayGems() =>
        revenue.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{PlayerManager.Instance.gems:n0}";

    public void UpdateGold(int amount)
    {
        PlayerManager.Instance.gold += amount;
        DisplayGold();
    }
    
    public void UpdateGems(int amount)
    {
        PlayerManager.Instance.gems += amount;
        DisplayGems();
    }
    
    public void BuyGold(int deal)
    {
        switch (deal)
        {
            default:
                if (PlayerManager.Instance.gems < 10) return;
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
