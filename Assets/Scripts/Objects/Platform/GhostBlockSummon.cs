using System;
using UnityEngine;

/// <summary>
/// Create a ghost block that keep original position and a follower one.
/// Keep the follower block saved for essayer destroy. 
/// </summary>
public class GhostBlockSummon : MonoBehaviour
{
    [SerializeField] private bool staticGhostBlock = true, followerGhostBlock;
    [NonSerialized] public GameObject Ghost;
    
    private void Start()
    {
        var ghostBlockPrefab = PrefabManager.Instance.ghostBlock;
        var localTransform = transform;
        var position = localTransform.position;
        
        if (staticGhostBlock) Ghost = Instantiate(ghostBlockPrefab, position, Quaternion.identity);
        if (followerGhostBlock) Instantiate(ghostBlockPrefab, position, Quaternion.identity, localTransform);
    }   
}