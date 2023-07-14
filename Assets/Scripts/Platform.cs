using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public float horizontalDistance = 5f;
    public float verticalDistance = 5f;
    public float rotationAngle = 3f;
    public float rotationSpeed = 2f;

    [SerializeField] private PlatformType platformType;
    [SerializeField] private List<Sprite> plants;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject coin;
    
    private void Start()
    {
        PlantEnvironment();
        
        switch (platformType)
        {
            case PlatformType.HorizontalMoving:
                transform.position += Vector3.forward;
                StartCoroutine(MoveHorizontal());
                break;
            
            case PlatformType.VerticalMoving:
                transform.position += Vector3.forward * 2;
                StartCoroutine(MoveVertical());
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
        var randomPlant = random.Next(0, plants.Count);
        
        var plant = new GameObject("Plant");
        plant.transform.SetParent(transform);
        plant.transform.localPosition = new Vector2((float)(random.NextDouble() * 2 - 1), randomPlant > 1 ?
            (float)(random.NextDouble() * 0.5) + 0.9f :
            (float)(
                random.NextDouble() * 0.5f) + 3f);
        
        var plantRenderer = plant.AddComponent<SpriteRenderer>();
        plantRenderer.sprite = plants[randomPlant];
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
