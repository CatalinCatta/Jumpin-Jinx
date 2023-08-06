using System;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    private PlayerStatus _playerStatus;
    private bool _isDestroying;
    
    [SerializeField] private ConsumableType consumableType;
    
    private void Start() =>
        _playerStatus = FindObjectOfType<PlayerStatus>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player") || _isDestroying)
            return;

        _isDestroying = true;
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

        StartCoroutine(Utils.PlaySoundOnDeath(gameObject));
    }
}