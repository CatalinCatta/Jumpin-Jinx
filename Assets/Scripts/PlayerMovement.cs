using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 15f;
    public float jumpPower = 60f;

    private Vector3 _movement;
    private bool _jump;
    private Vector3 _velocity;
    private int _groundCollisions;
    private Animator _animator;

    [SerializeField] private List<RuntimeAnimatorController> playerAnimations;

    private void Start() =>
        _animator = transform.GetComponent<Animator>();
    
    private enum PlayerAnimationsNames
    {
        Idle = 0,
        Run,
        Jump
    }
    
    private void Update()
    {
        _movement = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? Vector3.left :
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? Vector3.right : Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Space) && _groundCollisions > 0)
            _jump = true;

        FlipCharacter();
        PlayAnimation();
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

    private void PlayAnimation()
    {
        if (_groundCollisions == 0)
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
        if (collision.gameObject.CompareTag("Ground"))
            _groundCollisions++;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            _groundCollisions--;
    }
}
