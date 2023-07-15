using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float movementSpeed = 15f;
    public float jumpPower = 100f;

    private Vector3 _movement;
    private bool _jump;
    private Vector3 _velocity;
    private readonly List<GameObject> _groundCollisions = new();
    private Animator _animator;
    private float _fireTimer;
    private bool _isFireActivated;
    private SpriteRenderer _sprite;
    private PlayerStatus _playerStatus;
    
    [SerializeField] private List<RuntimeAnimatorController> playerAnimations;
    [SerializeField] private List<Sprite> playerSprites;
    [SerializeField] private GameObject bullet;

    private void Awake() =>
        _playerStatus = transform.GetComponent<PlayerStatus>();

    private void Start()
    {
        _animator = transform.GetComponent<Animator>();
        _sprite = transform.GetComponent<SpriteRenderer>();
    }
    
    private enum PlayerAnimationsNames
    {
        Idle = 0,
        Run,
        Jump
    }
    
    private enum PlayerSprites
    {
        Idle = 0,
        Shot
    }

    private void Update()
    {
        if (_playerStatus.FreezedFromDamage)
            return;

        if (_groundCollisions.Count == 0)
            transform.SetParent(null);
            
        _movement = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? Vector3.left :
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? Vector3.right : Vector3.zero;

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _groundCollisions.Count > 0)
            _jump = true;
      
        FlipCharacter();
    
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
            _fireTimer = Time.time;
            _isFireActivated = true;
        }

        if (!_isFireActivated)
            PlayAnimation();

        if (Time.time - _fireTimer >= 0.4f)
            _isFireActivated = false;
    }

    private void FixedUpdate()
    {

        var targetVelocity = _movement * movementSpeed;
        _velocity = Vector3.Lerp(_velocity, targetVelocity, 0.2f);

        var newPosition = transform.position + _velocity * Time.fixedDeltaTime;

        if (_jump)
        {
            _velocity.y = jumpPower;
            _jump = false;
        }

        _velocity.y += Physics.gravity.y * Time.fixedDeltaTime;

        transform.position = newPosition;
    }
    
    private void Fire()
    {
        _animator.enabled = false;
        _sprite.sprite = playerSprites[(int)PlayerSprites.Shot];
        transform.GetChild(1).gameObject.SetActive(true);

        StartCoroutine(CreateBullet());
    }

    private IEnumerator CreateBullet()
    {
        yield return new WaitForSeconds(0.4f);
        
        var playerTransform = transform;
        var bulletObject = Instantiate(bullet,  playerTransform.position + new Vector3(
            playerTransform.rotation == Quaternion.Euler(0, 0, 0) ? -0.4f : 0.4f, 0.5f) ,playerTransform.rotation);
    
        Physics2D.IgnoreCollision(bulletObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    private void PlayAnimation()
    {
        _animator.enabled = true;
        _sprite.sprite = playerSprites[(int)PlayerSprites.Idle];
        transform.GetChild(1).gameObject.SetActive(false);

        if (_groundCollisions.Count == 0)
        {
            if (_animator.runtimeAnimatorController == playerAnimations[(int)PlayerAnimationsNames.Jump])
                return;
                
            _animator.runtimeAnimatorController = playerAnimations[(int)PlayerAnimationsNames.Jump];
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            if (_movement != Vector3.zero)
            {
                if (_animator.runtimeAnimatorController != playerAnimations[(int)PlayerAnimationsNames.Run])
                    _animator.runtimeAnimatorController = playerAnimations[(int)PlayerAnimationsNames.Run];
            }
            else if (_animator.runtimeAnimatorController != playerAnimations[(int)PlayerAnimationsNames.Idle])
                _animator.runtimeAnimatorController = playerAnimations[(int)PlayerAnimationsNames.Idle];
        }
    }

    private void FlipCharacter()
    {
        if (_movement == Vector3.left)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_movement == Vector3.right)
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && collision.transform.position.y < transform.position.y - 0.9f )
        {
            _groundCollisions.Add(collision.gameObject);
            transform.SetParent(collision.transform);
        }
        
        if (collision.gameObject.CompareTag("Enemy"))
            _playerStatus.GetDamage(collision.transform.position);
        
        if (collision.gameObject.CompareTag("Death"))
            _playerStatus.Die();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && _groundCollisions.Contains(collision.gameObject))
            _groundCollisions.Remove(collision.gameObject);
    }
}
