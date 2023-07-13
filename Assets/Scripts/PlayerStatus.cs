using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private GameObject display;

    private int _coins;
    
    public void AddCoin()
    {
        _coins++;
        display.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();
    }
}
