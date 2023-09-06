using UnityEngine;
using Random = System.Random;

public class RandomEnvironmentCreator : MonoBehaviour   // TODO: Add it to editor.
{
    private Random _random;
    private Transform _transform;
    private PrefabManager _prefabManager;
    
    private void Start()
    {
        if (LvlManager.Instance.gameMode != GameMode.Endless) return;

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
            case < 5: //Enemy
                Instantiate(_prefabManager.enemy, selfPosition + Vector3.up + Vector3.back, Quaternion.identity, _transform);
                return;

            case < 10: //Spike
                Instantiate(_prefabManager.spike, selfPosition + Vector3.up * .75f + Vector3.back, Quaternion.identity, _transform);
                return;

            case < 25: // Coin
                Instantiate(_prefabManager.coin, selfPosition + Vector3.up * 1.5f, Quaternion.identity, _transform);
                return;

            case < 30: // Coin
                Instantiate(_prefabManager.heal, selfPosition + Vector3.up * 1.5f, Quaternion.identity, _transform);
                return;

            case < 60: // Plants
                CreatePlant();
                return;

            default: // Empty  
                return;
        }
    }

    private void CreatePlant()
    {
        var randomPlant = Utility.GetRandomNumberBetween(0, _prefabManager.plantSprites.Count);
        var plant = new GameObject("Plant");

        plant.transform.SetParent(_transform);
        plant.transform.localPosition = new Vector3((float)(_random.NextDouble() - .5f),
            (float)(_random.NextDouble() / 4) + (randomPlant > 1 ? .45f : 1.5f), -1);

        plant.AddComponent<SpriteRenderer>().sprite = _prefabManager.plantSprites[randomPlant];
    }
}