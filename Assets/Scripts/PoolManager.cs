using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<Pool> pools;
    
    public static PoolManager Instance { get; private set; }

    private Dictionary<Constants.Type, Queue<GameObject>> _poolDictionary;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        _poolDictionary = new Dictionary<Constants.Type, Queue<GameObject>>();
        var levelData = Constants.LevelDataDictionary[PlayerData.Instance.Level];

        foreach (var pool in pools)
        {
            var objectPool = new Queue<GameObject>();

            for (var i = 0; i < levelData.TypeSizeDictionary[pool.type]; i++)
            {
                var obj = Instantiate(pool.prefab, transform, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            _poolDictionary.Add(pool.type, objectPool);
            Debug.Log($"Initialized pool for: {pool.type} with size: {levelData.TypeSizeDictionary[pool.type]}");
        }
    }
    
    public GameObject SpawnFromPool(Constants.Type type, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool with tag {type} doesn't exist.");
            return null;
        }

        if (_poolDictionary[type].Count == 0)
        {
            return null;
        }

        var objectToSpawn = _poolDictionary[type].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        var poolableObject = objectToSpawn.GetComponent<IPoolable>();
        poolableObject?.OnSpawnFromPool();

        return objectToSpawn;
    }

    public void ReturnToPool(Constants.Type type, GameObject objectToReturn)
    {
        if (!_poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool with tag {type} doesn't exist.");
            Destroy(objectToReturn);
            return;
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(transform);
        _poolDictionary[type].Enqueue(objectToReturn);

        var poolableObject = objectToReturn.GetComponent<IPoolable>();

        poolableObject?.OnReturnToPool();
    }

    private bool IsInPool(Constants.Type type)
    {
        return _poolDictionary[type].Count > 0;
    }
    
    public bool IsTotalEnemiesSpawned()
    {
        foreach (var enemyType in Constants.EnemyTypes)
        {
            if (IsInPool(enemyType))
            {
                return false;
            }
        }
        
        return true;
    }
}