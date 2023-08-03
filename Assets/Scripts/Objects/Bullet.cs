using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private float _speed;

    private void Start()
    {
        _speed = LvlManager.Instance.currentLvl == 0? 8f - PlayerManager.Instance.atkLvl * 0.15f : 5f;  // 8f => 0.5f
        StartCoroutine(DestroyAfterDelay());
    }
    
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(9f);
        
        var spriteRenderer = transform.GetComponent<SpriteRenderer>();
        var elapsedTime = 0f;
        var originalColor = spriteRenderer.color;
        
        while (elapsedTime < 1f)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, elapsedTime / 1f));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    
    private void Update() =>
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            FindObjectOfType<PlayerStatus>().killCounter++;
        }
        else if (collision.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }
    
    
}
