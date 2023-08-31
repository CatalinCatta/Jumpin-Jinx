using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a bullet shot by the player.
/// </summary>
public class Bullet : MonoBehaviour
{
    private float _speed;

    private void Start()
    {
        _speed = LvlManager.Instance.CurrentLvl == 0 ? 1f + PlayerManager.Instance.AtkLvl * 0.18f : 5f; // 1f => 10f
        StartCoroutine(DestroyAfterDelay());
    }

    private void Update() =>
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            FindObjectOfType<PlayerStatus>().KillCounter++;
        }
        else if (col.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(9f);

        var spriteRenderer = transform.GetComponent<SpriteRenderer>();
        var elapsedTime = 0f;
        var originalColor = spriteRenderer.color;

        while (elapsedTime < 1f)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                Mathf.Lerp(1f, 0f, elapsedTime / 1f));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}