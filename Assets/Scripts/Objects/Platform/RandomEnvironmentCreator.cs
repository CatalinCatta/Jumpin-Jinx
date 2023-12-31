﻿using System.Linq;
using UnityEngine;
using Random = System.Random;

public class RandomEnvironmentCreator : MonoBehaviour 
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
                    Instantiate(_prefabManager.spider, selfPosition + Vector3.back, Quaternion.identity, _transform);
                return;

            case >= 5 and < 10: // Spike 5%
                if (allowDamageObject)
                    Instantiate(_prefabManager.spike, selfPosition + new Vector3(0, .75f, -1), Quaternion.identity,
                        _transform);
                return;

            case >= 10 and < 15: // Heal 5%
                Instantiate(_prefabManager.heal, selfPosition + Vector3.up * 1.5f, Quaternion.identity, _transform);
                return;

            case >= 15 and < 30: // Coin 15%
                Instantiate(_prefabManager.coin, selfPosition + Vector3.up * 1.5f, Quaternion.identity, _transform);
                return;

            case >= 30 and < 31: // Gem 1%
                Instantiate(_prefabManager.gem, selfPosition + Vector3.up * 1.5f, Quaternion.identity, _transform);
                return;
            
            case >= 31 and < 60: // Plants 29%
                CreatePlant();
                return;
        }
        // Empty 40%
    }

    private void CreatePlant()
    {
        var plants = Dictionaries.ObjectBuild.Where(kv => kv.Value.category == ObjectBuildCategory.Plant).ToList();
        
        if (plants.Count == 1) return;

        Instantiate(plants[Utility.GetRandomNumberBetween(0, plants.Count)].Value.prefab,
            new Vector3((float)(_random.NextDouble() - .5f), (float)(_random.NextDouble() / 4), -1),
            Quaternion.identity, _transform);
    }
}