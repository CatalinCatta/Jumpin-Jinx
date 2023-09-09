using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    [SerializeField] private bool collideWhitGhostBlock = true;
    protected Transform Transform;
    protected bool InCollisionWithGhostBlock;
    protected bool Endless;
    
    private void Start()
    {
        Transform = transform;
        Endless = ((LvlManager)IndestructibleManager.Instance).CurrentScene == Scene.Endless;
        SetUp();
        StartCoroutine(Move());
    }

    protected abstract void SetUp();
    protected abstract IEnumerator Move();
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collideWhitGhostBlock && collision.gameObject.CompareTag("GhostBlock"))
            InCollisionWithGhostBlock = true; // TODO: Add GhostBlock tag in editor.
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collideWhitGhostBlock && collision.gameObject.CompareTag("GhostBlock"))
            InCollisionWithGhostBlock = false; // TODO: Add GhostBlock tag in editor.
    }
}