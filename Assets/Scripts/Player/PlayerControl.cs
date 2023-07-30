using System.Collections.Generic;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private bool _movementTestActive;
    
    
    public float movementSpeed = 10f;
    public float jumpPower = 1000f;

    private Vector3 _movement;
    private bool _jump;
    private Vector3 _velocity;
    private float _fireTimer;
    private bool _isFireActivated;
    private Vector2 _smoothVelocity;
  
    private Animator _animator;
    private SpriteRenderer _sprite;
    private PlayerStatus _playerStatus;
    private PlayerAudioControl _playerAudioControl;

    [SerializeField] private List<RuntimeAnimatorController> playerAnimations;
    [SerializeField] private List<Sprite> playerSprites;
    [SerializeField] private GameObject bullet;

    private void Awake()
    {
        _playerStatus = transform.GetComponent<PlayerStatus>();
        _playerAudioControl = transform.GetComponent<PlayerAudioControl>();
    }
    
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
        if (Input.GetKeyDown(SettingsManager.Instance.pauseKeyCode))
        {
            Time.timeScale = Math.Abs(Time.timeScale - 1f);
            _playerStatus.display.transform.parent.GetChild(1).gameObject.SetActive(Time.timeScale < 1f);
            _playerStatus.display.gameObject.SetActive(Time.timeScale > 0f);
        }

        if (_playerStatus.freezeFromDamage || Time.timeScale == 0f)
            return;
        
        if (!IsGrounded())
            transform.SetParent(null);
            
        _movement = Input.GetKey(SettingsManager.Instance.moveLeftKeyCode) ? Vector3.left :
            Input.GetKey(SettingsManager.Instance.moveRightKeyCode) ? Vector3.right : Vector3.zero;

        if (IsGrounded() && Input.GetKeyDown(SettingsManager.Instance.jumpKeyCode))
        {
            _playerAudioControl.PlayJumpSound();
            _jump = true;
        }
        
        FlipCharacter();
    
        if (Input.GetKeyDown(SettingsManager.Instance.jumpBuffKeyCode) && !_playerStatus.jumpBuffActive)
            _playerStatus.ConsumeJumpBuff();
    
        if (Input.GetKeyDown(SettingsManager.Instance.speedBuffKeyCode) && !_playerStatus.speedBuffActive)
            _playerStatus.ConsumeSpeedBuff();
    
        if (Input.GetKeyDown(SettingsManager.Instance.fireKeyCode))
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
        if (_playerStatus.freezeFromDamage)
            return;
        
        if (_movement.x != 0)
            _playerAudioControl.PlayWalkSound();
        
        var rigidBody = GetComponent<Rigidbody2D>();
        
        rigidBody.velocity = 
            _movementTestActive ? Vector2.SmoothDamp(rigidBody.velocity, _movement * movementSpeed, ref _smoothVelocity, 0.1f) : new Vector2(_movement.x * movementSpeed, rigidBody.velocity.y); //Vector2.SmoothDamp(rigidBody.velocity, _movement * movementSpeed, ref _smoothVelocity, 0.1f);

        if (!_jump) return;

        if (_movementTestActive)
            rigidBody.velocity =
                Vector2.SmoothDamp(rigidBody.velocity, Vector2.up * jumpPower / 5f, ref _smoothVelocity, 0.1f);
        else
            rigidBody.AddForce(new Vector2(rigidBody.velocity.x, jumpPower)); // Vector2.SmoothDamp (rigidBody.velocity, Vector2.up * jumpPower, ref _smoothVelocity, 0.1f);
      
        _jump = false;
    }

    public void ChangeMovementTest() =>
        _movementTestActive = !_movementTestActive;
    
    
    private void Fire()
    {
        _animator.enabled = false;
        _sprite.sprite = playerSprites[(int)PlayerSprites.Shot];
        transform.GetChild(1).gameObject.SetActive(true);
        _playerAudioControl.PlayShootArrowSound();

        StartCoroutine(CreateBullet());
    }

    private IEnumerator CreateBullet()
    {
        yield return new WaitForSeconds(0.4f);
        
        var playerTransform = transform;
        
        Physics2D.IgnoreCollision(Instantiate(bullet,  playerTransform.position + new Vector3(
            playerTransform.rotation == Quaternion.Euler(0, 0, 0) ? -0.4f : 0.4f, 0.5f) ,playerTransform.rotation).GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    private void PlayAnimation()
    {
        _animator.enabled = true;
        _sprite.sprite = playerSprites[(int)PlayerSprites.Idle];
        transform.GetChild(1).gameObject.SetActive(false);

        var shadow = transform.GetChild(0).gameObject;
        
        if (!IsGrounded())
        {
            if (_animator.runtimeAnimatorController == playerAnimations[(int)PlayerAnimationsNames.Jump])
                return;
                
            _animator.runtimeAnimatorController = playerAnimations[(int)PlayerAnimationsNames.Jump];
            shadow.SetActive(false);
        }
        else
        {
            shadow.SetActive(true);
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
        if (!collision.gameObject.CompareTag("Enemy"))
            return;
                    
        var colliderSize = (Vector3)Utils.GetColliderSize(collision);
        var position = transform.position;
        var colliderPosition = collision.transform.position;
        _playerStatus.GetDamage(
            position - colliderPosition - new Vector3(colliderSize.x * (position.x < colliderPosition.x ? 1 : -1) , colliderSize.y * (position.y < colliderPosition.y ? 1 : -1), 0) , 5);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
            if (contact.collider.gameObject.CompareTag("Death") && !_playerStatus.speedBuffActive)
                _playerStatus.Die();
    }
    
    private bool IsGrounded()
    {
        var playerPosition = transform.position;
        
        foreach (var objectCollider in Physics2D.OverlapAreaAll(playerPosition + new Vector3(-0.35f, 0, 0), playerPosition + new Vector3(0.35f, -0.5f, 0)))
        {
            if (!objectCollider.CompareTag("Ground"))
                continue;
            
            transform.SetParent(objectCollider.transform);
            
            if (objectCollider.gameObject.TryGetComponent<Platform>(out var platform) &&
                platform.platformType == PlatformType.Temporary)
                StartCoroutine(platform.DestroyTemporaryPlatform());
            
            return true;
        }
        return false;
    }
}
