using UnityEngine;

public class Coin : MonoBehaviour
{
    private PlayerStatus _playerStatus;

    private void Awake() =>
        _playerStatus = FindObjectOfType<PlayerStatus>();


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        
        Destroy(gameObject);
        _playerStatus.AddCoin();
    }
}
