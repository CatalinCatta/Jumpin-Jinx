using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CustomParticles : MonoBehaviour
{
    [SerializeField] public Sprite particleImage;
    [SerializeField] public bool uiElement = true, scaleByLength = true, stop, spawnWithCombinationOfSelectedColor = true;
    [SerializeField] public int quantity;
    [SerializeField] public float particleZPosition, particleMaxSideMovementPerFrame;
    [SerializeField] public Vector2 minAndMaxParticleSizes, spawnSize, fallSize, minAndMaxSpeed;
    [SerializeField] public Vector3 maxRotationAngleSpawn, particleMaxRotationPerFrame;
    [Range(0f, 100f)] [SerializeField] public float fadeParticleOnLastPercent;
    [SerializeField] public List<Color> colors;

    private Transform _transform;
    private Vector3 _position;
    private System.Random _random;
    private GameObject[] _particles;

    private void Start()
    {
        _transform = transform;
        _position = _transform.position;
        _random = new System.Random();
        _particles = new GameObject[quantity];
        if (stop) return;
        for (var i = 0; i < quantity; i++) CreateParticle(i, true);
    }

    private void LateUpdate()
    {
        for (var i = 0; i < quantity; i++)
            if (!stop && _particles[i] == null)
                CreateParticle(i, false);
    }

    public void ReleaseParticles()
    {
        for (var i = 0; i < quantity; i++) CreateParticle(-1, false);
    }

    private void CreateParticle(int position, bool start)
    {
        var newScale =
            (float)(_random.NextDouble() * (minAndMaxParticleSizes.y - minAndMaxParticleSizes.x) +
                    minAndMaxParticleSizes.x) / (scaleByLength ? particleImage.rect.size.x : particleImage.rect.size.y);
        var particleObject = position == -1
            ? new GameObject("Particle")
            : _particles[position] = new GameObject("Particle");
        var particleTransform = particleObject.transform;
        particleTransform.parent = _transform;
        particleTransform.localScale = new Vector3(newScale, newScale, 1);
        particleTransform.localPosition = new Vector3((float)(_random.NextDouble() * spawnSize.x / 2),
            (float)(-_random.NextDouble() * (start ? fallSize.y : spawnSize.y)), particleZPosition);
        particleTransform.rotation = Quaternion.Euler(Random.Range(-maxRotationAngleSpawn.x, maxRotationAngleSpawn.x),
            Random.Range(-maxRotationAngleSpawn.y, maxRotationAngleSpawn.y),
            Random.Range(-maxRotationAngleSpawn.z, maxRotationAngleSpawn.z));
        
        if (uiElement)
        {
            var image = particleObject.AddComponent<Image>();
            image.sprite = particleImage;
            image.raycastTarget = false;
            if (colors.Count > 0) image.color = GetParticleColor();
        }
        else
        {
            var spriteRenderer = particleObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = particleImage;
            if (colors.Count > 0) spriteRenderer.color = GetParticleColor();
        }

        var component = particleObject.AddComponent<IndividualParticle>();
        component.speed = (float)(_random.NextDouble() * (minAndMaxSpeed.y - minAndMaxSpeed.x) + minAndMaxSpeed.x);
        component.minAndMaxXPosition = new Vector2(-1, 1) * fallSize.x / 2;
        component.height = fallSize.y - (_position.y - particleObject.transform.position.y);
        component.fadePercent = fadeParticleOnLastPercent;
        component.z = particleZPosition;
        component.sideMaxMovementPerFrame = particleMaxSideMovementPerFrame;
        component.maxRotationPerFrame = particleMaxRotationPerFrame;
    }

    private Color GetParticleColor()
    {
        if (colors.Count == 1) return colors[0];
        if (!spawnWithCombinationOfSelectedColor) return colors[Utility.GetRandomNumberBetween(0, colors.Count)];
        
        var firstColorIndex = Utility.GetRandomNumberBetween(0, colors.Count);
        var firstColor = colors[firstColorIndex];
        var secondColor = firstColorIndex > 0
            ? firstColorIndex < colors.Count ? colors[(int)(_random.NextDouble() * 2) - 1] : colors[firstColorIndex - 1]
            : colors[firstColorIndex + 1];
        return Random.ColorHSV(Mathf.Min(firstColor.r, secondColor.r), Mathf.Max(firstColor.r, secondColor.r),
            Mathf.Min(firstColor.g, secondColor.g), Mathf.Max(firstColor.g, secondColor.g),
            Mathf.Min(firstColor.b, secondColor.b), Mathf.Max(firstColor.b, secondColor.b));
    }
}

public class IndividualParticle : MonoBehaviour
{
    [SerializeField] public float speed, z, height, sideMaxMovementPerFrame;
    [SerializeField] public Vector2 minAndMaxXPosition;
    [SerializeField] public Vector3 maxRotationPerFrame;
    [Range(0f, 100f)] [SerializeField] public float fadePercent;

    private Transform _transform;
    private System.Random _random;
    
    private void Start()
    {
        _transform = transform;
        _random = new System.Random();
    }
    
    private void Update()
    {
        var position = _transform.localPosition;
        var x = (float)(sideMaxMovementPerFrame * (_random.NextDouble() - 0.5) + position.x);
        x = x > minAndMaxXPosition.y ? minAndMaxXPosition.y : x < minAndMaxXPosition.x ? minAndMaxXPosition.x : x;
        var currentRotation = _transform.rotation.eulerAngles;
       
        _transform.rotation = Quaternion.Lerp(Quaternion.Euler(currentRotation),
            Quaternion.Euler(Random.Range(-maxRotationPerFrame.x, maxRotationPerFrame.x) + currentRotation.x,
                Random.Range(-maxRotationPerFrame.y, maxRotationPerFrame.y) + currentRotation.y,
                Random.Range(-maxRotationPerFrame.z, maxRotationPerFrame.z) + currentRotation.z),
            Mathf.Clamp01(speed * Time.deltaTime));
        position = _transform.localPosition = new Vector3(x, position.y - speed * Time.deltaTime, z);

        var target = -height + height * fadePercent / 100;
        if (position.y < -height)
        {
            Destroy(gameObject);
            return;
        }
        if (position.y > target) return;
        
        if (TryGetComponent<Image>(out var image))
        {
            var oldColor = image.color;
            image.color = new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.InverseLerp(-height, target, position.y));
        }
        else
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var oldColor = spriteRenderer.color;
            spriteRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b,
                Mathf.InverseLerp(-height, target, position.y));
        }
    }
}