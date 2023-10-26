using UnityEngine;

/// <summary>
/// Controls the behavior of the spider character.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class Enemy : MonoBehaviour
{
    private float _direction;
    private Transform _transform;
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private Collider2D _collider2D;
    
    private Vector2 _smoothVelocity;
    private bool _isDead;
    
    private void Awake()
    {
        _direction = Utility.GetRandomNumberExcludingZero(2) == 1 ? -1f : 1f;
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        
        _transform.rotation = Quaternion.Euler(0, (_direction - 1) * -90, 0);
    }

    private void LateUpdate()
    {
        if (_isDead) return;

        var position = _transform.position;
        var isBlocked = true;
        var size = _collider2D.bounds.size; 
        
        foreach (var objectCollider in Physics2D.OverlapAreaAll(
                     position + new Vector3(size.x / 2 * _direction, size.y, 0),
                     position + Vector3.right * size.x * _direction * .55f)) 
        {
            if (!objectCollider.CompareTag("Ground")) continue;

            if (objectCollider.transform.position.y > position.y + .2f)
            {
                isBlocked = true;
                break;
            }

            isBlocked = false;
        }

        if (isBlocked)
        {
            _direction = -_direction;
            _transform.rotation = Quaternion.Euler(0, (_direction - 1) * -90, 0);
        }

        _rigidBody.velocity =
            Vector2.SmoothDamp(_rigidBody.velocity, Vector2.right * 2 * _direction, ref _smoothVelocity, .1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var colliderGameObject = collision.gameObject;
        var colliderTransform = collision.transform;
        var position = _transform.position;
        var col = colliderTransform.GetComponent<Collider2D>();
        
        if (colliderGameObject.CompareTag("Death"))
        {
            _isDead = true;
            _animator.Play("Die");
        }
        else if (!colliderGameObject.CompareTag("Ground"))
        {
            if (!colliderGameObject.CompareTag("Player")) Physics2D.IgnoreCollision(col, _collider2D);
        }
        else if (position.y > colliderTransform.position.y) _transform.SetParent(colliderTransform);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            _isDead = true;
            _animator.Play("Die");
        }
    }

    private void SelfDestroy() => Destroy(gameObject);
}