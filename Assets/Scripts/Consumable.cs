using System;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    private PlayerStatus _playerStatus;

    [SerializeField] private ConsumableType consumableType;
    
    private void Awake() =>
        _playerStatus = FindObjectOfType<PlayerStatus>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        switch (consumableType)
        {
            case ConsumableType.Coin:
                _playerStatus.AddCoin();
                break;
            case ConsumableType.Heal:        
                _playerStatus.Heal();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Destroy(gameObject);
    }
}
