using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// If gameObject with tag player touch this, destroy it after <see cref="timer"/>(default 2s).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class TemporaryObject : MonoBehaviour
{
    private bool _autoDestructionStarted;
    private SpriteRenderer _spriteRenderer;
    private GameObject _ghost;
    [SerializeField] private float timer = 2f;
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (TryGetComponent<GhostBlockSummon>(out var ghostComponent)) _ghost = ghostComponent.Ghost;
    }

    /// <summary>
    /// Destroys the temporary platform gradually.
    /// </summary>
    private IEnumerator Destroy()
    {
        _autoDestructionStarted = true;

        var localTransform = transform;
        var elapsedTime = 0f;
        var originalColor = _spriteRenderer.color;
        string[] entitiesTags = {"Player", "Enemy"};

        while (elapsedTime < timer)
        {
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - elapsedTime / timer);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        for (var i = 0; i < localTransform.childCount; i++)
        {
            var child = localTransform.GetChild(i);
            if (entitiesTags.Contains(child.tag)) child.parent = null;
        }

        Destroy(gameObject);
        if (_ghost != null) Destroy(_ghost);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_autoDestructionStarted) StartCoroutine(Destroy());
    }
}