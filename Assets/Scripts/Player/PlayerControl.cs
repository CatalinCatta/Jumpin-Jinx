using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class controls the player character in the game.
/// </summary>
[RequireComponent(typeof(PlayerStatus), typeof(Animator), typeof(PlayerAudioControl))]
public class PlayerControl : MonoBehaviour
{
    [Header("Player Settings")] [NonSerialized]
    public float MovementSpeed, JumpPower;

    [Header("Activities")] private bool _isFireActivated, _isShooting, _jump, _movementTestActive;

    [Header("Animation Names")]
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int InAir = Animator.StringToHash("InAir");
    
    [Header("Movement Handler Vectors")] private Vector3
        _movement,
        _velocity;
    private Vector2 _smoothVelocity;

    [Header("Player Components")]
    private Transform _localTransform;
    private PlayerAudioControl _playerAudioControl;
    private PlayerStatus _playerStatus;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    [Header("Utilities")]
    [SerializeField] private GameObject bullet;
    private PlayerManager _playerManager;
    private SettingsManager _settingsManager;
    private bool _endlessRun;
    
    private void Awake()
    {
        _localTransform = transform;
        _playerStatus = GetComponent<PlayerStatus>();
        _playerAudioControl = GetComponent<PlayerAudioControl>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _endlessRun = LvlManager.Instance.CurrentLvl == 0;
        _playerManager = PlayerManager.Instance;
        _settingsManager = SettingsManager.Instance;
        
        MovementSpeed = _endlessRun
            ? 7f * (_playerManager!.Upgrades[(int)UpgradeType.MovementSpeed].Quantity / 20f + 1f)
            : 10f; //   7f =>   24.5f
        JumpPower = _endlessRun
            ? 700f * (_playerManager!.Upgrades[(int)UpgradeType.JumpPower].Quantity / 20f + 1f)
            : 1000f; // 700f => 2450f
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(_settingsManager.PauseKeyCode)) Pause();

        if (_playerStatus.FreezeFromDamage || Time.timeScale == 0f) return;

        _movement = Input.GetKey(_settingsManager.MoveLeftKeyCode) ? Vector3.left :
            Input.GetKey(_settingsManager.MoveRightKeyCode) ? Vector3.right : Vector3.zero;

        var grounded = IsGrounded();

        _animator.SetBool(InAir, grounded);

        if (grounded)
        {
            if (Input.GetKeyDown(_settingsManager.JumpKeyCode))
            {
                _playerAudioControl.PlayJumpSound();
                _jump = true;
            }
        }
        else transform.SetParent(null);

        if (Input.GetKeyDown(_settingsManager.JumpBuffKeyCode) && !_playerStatus.JumpBuffActive)
            _playerStatus.ConsumeJumpBuff();

        if (Input.GetKeyDown(_settingsManager.SpeedBuffKeyCode) && !_playerStatus.SpeedBuffActive)
            _playerStatus.ConsumeSpeedBuff();

        if (!_isFireActivated && Input.GetKey(_settingsManager.FireKeyCode)) Fire();
    }

    /// <summary>
    /// Switch game state pause/resume. 
    /// </summary>
    public void Pause()
    {
        Time.timeScale = Math.Abs(Time.timeScale - 1f);
        _playerStatus.display.transform.parent.GetChild(1).gameObject.SetActive(Time.timeScale < 1f);
        _playerStatus.display.gameObject.SetActive(Time.timeScale > 0f);
    }
    
    private void FixedUpdate()
    {
        if (_playerStatus.FreezeFromDamage) return;

        if (_movement.x != 0)
        {
            FlipCharacter();
            _playerAudioControl.PlayWalkSound();
        }
        _animator.SetBool(Walk, _movement.x != 0);

        _rigidbody.velocity = _movementTestActive
            ? Vector2.SmoothDamp(_rigidbody.velocity, _movement * MovementSpeed, ref _smoothVelocity, .1f)
            : new Vector2(_movement.x * MovementSpeed, _rigidbody.velocity.y);

        if (!_jump) return;

        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);

        if (_movementTestActive)
            _rigidbody.velocity =
                Vector2.SmoothDamp(_rigidbody.velocity, Vector2.up * JumpPower / 5f, ref _smoothVelocity, .1f);
        else _rigidbody.AddForce(new Vector2(_rigidbody.velocity.x, JumpPower));

        _jump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;

        var colliderSize = (Vector3)Utility.RetrieveColliderSize(collision);
        var colliderTransform = collision.transform;
        var position = _localTransform.position;
        var colliderPosition = colliderTransform.position;
        
        _playerStatus.GetDamage(
            position - colliderPosition - new Vector3(colliderSize.x * (position.x < colliderPosition.x ? 1 : -1),
                colliderSize.y * (position.y < colliderPosition.y ? 1 : -1), 0), 5,
            colliderTransform.TryGetComponent<Enemy>(out _) ? ObjectBuildType.Spider :
            colliderTransform.name.Contains("Spike") ? ObjectBuildType.Spike : ObjectBuildType.Null);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
            if (contact.collider.gameObject.CompareTag("Death") && !_playerStatus.SpeedBuffActive)
                _playerStatus.Die();
    }

    /// <summary>
    /// Switch movement velocity.
    /// </summary>
    public void ChangeMovementTest() => _movementTestActive = !_movementTestActive;

    private void Fire()
    {
        if (_isFireActivated) return;

        // TODO: Add animation.
        
        _playerAudioControl.PlayShootArrowSound();

        StartCoroutine(CreateBullet());
    }

    private IEnumerator CreateBullet()
    {
        _isFireActivated = true;

        yield return new WaitForSeconds(_endlessRun
            ? 1.25f - PlayerManager.Instance.Upgrades[(int)UpgradeType.Attack].Quantity * .0225f
            : .25f); // 1.25f => .125f

        Instantiate(bullet,
            _localTransform.position +
            new Vector3(_localTransform.rotation == Quaternion.Euler(0, 0, 0) ? -.4f : .4f, 1.5f),
            _localTransform.rotation);

        _isFireActivated = false;
    }

    /// <summary>
    /// Flips the character's sprite horizontally based on movement direction.
    /// </summary>
    private void FlipCharacter() =>
        _localTransform.rotation = Quaternion.Euler(0, _movement == Vector3.right ? 0 : 180, 0);

    /// <summary>
    /// Checks if the player is grounded by performing a physics overlap check.
    /// </summary>
    /// <returns><c>true</c> if the player is grounded; otherwise, <c>false</c>.</returns>
    private bool IsGrounded()
    {
        var playerPosition = _localTransform.position;

        foreach (var objectCollider in Physics2D.OverlapAreaAll(playerPosition + new Vector3(-.35f, 0, 0),
                     playerPosition + new Vector3(.35f, -.5f, 0)))
        {
            if (objectCollider.CompareTag("Ground"))
            {

                _localTransform.SetParent(objectCollider.transform);
                return true;
            }
            else if (objectCollider.CompareTag("Death") && _playerStatus.SpeedBuffActive)
            {
                transform.SetParent(null);
                return true;
            }
        }

        return false;
    }
}