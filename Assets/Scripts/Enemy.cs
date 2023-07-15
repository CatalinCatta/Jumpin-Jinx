using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _direction;
    
    private void Start() => _direction = Utils.RandomPickNumberExcludingZero(2) == 1? -1f : 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Death"))
            Destroy(gameObject);
        if (collision.gameObject.CompareTag("Coin"))
            Physics2D.IgnoreCollision(collision.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (collision.gameObject.CompareTag("Ground") && collision.transform.position.y < transform.position.y - 1.25f )
            transform.SetParent(collision.transform);
    }
    
    private void Update()
    {
        var movement = Vector3.right * _direction;
        
        var hit = Physics2D.Raycast(transform.position + movement, Vector3.down, 1.5f);

        if (hit.collider != null && hit.collider.CompareTag("Ground") && transform.position.y - hit.transform.position.y is > 1.1f and < 1.4f)
            transform.Translate(movement * 0.03f);
        else
            _direction *= -1f;
    }
}
