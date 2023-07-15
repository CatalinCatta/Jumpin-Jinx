using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformType platformType;

    private SpriteRenderer _spriteRenderer;
    
    private int _horizontalDistance = 5;
    private int _verticalDistance = 5;
    private int _rotationAngle = 3;
    private int _rotationSpeed = 2;
   
    [SerializeField] private List<Sprite> plants;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject spike;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject heal;
    [SerializeField] private Sprite dirt;
    
    private void Start()
    {
        if (platformType == PlatformType.Temporar)
            Debug.Log(transform.position);
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        PlantEnvironment();

        switch (platformType)
        {
            case PlatformType.Temporar:
                SetUpTemporarPlatform();
                transform.position += Vector3.back * 0.5f;
                break;
            
            case PlatformType.VerticalMoving:
                transform.position += Vector3.back;
                StartCoroutine(MoveSideways(new Vector3(0f, Utils.RandomPickNumberBetween(_verticalDistance, _verticalDistance * 2), 0f)));
                break;
            
            case PlatformType.HorizontalMoving:
                transform.position += Vector3.back * 2f;
                StartCoroutine(MoveSideways(new Vector3(Utils.RandomPickNumberBetween(_horizontalDistance, _horizontalDistance * 2), 0f, 0f)));
                break;
            
            case PlatformType.CircularMoving:
                transform.position += Vector3.back * 3f;
                StartCoroutine(MoveCircular());
                break;
            
        }
    }

    private void SetUpTemporarPlatform()
    {
        _spriteRenderer.sprite = dirt;
        _spriteRenderer.color = Color.gray;
    }
    
    private void PlantEnvironment()
    {
        var random = new System.Random();

        var randomNumber = random.Next(100);

        switch (randomNumber)
        {
            case < 5:               //Enemy
                Instantiate(enemy, transform.position + Vector3.up + Vector3.back, Quaternion.identity, transform);
                return;
            case < 10:              //Spike
                if (platformType != PlatformType.Temporar)
                    Instantiate(spike, transform.position + Vector3.up * 1.5f + Vector3.back, Quaternion.identity, transform);
                return;
                
            case < 25:              // Coin
                Instantiate(coin, transform.position + Vector3.up * 3, Quaternion.identity, transform);
                return;
                
            case < 30:              // Coin
                Instantiate(heal, transform.position + Vector3.up * 3, Quaternion.identity, transform);
                return;

            case < 60:              // Plants
                if (platformType != PlatformType.Temporar)
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

    public IEnumerator DestroyTemporarPlatform()
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
        
        if (transform.childCount > 0)
            for (var i = 0; i < transform.childCount; i++)
                transform.GetChild(i).parent = null;
        
        Destroy(gameObject);
    }
}
