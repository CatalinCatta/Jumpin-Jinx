using System.Collections;
using UnityEngine;

/// <summary>
/// Rotate an object at a specific speed and angle. If speed goes negative will rotate object backwards.
/// On Endless Game Mode will get random rotation speed and angle and will pass true ghost blocks. 
/// </summary>
public class CircularMovingPlatform : MovingObject    // TODO: Add it to editor.
{
    [Range(-5f, 5f)] [SerializeField] public float rotationSpeed;
    [Range(0f, 10f)] [SerializeField] public float rotationAngle = 4f;

    protected override void SetUp()
    {
        if (LvlManager.Instance.gameMode != GameMode.Endless) return;
        
        rotationAngle = Utility.GetRandomNumberBetween(2, 6);
        while (rotationSpeed == 0) rotationSpeed = Utility.GetRandomNumberBetween(-3, 3);
    }

    protected override IEnumerator Move()
    {
        var center = Transform.position;
        var angle = 0f;

        while (true)
        {
            if (InCollisionWithGhostBlock) rotationSpeed *= -1;
            
            angle += rotationSpeed * Time.deltaTime;
            Transform.position = new Vector3(center.x + Mathf.Cos(angle) * rotationAngle,
                center.y + Mathf.Sin(angle) * rotationAngle, center.z);

            yield return null;
        }
    }
}