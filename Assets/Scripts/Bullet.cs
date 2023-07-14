using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
            Destroy(gameObject);
    }
}
