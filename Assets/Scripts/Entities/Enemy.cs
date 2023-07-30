using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _direction;
    private Vector2 _smoothVelocity;
    
    private void Start()
    {
        _direction = Utils.RandomPickNumberExcludingZero(2) == 1 ? -1f : 1f;
        transform.rotation = Quaternion.Euler(0, (_direction - 1 ) * -90 , 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Death"))
            StartCoroutine(Utils.PlaySoundOnDeath(gameObject));
        if (collision.gameObject.CompareTag("Consumable") || collision.gameObject.CompareTag("Enemy") )
            Physics2D.IgnoreCollision(collision.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (collision.gameObject.CompareTag("Ground") && collision.transform.position.y < transform.position.y - 1.25f )
            transform.SetParent(collision.transform);
    }
    
    private void Update()
    {
        var rigidBody = GetComponent<Rigidbody2D>();
        var grounded = false;
        Transform platform = null;
        
        foreach (var objectCollider in Physics2D.OverlapAreaAll(transform.position + Vector3.right * _direction * 1.5f,
                     transform.position + Vector3.down))
        {
            if (!objectCollider.CompareTag("Ground"))
                continue;

            if (transform.position.y - objectCollider.transform.position.y < 0.5f)
                break;

            if (transform.position.y - objectCollider.transform.position.y is <= 0.5f or >= 0.9f || objectCollider.TryGetComponent<PolygonCollider2D>(out _) || objectCollider.transform == transform.parent) continue;

            platform = objectCollider.transform;
            grounded = true;
            break;
        }

        if (grounded)
        {
            rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, Vector2.right * _direction * 3,
                ref _smoothVelocity, 0.1f);
            transform.SetParent(platform);
        }
        else
        {
            _direction *= -1f;
            transform.rotation = Quaternion.Euler(0, (_direction - 1) * -90, 0);
        }
    }
}
