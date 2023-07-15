using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformType platformType;

    private int _horizontalDistance = 5;
    private int _verticalDistance = 5;
    private int _rotationAngle = 3;
    private int _rotationSpeed = 2;
   
    [SerializeField] private List<Sprite> plants;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject coin;
    
    private void Start()
    {
        PlantEnvironment();
        
        switch (platformType)
        {
            case PlatformType.VerticalMoving:
                transform.position += Vector3.forward;
                StartCoroutine(MoveSideways(new Vector3(0f, Utils.RandomPickNumberBetween(_verticalDistance, _verticalDistance * 2), 0f)));
                break;
            
            case PlatformType.HorizontalMoving:
                transform.position += Vector3.forward * 2;
                StartCoroutine(MoveSideways(new Vector3(Utils.RandomPickNumberBetween(_horizontalDistance, _horizontalDistance * 2), 0f, 0f)));
                break;
            
            case PlatformType.CircularMoving:
                transform.position += Vector3.forward * 3;
                StartCoroutine(MoveCircular());
                break;
            
        }
    }

    private void PlantEnvironment()
    {
        var random = new System.Random();

        var randomNumber = random.Next(100);

        switch (randomNumber)
        {
            case < 10:  //Enemy
                Instantiate(enemy, transform.position + Vector3.up, Quaternion.identity, transform);
                return;
                
            case < 30: // Coin
                Instantiate(coin, transform.position + Vector3.up * 3, Quaternion.identity, transform);
                return;

            case < 60: // Plants
                CreatePlant();
                return;
            
            default:   // Empty  
                return;
        }
    }

    private void CreatePlant()
    {
        var random = new System.Random();
        var randomPlant = Utils.RandomPickNumberExcludingZero(plants.Count);
        
        var plant = new GameObject("Plant");
        plant.transform.SetParent(transform);
        plant.transform.localPosition = new Vector2((float)(random.NextDouble() * 2 - 1), randomPlant > 1 ?
            (float)(random.NextDouble() * 0.5) + 0.9f :
            (float)(random.NextDouble() * 0.5f) + 3f);
        
        var plantRenderer = plant.AddComponent<SpriteRenderer>();
        plantRenderer.sprite = plants[randomPlant];
    }
    
    private IEnumerator MoveSideways(Vector3 distance)
    {
        var startPos = transform.position;
        var targetPos = startPos + distance;

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
            angle += _rotationSpeed * Time.deltaTime;
            var x = center.x + Mathf.Cos(angle) * _rotationAngle;
            var y = center.y + Mathf.Sin(angle) * _rotationAngle;
            var z = center.z; 

            transform.position = new Vector3(x, y, z);

            yield return null;
        }
    }
}
