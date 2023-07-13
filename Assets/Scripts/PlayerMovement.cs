using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 15f;
    public float jumpPower = 60f;

    private Vector3 _movement;
    private bool _jump;
    private Vector3 _velocity;
    private int _groundCollisions;

    private void Update()
    {
        _movement = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? Vector3.left :
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? Vector3.right : Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Space) && _groundCollisions > 0)
            _jump = true;
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
