using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float movementSpeed = 15f;
    public float jumpPower = 100f;

    private Vector3 _movement;
    private bool _jump;
    private Vector3 _velocity;
    private float _fireTimer;
    private bool _isFireActivated;
  
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = Math.Abs(Time.timeScale - 1f);
            _playerStatus.display.transform.parent.GetChild(1).gameObject.SetActive(Time.timeScale < 1f);
            _playerStatus.display.gameObject.SetActive(Time.timeScale > 0f);
        }

        if (_playerStatus.freezeFromDamage || Time.timeScale == 0f)
            return;
        
        if (!IsGrounded())
            transform.SetParent(null);
            
        _movement = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? Vector3.left :
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? Vector3.right : Vector3.zero;

        if (IsGrounded() && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            _playerAudioControl.PlayJumpSound();
            _jump = true;
        }
        
        FlipCharacter();
    
        if (Input.GetKeyDown(KeyCode.LeftControl) && !_playerStatus.jumpBuffActive)
            _playerStatus.ConsumeJumpBuff();
    
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !_playerStatus.speedBuffActive)
            _playerStatus.ConsumeSpeedBuff();
    
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
        if (_movement.x != 0)
            _playerAudioControl.PlayWalkSound();
        
        _velocity = Vector3.Lerp(_velocity, _movement * movementSpeed, 0.2f);

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
        if (collision.gameObject.CompareTag("Enemy"))
            _playerStatus.GetDamage(collision.transform.position, 5);
        
        if (collision.gameObject.CompareTag("Death") && !_playerStatus.speedBuffActive)
            _playerStatus.Die();
    }


    private bool IsGrounded()
    {
        var playerPosition = transform.position;
        foreach (var objectCollider in Physics2D.OverlapAreaAll(playerPosition + new Vector3(-0.35f, 0, 0), playerPosition + new Vector3(0.35f, -0.5f, 0)))
        {
            if (objectCollider.CompareTag("Ground"))
            {
                transform.SetParent(objectCollider.transform);
                if (objectCollider.gameObject.TryGetComponent<Platform>(out var platform) &&
                    platform.platformType == PlatformType.Temporary)
                    StartCoroutine(platform.DestroyTemporaryPlatform());
                return true;
            }
        }
        return false;
    }
}
