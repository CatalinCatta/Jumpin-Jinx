using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public float horizontalDistance = 5f;
    public float verticalDistance = 5f;
    public float rotationAngle = 3f;
    public float rotationSpeed = 2f;

    [SerializeField] private PlatformType platformType;
    
    private void Start()
    {
        switch (platformType)
        {
            case PlatformType.HorizontalMoving:
                StartCoroutine(MoveHorizontal());
                break;
            
            case PlatformType.VerticalMoving:
                StartCoroutine(MoveVertical());
                break;
            
            case PlatformType.CircularMoving:
                StartCoroutine(MoveCircular());
                break;
            
        }
    }


    private IEnumerator MoveHorizontal()
    {
        var startPos = transform.position;
        var targetPos = startPos + new Vector3(horizontalDistance, 0f, 0f);

        while (true)
        {
            while (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime);

                yield return null;
            }

            while (transform.position != startPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime);

                yield return null;
            }
        }
    }

    private IEnumerator MoveVertical()
    {
        var startPos = transform.position;
        var targetPos = startPos + new Vector3(0f, verticalDistance, 0f);

        while (true)
        {
            while (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime);

                yield return null;
            }

            while (transform.position != startPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime);

                yield return null;
            }
        }
    }

    private IEnumerator MoveCircular()
    {
        var center = transform.position;
        var angle = 0f;

        while (true)
        {
            angle += rotationSpeed * Time.deltaTime;
            var x = center.x + Mathf.Cos(angle) * rotationAngle;
            var y = center.y + Mathf.Sin(angle) * rotationAngle;
            var z = center.z; 

            transform.position = new Vector3(x, y, z);

            yield return null;
        }
    }
}
