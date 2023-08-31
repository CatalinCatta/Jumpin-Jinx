using UnityEngine;

/// <summary>
/// Controls the behavior of the enemy character.
/// </summary>
public class Enemy : MonoBehaviour
{
    private float _direction;
    private Vector2 _smoothVelocity;

    private void Start()
    {
        _direction = Utils.RandomPickNumberExcludingZero(2) == 1 ? -1f : 1f;
        transform.rotation = Quaternion.Euler(0, (_direction - 1) * -90, 0);
    }

    private void LateUpdate()
    {
        var rigidBody = GetComponent<Rigidbody2D>();
        var grounded = false;
        Transform platform = null;

        foreach (var objectCollider in Physics2D.OverlapAreaAll(transform.position + Vector3.right * _direction * 1.5f,
                     transform.position + Vector3.down))
        {
            if (!objectCollider.CompareTag("Ground"))
                continue;

            if (transform.position.y - objectCollider.transform.position.y < .5f)
            {
                grounded = false;
                break;
            }

            if (transform.position.y - objectCollider.transform.position.y is <= .5f or >= .9f ||
                objectCollider.TryGetComponent<PolygonCollider2D>(out _) ||
                objectCollider.transform == transform.parent) continue;

            platform = objectCollider.transform;
            grounded = true;
        }

        if (grounded)
        {
            rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, Vector2.right * _direction * 3,
                ref _smoothVelocity, .1f);
            transform.SetParent(platform);
        }
        else
        {
            _direction *= -1f;
            transform.rotation = Quaternion.Euler(0, (_direction - 1) * -90, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            StartCoroutine(Utils.PlaySoundOnDeath(gameObject));
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            if (collision.transform.position.y < transform.position.y - 1.25f)
                transform.SetParent(collision.transform);
        }
        else
        {
            Physics2D.IgnoreCollision(collision.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
            StartCoroutine(Utils.PlaySoundOnDeath(gameObject));
    }
}