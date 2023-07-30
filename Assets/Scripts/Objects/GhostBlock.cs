using System;
using UnityEngine;

public class GhostBlock : MonoBehaviour
{
    public Platform originPlatform;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Ground"))
            return;
        
        var platform = col.GetComponent<Platform>();

        if (CheckPlatform(platform))
            platform.inCollisionWithGhostBlock = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Ground"))
            return;
        
        var platform = col.GetComponent<Platform>();

        if (CheckPlatform(platform))
            platform.inCollisionWithGhostBlock = false;
    }

    private bool CheckPlatform(Platform platform)
    {
        var platformPosition = platform.transform.position;
        var selfPosition = transform.position;

        return platform != originPlatform &&
               ((platform.platformType == PlatformType.HorizontalMoving &&
                 platformPosition.x <= selfPosition.x - 0.64f) ||
                (platform.platformType == PlatformType.VerticalMoving &&
                 platformPosition.y <= selfPosition.y - 0.64f));
    }
}
