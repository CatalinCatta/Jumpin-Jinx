using UnityEngine;

/// <summary>
/// Represents a ghost block that collide with platforms to stop their movement.
/// </summary>
public class GhostBlock : MonoBehaviour
{
    public Platform originPlatform;
    public bool isMoving;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Ground") || !col.TryGetComponent<Platform>(out var platform))
            return;

        if (platform == originPlatform)
            Physics2D.IgnoreCollision(col.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        if (CheckPlatform(platform))
            platform.inCollisionWithGhostBlock = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Ground") && col.TryGetComponent<Platform>(out var platform) &&
            CheckPlatform(platform))
            platform.inCollisionWithGhostBlock = false;
    }

    private bool CheckPlatform(Platform platform)
    {
        var platformPosition = platform.transform.position;
        var selfPosition = transform.position;

        return
            (platform.platformType == PlatformType.HorizontalMoving &&
             platformPosition.x <= selfPosition.x - .64f &&
             platformPosition.y < selfPosition.y + .2f &&
             platformPosition.y > selfPosition.y - .2f &&
             (originPlatform == null || originPlatform.platformType != PlatformType.HorizontalMoving || isMoving)) ||
            (platform.platformType == PlatformType.VerticalMoving &&
             platformPosition.y <= selfPosition.y - .64f &&
             platformPosition.x < selfPosition.x + .2f &&
             platformPosition.x > selfPosition.x - .2f &&
             (originPlatform == null || originPlatform.platformType != PlatformType.VerticalMoving || isMoving));
    }
}