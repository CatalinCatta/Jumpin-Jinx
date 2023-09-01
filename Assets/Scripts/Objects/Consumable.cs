using System;
using UnityEngine;

/// <summary>
/// Represents a consumable item that can be collected by the player.
/// </summary>
public class Consumable : MonoBehaviour
{
    [SerializeField] private ConsumableType consumableType;
    private bool _isDestroying;
    private PlayerStatus _playerStatus;

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

        StartCoroutine(Utility.PlayDeathSoundAndCleanup(gameObject));
    }
}