using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    [SerializeField] public bool collideWhitGhostBlock = true;
    protected Transform Transform;
    protected bool InCollisionWithGhostBlock, Endless;
    [NonSerialized] public List<GameObject> IgnoredObjects;
    
    private void Start()
    {
        Transform = transform;
        Endless = LvlManager.Instance.CurrentScene == Scene.Endless;
        IgnoredObjects = new List<GameObject>();
        SetUp();
        StartCoroutine(Move());
    }

    protected abstract void SetUp();
    protected abstract IEnumerator Move();
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var colliderObject = collision.gameObject;
        if (collideWhitGhostBlock && colliderObject.CompareTag("GhostBlock") &&
            !IgnoredObjects.Contains(colliderObject)) InCollisionWithGhostBlock = true;
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        var colliderObject = collision.gameObject;
        if (collideWhitGhostBlock && colliderObject.CompareTag("GhostBlock") &&
            !IgnoredObjects.Contains(colliderObject)) InCollisionWithGhostBlock = false;
    }
}