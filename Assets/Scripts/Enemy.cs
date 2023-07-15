using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Death"))
            Destroy(gameObject);
        if (collision.gameObject.CompareTag("Coin"))
            Physics2D.IgnoreCollision(collision.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (collision.gameObject.CompareTag("Ground") && collision.transform.position.y < transform.position.y - 1.25f )
            transform.SetParent(collision.transform);
    }
}
