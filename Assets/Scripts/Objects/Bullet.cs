using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a bullet shot by the player.
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private float _speed;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _speed = LvlManager.Instance.CurrentLvl == 0
            ? 1f + PlayerManager.Instance.Upgrades[(int)UpgradeType.Attack].Quantity * 0.18f
            : 5f; // 1f => 10f
        StartCoroutine(DestroyAfterDelay());
    }

    private void Update() => _rigidbody.velocity = Vector2.right * _speed;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            FindObjectOfType<PlayerStatus>().KillCounter++;
        }
        else if (col.gameObject.CompareTag("Ground")) Destroy(gameObject);
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(10f);

        var spriteRenderer = GetComponent<SpriteRenderer>();
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