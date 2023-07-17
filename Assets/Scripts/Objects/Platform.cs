using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformType platformType;

    private SpriteRenderer _spriteRenderer;
    
    [SerializeField]private int movement;
    [SerializeField]private int rotationAngle;
    [SerializeField]private int rotationSpeed;
    
    [SerializeField] private List<Sprite> plants;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject spike;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject heal;
    [SerializeField] private Sprite dirt;
    
    [SerializeField] private bool endlessRun = true; 
    
    private void Start()
    {
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        if (endlessRun) 
            PlantEnvironment();
        movement = movement == 0 ? Utils.RandomPickNumberBetween(5, 10) : movement;
        rotationAngle = rotationAngle == 0 ? Utils.RandomPickNumberBetween(2, 6) : rotationAngle;
        rotationSpeed = rotationSpeed == 0 ? Utils.RandomPickNumberBetween(-3, 3) : rotationSpeed;
        
        switch (platformType)
        {
            case PlatformType.Temporary:
                SetUpTemporaryPlatform();
                transform.position += Vector3.back * 0.5f;
                break;
            
            case PlatformType.VerticalMoving:
                transform.position += Vector3.back;
                StartCoroutine(MoveSideways(new Vector3(0f, movement, 0f)));
                break;
            
            case PlatformType.HorizontalMoving:
                transform.position += Vector3.back * 2f;
                StartCoroutine(MoveSideways(new Vector3(movement, 0f, 0f)));
                break;
            
            case PlatformType.CircularMoving:
                transform.position += Vector3.back * 3f;
                StartCoroutine(MoveCircular());
                break;
            
        }
    }

    private void SetUpTemporaryPlatform()
    {
        _spriteRenderer.sprite = dirt;
        _spriteRenderer.color = Color.gray;
    }
    
    private void PlantEnvironment()
    {
        var random = new System.Random();

        var randomNumber = random.Next(100);
        var selfTransform = transform;
        var selfPosition = selfTransform.position;
        
        
        switch (randomNumber)
        {
            case < 5:               //Enemy
                Instantiate(enemy, selfPosition + Vector3.up + Vector3.back, Quaternion.identity, selfTransform);
                return;
            case < 10:              //Spike
                if (platformType != PlatformType.Temporary)
                    Instantiate(spike, selfPosition + Vector3.up * 1.5f + Vector3.back, Quaternion.identity, selfTransform);
                return;
                
            case < 25:              // Coin
                Instantiate(coin, selfPosition + Vector3.up * 3, Quaternion.identity, selfTransform);
                return;
                
            case < 30:              // Coin
                Instantiate(heal, selfPosition + Vector3.up * 3, Quaternion.identity, selfTransform);
                return;

            case < 60:              // Plants
                if (platformType != PlatformType.Temporary)
                    CreatePlant();
                return;
            
            default:                // Empty  
                return;
        }
    }

    private void CreatePlant()
    {
        var random = new System.Random();
        var randomPlant = Utils.RandomPickNumberExcludingZero(plants.Count);
        
        var plant = new GameObject("Plant");
        plant.transform.SetParent(transform);
        plant.transform.localPosition = new Vector3((float)(random.NextDouble() * 2 - 1), 
            (float)(random.NextDouble() * 0.5f) + (randomPlant > 1 ? 0.9f : 3f), -1);
        
        plant.AddComponent<SpriteRenderer>().sprite = plants[randomPlant];
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
            angle += rotationSpeed * Time.deltaTime;

            transform.position = new Vector3(center.x + Mathf.Cos(angle) * rotationAngle, center.y + Mathf.Sin(angle) * rotationAngle, center.z);

            yield return null;
        }
    }

    public IEnumerator DestroyTemporaryPlatform()
    {
        var elapsedTime = 0f;
        var originalColor = _spriteRenderer.color;

        while (elapsedTime < 2f)
        {
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - elapsedTime / 2f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        string[] entitiesTags = { "Player", "Enemy", "Consumable" };
            
        if (transform.childCount > 0)
            for (var i = 0; i < transform.childCount; i++)
                if (entitiesTags.Contains(transform.GetChild(i).tag))
                    transform.GetChild(i).parent = null;
        
        Destroy(gameObject);
    }
}
