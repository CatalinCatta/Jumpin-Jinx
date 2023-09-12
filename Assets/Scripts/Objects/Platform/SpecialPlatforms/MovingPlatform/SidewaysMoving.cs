using System.Collections;
using UnityEngine;

/// <summary>
/// Move GameObject horizontal or vertical with specified values. If its endless game mode will randomly auto select.
/// </summary>
public class SidewaysMoving : MovingObject    // TODO: Add it to editor.
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }
    
    [Range(-20.0f, 20.0f)]
    [SerializeField] public float movement;
    [SerializeField] public Direction direction;

    private Vector3 _distance;

    protected override void SetUp() => _distance = direction == Direction.Horizontal
        ? new Vector3(movement, 0f, 0f)
        : new Vector3(0f, movement, 0f);

    protected override IEnumerator Move()
    {
        var startPos = Transform.position;
        var targetPos = startPos + _distance;

        while (true)
        {
            yield return MoveToPosition(startPos);
            yield return MoveToPosition(targetPos, true);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPos, bool checkCollision = false)
    {
        while (Transform.position != targetPos)
        {
            Transform.position = Vector3.MoveTowards(Transform.position, targetPos, Time.deltaTime);
            yield return null;

            if (checkCollision && InCollisionWithGhostBlock) break;
        }
    }
}
