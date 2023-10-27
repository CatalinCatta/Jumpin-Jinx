using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    [SerializeField] public bool collideWhitGhostBlock = true;
    protected Transform Transform;
    protected bool InCollisionWithGhostBlock, Endless;
    
    private void Start()
    {
        Transform = transform;
        Endless = LvlManager.Instance.CurrentScene == Scene.Endless;
        SetUp();
        StartCoroutine(Move());
    }

    protected abstract void SetUp();
    protected abstract IEnumerator Move();


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (collideWhitGhostBlock && col.gameObject.CompareTag("GhostBlock"))InCollisionWithGhostBlock = true;
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (collideWhitGhostBlock && col.gameObject.CompareTag("GhostBlock"))InCollisionWithGhostBlock = false;
    }
}