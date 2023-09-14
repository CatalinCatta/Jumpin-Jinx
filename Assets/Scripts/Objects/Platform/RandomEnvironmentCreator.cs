using UnityEngine;
using Random = System.Random;

public class RandomEnvironmentCreator : MonoBehaviour   // TODO: Add it to editor.
{
    private Random _random;
    private Transform _transform;
    private PrefabManager _prefabManager;

    [SerializeField] public bool allowDamageObject = true;
    [SerializeField] public bool allowConsumableObject = true;
    [SerializeField] public bool allowPlants = true;
    
    private void Start()
    {
        if (LvlManager.Instance.CurrentLvl != (int)Scene.Endless) return;

        _random = new Random();
        _transform = transform;
        _prefabManager = PrefabManager.Instance;
        CreateEnvironment();
    }

    private void CreateEnvironment()
    {
        var selfPosition = _transform.position;
        
        switch (_random.Next(100))
        {
            case >= 0 and < 5: // Enemy 5%
                if (allowDamageObject)
                    Instantiate(_prefabManager.enemy, selfPosition + Vector3.up + Vector3.back, Quaternion.identity,
                        _transform);
                return;

            case >= 5 and < 10: // Spike 5%
                if (allowDamageObject)
                    Instantiate(_prefabManager.spike, selfPosition + Vector3.up * .75f + Vector3.back,
                        Quaternion.identity, _transform);
                return;

            case >= 10 and < 15: // Heal 5%
                Instantiate(_prefabManager.heal, selfPosition + Vector3.up * 1.5f, Quaternion.identity, _transform);
                return;
            
            case >= 15 and < 30: // Coin 15%
                Instantiate(_prefabManager.coin, selfPosition + Vector3.up * 1.5f, Quaternion.identity, _transform);
                return;

            case >= 30 and < 60: // Plants 30%
                CreatePlant();
                return;
        }
        // Empty 40%
    }

    private void CreatePlant()
    {
        if (_prefabManager.plantSprites.Count == 0) return;
        
        var randomPlant = Utility.GetRandomNumberBetween(0, _prefabManager.plantSprites.Count);
        var plant = new GameObject("Plant");

        plant.transform.SetParent(_transform);
        plant.transform.localPosition = new Vector3((float)(_random.NextDouble() - .5f),
            (float)(_random.NextDouble() / 4) + (randomPlant > 1 ? .45f : 1.5f), -1);

        plant.AddComponent<SpriteRenderer>().sprite = _prefabManager.plantSprites[randomPlant];
    }
}