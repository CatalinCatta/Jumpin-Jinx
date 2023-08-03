using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformType platformType;

    private SpriteRenderer _spriteRenderer;
    
    public int movement;
    public int rotationAngle;
    public int rotationSpeed;
    
    [SerializeField] private List<Sprite> plants;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject spike;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject heal;
    [SerializeField] private Sprite dirt;
    
    [SerializeField] public bool endlessRun;

    [SerializeField] private GameObject ghostBlock;

    private bool _autoDestructionStarted;

    public bool inCollisionWithGhostBlock;
    
    private void Start()
    {
        if (platformType != PlatformType.CircularMoving)
        {
            var currentPosition = transform.position;
            
            Instantiate(ghostBlock, currentPosition, Quaternion.identity).GetComponent<GhostBlock>().originPlatform = this;
            var ghostBlockObject = Instantiate(ghostBlock, currentPosition, Quaternion.identity, transform).GetComponent<GhostBlock>();
            ghostBlockObject.originPlatform = this;
            ghostBlockObject.isMoving = true;
        }   
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (endlessRun) 
            PlantEnvironment();
        
        movement = movement == 0 ? Utils.RandomPickNumberBetween(5, 10) : movement;
        rotationAngle = rotationAngle == 0 ? Utils.RandomPickNumberBetween(2, 6) : rotationAngle;
        while (rotationSpeed == 0)
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
                var currentTransform = transform;
                currentTransform.localScale = new Vector3(1.1f, 1f, 1f);
                currentTransform.position += Vector3.back * 2f;
                GetComponent<BoxCollider2D>().size = new Vector2(1.13f, 1f);
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
                    Instantiate(spike, selfPosition + Vector3.up * 0.75f + Vector3.back, Quaternion.identity, selfTransform);
                return;
                
            case < 25:              // Coin
                Instantiate(coin, selfPosition + Vector3.up * 1.5f, Quaternion.identity, selfTransform);
                return;
                
            case < 30:              // Coin
                Instantiate(heal, selfPosition + Vector3.up * 1.5f, Quaternion.identity, selfTransform);
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
        var randomPlant = Utils.RandomPickNumberBetween(0, plants.Count);
        var plant = new GameObject("Plant");
        
        plant.transform.SetParent(transform);
        plant.transform.localPosition = new Vector3((float)(random.NextDouble() - 0.5f), 
            (float)(random.NextDouble() / 4) + (randomPlant > 1 ? 0.45f : 1.5f), -1);
        
        plant.AddComponent<SpriteRenderer>().sprite = plants[randomPlant];
    }
    
    private IEnumerator MoveSideways(Vector3 distance)
    {
        var startPos = transform.position;
        var targetPos = startPos + distance;

        while (true)
        {
            while (transform.position != startPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime);
                yield return null;
            }
            
            while (transform.position != targetPos && !inCollisionWithGhostBlock)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime);
                yield return null;
            }

            yield return null;
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
        if (_autoDestructionStarted)
            yield break;

        _autoDestructionStarted = true;
        
        var elapsedTime = 0f;
        var originalColor = _spriteRenderer.color;

        while (elapsedTime < 2f)
        {
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - elapsedTime / 2f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        string[] entitiesTags = { "Player", "Enemy", "Consumable" };
            
        _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        if (transform.childCount > 0)
            for (var i = 0; i < transform.childCount; i++)
                if (entitiesTags.Contains(transform.GetChild(i).tag))
                    transform.GetChild(i).parent = null;
        
        Destroy(gameObject);
    }
}
