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
        Physics2D.IgnoreCollision(collision.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());

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

        StartCoroutine(Utils.PlaySoundOnDeath(gameObject));
    }
}
