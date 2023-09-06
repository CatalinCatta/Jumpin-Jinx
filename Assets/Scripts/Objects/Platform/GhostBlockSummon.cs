using UnityEngine;

public class GhostBlockSummon : MonoBehaviour   // TODO: Add it to editor.
{
    [SerializeField] private bool staticGhostBlock, followerGhostBlock;
    public GameObject ghost;
    
    private void Start()
    {
        var ghostBlockPrefab = PrefabManager.Instance.ghostBlock;
        var localTransform = transform;
        var position = localTransform.position;
        
        if (staticGhostBlock) ghost = Instantiate(ghostBlockPrefab, position, Quaternion.identity);
        if (followerGhostBlock) Instantiate(ghostBlockPrefab, position, Quaternion.identity, localTransform);
    }   
}