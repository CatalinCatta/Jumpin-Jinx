using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const float Speed = 5f;

    private void Start() =>
        StartCoroutine(DestroyAfterDelay());

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(9f);
        
        var spriteRenderer = transform.GetComponent<SpriteRenderer>();
        var elapsedTime = 0f;
        var originalColor = spriteRenderer.color;

        while (elapsedTime < 1f)
        {
            var normalizedTime = elapsedTime / 1;
            var fadeAlpha = Mathf.Lerp(1f, 0f, normalizedTime);

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, fadeAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    
    private void Update() =>
        transform.Translate(Vector3.left * Speed * Time.deltaTime);
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
            Destroy(gameObject);
    }
    
    
}
