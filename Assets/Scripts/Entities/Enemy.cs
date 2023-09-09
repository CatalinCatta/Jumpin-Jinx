using UnityEngine;

/// <summary>
/// Controls the behavior of the enemy character.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    private float _direction;
    private Transform _transform;
    private Rigidbody2D _rigidBody;

    private Vector2 _smoothVelocity;

    private void Start()
    {
        _direction = Utility.GetRandomNumberExcludingZero(2) == 1 ? -1f : 1f;
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody2D>();
        
        _transform.rotation = Quaternion.Euler(0, (_direction - 1) * -90, 0);
    }

    private void LateUpdate()
    {
        var grounded = false;
        Transform platform = null;

        foreach (var objectCollider in Physics2D.OverlapAreaAll(_transform.position + Vector3.right * _direction * 1.5f,
                     _transform.position + Vector3.down)) 
        {
            if (!objectCollider.CompareTag("Ground")) continue;
            var objectColliderTransform = objectCollider.transform;
            
            if (_transform.position.y - objectColliderTransform.position.y < .5f)
            {
                grounded = false;
                break;
            }

            if (_transform.position.y - objectColliderTransform.position.y is <= .5f or >= .9f ||
                objectCollider.TryGetComponent<PolygonCollider2D>(out _) ||
                objectColliderTransform.transform == _transform.parent) continue;

            platform = objectColliderTransform;
            grounded = true;
        }

        if (grounded)
        {
            _rigidBody.velocity = Vector2.SmoothDamp(_rigidBody.velocity, Vector2.right * _direction * 3,
                ref _smoothVelocity, .1f);
            _transform.SetParent(platform);
        }
        else
        {
            _direction *= -1f;
            _transform.rotation = Quaternion.Euler(0, (_direction - 1) * -90, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var colliderGameObject = collision.gameObject;
        var colliderTransform = collision.transform;
        
        if (colliderGameObject.CompareTag("Death")) StartCoroutine(Utility.PlayDeathSoundAndCleanup(gameObject));
        else if (!colliderGameObject.CompareTag("Ground"))
            Physics2D.IgnoreCollision(colliderTransform.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        else if (colliderTransform.position.y < _transform.position.y - 1.25f) _transform.SetParent(colliderTransform);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet")) StartCoroutine(Utility.PlayDeathSoundAndCleanup(gameObject));
    }
}